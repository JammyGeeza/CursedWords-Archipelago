using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using Mod.Helpers;
using Mod.Mappings;
using Mod.Patches;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

namespace Modd
{
    [BepInPlugin("archipelago", "Cursed Words Archipelago", "0.0.0")]
    public class CursedWordsArchipelago : BaseUnityPlugin
    {
        #region Private Properties

        private static Harmony Harmony { get; set; }

        private ConcurrentQueue<Func<IEnumerator>> ActionQueue { get; set; } = new ConcurrentQueue<Func<IEnumerator>>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The current instance of this mod.
        /// </summary>
        public static CursedWordsArchipelago Instance { get; private set; }

        /// <summary>
        /// Is the player currently in game? (Not in the save selection screen)
        /// </summary>
        public bool IsInGame { get; set; } = false;

        public ManualLogSource LogSource
        {
            get => Logger;
        }

        /// <summary>
        /// All un-checked shop stamp checks
        /// </summary>
        public Dictionary<long, ScoutedItemInfo> RemainingShopStampChecks { get; set; } = new Dictionary<long, ScoutedItemInfo>();

        /// <summary>
        /// All un-checked shop stamp checks
        /// </summary>
        public Dictionary<long, ScoutedItemInfo> RemainingShopStickerChecks { get; set; } = new Dictionary<long, ScoutedItemInfo>();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Logger.LogInfo("Success, the mod has loaded!");

            // Initialize Harmony
            Logger.LogInfo("Applying patches...");

            HarmonyFileLog.Enabled = true;
            Harmony = new Harmony("archipelago");
            Harmony.PatchAll();

            Logger.LogInfo("Patches applied");

            // Set instance
            if (Instance != null && Instance != this)
            {
                Logger.LogWarning("Duplicate mod instance detected. Destroying...");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Register event handlers
            ArchipelagoHelper.OnConnected += ArchipelagoHelper_OnConnected;
            ArchipelagoHelper.OnDeathlink += ArchipelagoHelper_OnDeathlink;
            ArchipelagoHelper.OnCheckedLocationsUpdated += ArchipelagoHelper_OnCheckedLocationsUpdated;
            ArchipelagoHelper.OnItemsReceived += ArchipelagoHelper_OnItemsReceived;

            Logger.LogMessage("Clearing bulk unlocks");
            BulkUnlock.AllBulkUnlocks.Clear();

            // Insert custom bulk uploads as requiring unlocks
            Logger.LogInfo("Inserting custom bulk unlocks");
            foreach (Type type in Lookups.ValidBulkUnlockTypes)
            {
                BulkUnlock unlock = Activator.CreateInstance(type) as BulkUnlock;

                //Logger.LogInfo($"\t{unlock.Name}");

                BulkUnlock.AllBulkUnlocks.Add(unlock);
            }

            DustyCoffin dc = new DustyCoffin();
            var sprite = dc.SpriteData.First().GetSprite();

            Logger.LogInfo($"Existing sprite texture size: {sprite.texture.width}x{sprite.texture.height}");
            Logger.LogInfo($"Existing sprite rect: {sprite.rect}");
            Logger.LogInfo($"Existing sprite bounds: {sprite.bounds}");

            var sdfSprite = dc.SpriteData.First().GetSDFSprite();
            Logger.LogInfo($"Existing SDF sprite offset: {sdfSprite.Metadata.BorderOffset}");
            Logger.LogInfo($"Source sprite: {sdfSprite.Metadata.SourceSprite.texture.width}x{sdfSprite.Metadata.SourceSprite.texture.height} format: {sdfSprite.Metadata.SourceSprite.texture.format}");
            Logger.LogInfo($"SDF sprite: {sdfSprite.Metadata.SDFSprite.texture.width}x{sdfSprite.Metadata.SDFSprite.texture.height} format: {sdfSprite.Metadata.SDFSprite.texture.format}");

        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        private void Update()
        {
            if (!Instance.IsInGame)
            {
                return;
            }

            // De-queue action and perform it
            if (Instance.ActionQueue.TryDequeue(out Func<IEnumerator> action))
            {
                StartCoroutine(action());
            }

            // Development short-cuts
            if (UnityInput.Current.GetKeyUp(KeyCode.F2))
            {
                Logger.LogInfo($"F2 key-up");

                // Get controller
                //if (FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
                //{
                //    // Complete current encounter
                //    controller.DevCompleteEncounter();
                //    //controller.DevWinGame();
                //}
            }
            else if (UnityInput.Current.GetKeyUp(KeyCode.F3))
            {
                Logger.LogInfo($"F3 key-up");

                //// Get controller
                //if (FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
                //{
                //    // Fail encounter
                //    controller.DevFailEncounter();
                //}
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an action to the action queue.
        /// </summary>
        /// <param name="action">The action to queue.</param>
        public void QueueAction(Func<IEnumerator> action)
        {
            Instance.ActionQueue.Enqueue(action);
        }

        /// <summary>
        /// Try to check a location by its name.
        /// </summary>
        /// <param name="locationName">The name of the location.</param>
        public void TryCheckLocation(string locationName)
        {
            QueueAction(() => CheckLocation(locationName));
        }

        /// <summary>
        /// Attempt to check an encounter location.
        /// </summary>
        /// <param name="character">The character to check against.</param>
        /// <param name="stage">The stage to check against.</param>
        /// <param name="nodeType">The encounter node type to check against.</param>
        public void TryCheckEncounterLocations(Character character, int stage, NodeType nodeType)
        {
            foreach (LocationCriteria criteria in ItemMappings.Locations.Where(l => l.OnEncounterAction?.Invoke(character, stage, nodeType) == true))
            {
                Logger.LogWarning($"Criteria met for location check: '{criteria.LocationName}'");
                TryCheckLocation(criteria.LocationName);
            }
        }

        /// <summary>
        /// Attempt to check a generic location.
        /// </summary>
        /// <param name="action">The action to check against.</param>
        public void TryCheckGenericLocations(string action)
        {
            foreach (LocationCriteria criteria in ItemMappings.Locations.Where(l => l.OnGenericAction?.Invoke(action) == true))
            {
                Logger.LogWarning($"Criteria met for location check: '{criteria.LocationName}'");
                TryCheckLocation(criteria.LocationName);
            }
        }

        /// <summary>
        /// Attempt to check a numeric location.
        /// </summary>
        /// <param name="action">The action to check against.</param>
        /// <param name="amount">The amount to check against.</param>
        public void TryCheckNumericLocations(string action, long amount)
        {
            foreach (LocationCriteria criteria in ItemMappings.Locations.Where(l => l.OnNumericAction?.Invoke(action, amount) == true))
            {
                Logger.LogWarning($"Criteria met for location check: '{criteria.LocationName}'");
                TryCheckLocation(criteria.LocationName);
            }
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Check a location by its name.
        /// </summary>
        /// <param name="locationName">The name of the location to check.</param>
        static IEnumerator CheckLocation(string locationName)
        {
            ArchipelagoHelper.TryCheckLocation(locationName);
            yield break;
        }

        /// <summary>
        /// Handle a deathlink received.
        /// </summary>
        /// <param name="deathlink">The received deathlink object.</param>
        static IEnumerator HandleDeathlink(DeathLink deathlink)
        {
            // Check if in an encounter
            if (FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
            {
                // Ignore death as received via deathlink
                MusicController_Patches.IgnoreDeath = true;

                // Trigger encounter failure
                controller.DevFailEncounter();
            }
            else
            {
                Instance.Logger.LogWarning("Deathlink skipped - not currently in an encounter.");
            }

            yield break;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for checked locations updated.
        /// </summary>
        private void ArchipelagoHelper_OnCheckedLocationsUpdated(System.Collections.ObjectModel.ReadOnlyCollection<long> newCheckedLocations)
        {
            foreach (long checkedLocation in newCheckedLocations)
            {
                Logger.LogInfo($"Checked location updated: {ArchipelagoHelper.GetLocationName(checkedLocation)}");

                // Attempt to remove from unchecked shop items
                RemainingShopStampChecks.Remove(checkedLocation);
                RemainingShopStickerChecks.Remove(checkedLocation);
            }
        }

        /// <summary>
        /// Event handler for connection established to archipelago session.
        /// </summary>
        private async void ArchipelagoHelper_OnConnected()
        {
            Logger.LogMessage("Connected to archipelago");

            // Get un-checked shop checks
            List<long> uncheckedShopStampChecks = ArchipelagoHelper.GetUncheckedLocationsByName("Shop Stamp Item");
            List<long> uncheckedShopStickerChecks = ArchipelagoHelper.GetUncheckedLocationsByName("Shop Sticker Item");
            if (uncheckedShopStampChecks.Count == 0 && uncheckedShopStickerChecks.Count == 0)
            {
                return;
            }

            // Scout and store un-checked shop checks
            RemainingShopStampChecks = await ArchipelagoHelper.ScoutLocationsByIdAsync(uncheckedShopStampChecks.ToArray());
            RemainingShopStampChecks = RemainingShopStampChecks.OrderBy((kvp) => kvp.Value.LocationName).ToDictionary(x => x.Key, x => x.Value);

            RemainingShopStickerChecks = await ArchipelagoHelper.ScoutLocationsByIdAsync(uncheckedShopStickerChecks.ToArray());
            RemainingShopStickerChecks = RemainingShopStickerChecks.OrderBy((kvp) => kvp.Value.LocationName).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Event handler for items received from archipelago session
        /// </summary>
        /// <param name="helper"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ArchipelagoHelper_OnItemsReceived(ReceivedItemsHelper helper)
        {
            while (helper.DequeueItem() is ItemInfo itemInfo)
            {
                Logger.LogWarning($"Item received: {itemInfo.ItemName}");

                // Add item action to queue, if exists
                if (ItemMappings.Map.TryGetValue(itemInfo.ItemName, out Func<IEnumerator> action))
                {
                    Logger.LogWarning($"Queueing item received action for '{itemInfo.ItemName}'...");
                    QueueAction(action);
                }
            }
        }

        /// <summary>
        /// Event handler for deathlink received from archipelago session.
        /// </summary>
        /// <param name="deathLink">The received deathlink</param>
        private void ArchipelagoHelper_OnDeathlink(DeathLink deathLink)
        {
            Logger.LogInfo($"Queueing deathlink from '{deathLink.Source}'");
            QueueAction(() => HandleDeathlink(deathLink));
        }

        #endregion
    }
}

using Archipelago.MultiClient.Net.Models;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Modd
{
    [BepInPlugin("archipelago", "Cursed Words Archipelago", "0.0.0")]
    public class CursedWordsArchipelago : BaseUnityPlugin
    {
        public static CursedWordsArchipelago Instance = null;
        private static Harmony Harmony = null;

        // Private fields
        private ConcurrentQueue<Func<IEnumerator>> actionQueue = new ConcurrentQueue<Func<IEnumerator>>();

        /// <summary>
        /// Is the player currently in game? (Not in the save selection screen)
        /// </summary>
        public bool IsInGame { get; set; } = false;

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
                Debug.LogWarning("Duplicate mod instance detected. Destroying...");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Register event handlers
            ArchipelagoHelper.OnConnected += ArchipelagoHelper_OnConnected;
            ArchipelagoHelper.OnCheckedLocationsUpdated += ArchipelagoHelper_OnCheckedLocationsUpdated;
            ArchipelagoHelper.OnItemsReceived += ArchipelagoHelper_OnItemsReceived;

            Debug.Log("Printing blue stamps...");
            foreach (Type type in Lookups.BlueStamps)
            {
                Debug.Log($"\t{type.Name}");
            }

            Debug.Log("Printing blue stickers...");
            foreach (Type type in Lookups.BlueStickers)
            {
                Debug.Log($"\t{type.Name}");
            }
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
            if (Instance.actionQueue.TryDequeue(out Func<IEnumerator> action))
            {
                Debug.Log("Dequeuing action...");
                StartCoroutine(action());
            }
        }

        /// <summary>
        /// Event handler for checked locations updated.
        /// </summary>
        private void ArchipelagoHelper_OnCheckedLocationsUpdated(System.Collections.ObjectModel.ReadOnlyCollection<long> newCheckedLocations)
        {
            foreach (long checkedLocation in newCheckedLocations)
            {
                Logger.LogInfo($"Checked location updated: {ArchipelagoHelper.GetLocationName(checkedLocation)}");
            }
        }

        /// <summary>
        /// Event handler for connection established to archipelago session.
        /// </summary>
        private void ArchipelagoHelper_OnConnected()
        {
            Logger.LogMessage("Connected to archipelago");
        }

        /// <summary>
        /// Event handler for items received from archipelago session
        /// </summary>
        /// <param name="helper"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ArchipelagoHelper_OnItemsReceived(Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            while (helper.DequeueItem() is ItemInfo itemInfo)
            {
                Logger.LogInfo($"Item received: {itemInfo.ItemName}");

                // Add item action to queue, if exists
                if (ItemMappings.Map.TryGetValue(itemInfo.ItemName, out Func<IEnumerator> action))
                {
                    Logger.LogInfo($"Queuing item action for {itemInfo.ItemName}...");

                    QueueAction(action);
                }
            }
        }

        /// <summary>
        /// Add an action to the action queue.
        /// </summary>
        /// <param name="action">The action to queue.</param>
        public void QueueAction(Func<IEnumerator> action)
        {
            Instance.actionQueue.Enqueue(action);
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
                QueueAction(() => CheckLocation(criteria.LocationName));
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
                QueueAction(() => CheckLocation(criteria.LocationName));
            }
        }

        /// <summary>
        /// Attempt to check a location by its action and amount.
        /// </summary>
        /// <param name="action">The action to check against.</param>
        /// <param name="amount">The amount to check against.</param>
        public void TryCheckWordLocations(string action, long amount)
        {
            foreach (LocationCriteria criteria in ItemMappings.Locations.Where(l => l.OnWordAction?.Invoke(action, amount) == true))
            {
                QueueAction(() => CheckLocation(criteria.LocationName));
            }
        }

        static IEnumerator CheckLocation(string locationName)
        {
            ArchipelagoHelper.CheckLocation(locationName);
            yield break;
        }
    }
}

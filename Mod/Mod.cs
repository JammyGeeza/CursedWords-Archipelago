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
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

namespace Modd
{
    [BepInPlugin("archipelago", "Cursed Words Archipelago", "0.5.0")]
    public class CursedWordsArchipelago : BaseUnityPlugin
    {
        #region Private Properties

        private static Harmony Harmony { get; set; }

        private ConcurrentQueue<(Func<IEnumerator> Action, string ItemName)> ActionQueue { get; set; } = new ConcurrentQueue<(Func<IEnumerator>, string)>();

        private Dictionary<string, int> PendingItems { get; } = new Dictionary<string, int>();

        private Coroutine _currentAction;

        private Dictionary<Type, string> _characterTypeCache;

        private Dictionary<Type, (string name, ItemRarity rarity)> _itemTypeCache;

        /// <summary>
        /// Gets or sets all locations relevant to the connected slot.
        /// </summary>
        private List<LocationCriteria> RelevantLocations { get; set; } = new List<LocationCriteria>();

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

        /// <summary>
        /// Cache item names to types to prevent needing to do this multiple times.
        /// </summary>
        public Dictionary<Type, string> CharacterTypeCache =>
            _characterTypeCache ??= Assembly.GetAssembly(typeof(Character))
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(Character)))
                .ToDictionary(t => t, t => (Activator.CreateInstance(t) as Character).GetName());

        /// <summary>
        /// Cache item names to types to prevent needing to do this multiple times.
        /// </summary>
        public Dictionary<Type, (string name, ItemRarity rarity)> ItemTypeCache =>
            _itemTypeCache ??= Assembly.GetAssembly(typeof(Item))
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(Item)))
                .Select(t => Activator.CreateInstance(t) as Item)
                .Where(t => t.UpgradeableComponents.Count < 2)
                .ToDictionary(x => x.GetType(), x => (x.Name, x.Rarity));

        public ManualLogSource LogSource
        {
            get => Logger;
        }

        /// <summary>
        /// All un-checked shop stamp checks
        /// </summary>
        public Dictionary<long, ScoutedItemInfo> RemainingShopChecks { get; set; } = new Dictionary<long, ScoutedItemInfo>();

        /// <summary>
        /// Gets or sets the current save slot.
        /// </summary>
        public static int SaveSlot { get; set; }

        /// <summary>
        /// Gets or sets the currently unlocked items.
        /// </summary>
        public List<Type> UnlockedItemTypeCache = new List<Type>();

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
            ArchipelagoHelper.OnDisconnected += ArchipelagoHelper_OnDisconnected;
            ArchipelagoHelper.OnCheckedLocationsUpdated += ArchipelagoHelper_OnCheckedLocationsUpdated;
            ArchipelagoHelper.OnItemsReceived += ArchipelagoHelper_OnItemsReceived;

            Logger.LogMessage("Clearing bulk unlocks");
            BulkUnlock.AllBulkUnlocks.Clear();

            // Insert custom bulk uploads as requiring unlocks
            //Logger.LogInfo("Inserting custom bulk unlocks");
            //foreach (Type type in Lookups.ValidBulkUnlockTypes)
            //{
            //    BulkUnlock unlock = Activator.CreateInstance(type) as BulkUnlock;
            //    BulkUnlock.AllBulkUnlocks.Add(unlock);
            //}
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

            if (_currentAction == null && Instance.ActionQueue.TryDequeue(out (Func<IEnumerator> Action, string ItemName) queued))
            {
                _currentAction = StartCoroutine(RunAction(queued.Action, queued.ItemName));
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
        /// Get the current pending count for an item.
        /// </summary>
        /// <param name="itemName">The item name to check.</param>
        public int GetPendingCount(string itemName)
        {
            return PendingItems.GetValueOrDefault(itemName, 0);
        }

        /// <summary>
        /// Add an action to the action queue.
        /// </summary>
        /// <param name="action">The action to queue.</param>
        /// <param name="itemName">The optional item name to track pending actions.</param>
        public void QueueAction(Func<IEnumerator> action, string itemName = null)
        {
            if (itemName != null)
            {
                PendingItems[itemName] = PendingItems.GetValueOrDefault(itemName, 0) + 1;
            }

            Instance.ActionQueue.Enqueue((action, itemName));
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
        /// <param name="action">The event action name.</param>
        /// <param name="player">The current player object.</param>
        /// <param name="bossModifiers">The boss modifiers applied to the encounter.</param>
        /// <param name="args">Any additional arguments.</param>
        public void TryCheckEncounterLocations(string action, Player player, List<BossModifier>? bossModifiers = null, object args = null)
        {
            foreach (LocationCriteria criteria in RelevantLocations.Where(l => l.OnEncounterAction?.Invoke(action, player, bossModifiers ?? new List<BossModifier>(), args ?? string.Empty) == true))
            {
                TryCheckLocation(criteria.LocationName);
            }
        }

        /// <summary>
        /// Attempt to check a generic location.
        /// </summary>
        /// <param name="action">The action to check against.</param>
        public void TryCheckGenericLocations(string action)
        {
            foreach (LocationCriteria criteria in RelevantLocations.Where(l => l.OnGenericAction?.Invoke(action) == true))
            {
                TryCheckLocation(criteria.LocationName);
            }
        }

        /// <summary>
        /// Attempt to check Item Action locations.
        /// </summary>
        /// <param name="action">The item action name.</param>
        /// <param name="item">The item.</param>
        public void TryCheckItemActionLocations(string action, Item item)
        {
            foreach (LocationCriteria criteria in RelevantLocations.Where(l => l.OnItemAction?.Invoke(action, item) == true))
            {
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
            foreach (LocationCriteria criteria in RelevantLocations.Where(l => l.OnNumericAction?.Invoke(action, amount) == true))
            {
                TryCheckLocation(criteria.LocationName);
            }
        }

        /// <summary>
        /// Attempt to check a numeric location.
        /// </summary>
        /// <param name="action">The action to check against.</param>
        /// <param name="amount">The amount to check against.</param>
        public void TryCheckTileLocations(string action, Tile tile)
        {
            foreach (LocationCriteria criteria in RelevantLocations.Where(l => l.OnTileAction?.Invoke(action, tile) == true))
            {
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

        private IEnumerator RunAction(Func<IEnumerator> action, string itemName)
        {
            yield return action();

            if (itemName != null)
            {
                PendingItems[itemName] = Math.Max(0, PendingItems.GetValueOrDefault(itemName, 0) - 1);
            }

            _currentAction = null;
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
                RemainingShopChecks.Remove(checkedLocation);
            }

            // Update location counts in AP Data
            ArchipelagoData apData = ArchipelagoData.GetDataForSaveSlot(SaveSlot);
            int checkedLocations = ArchipelagoHelper.GetCheckedLocationsCount();
            apData.LocationsCheckedTotal = checkedLocations;
            apData.LocationsTotal = ArchipelagoHelper.GetUncheckedLocationsCount() + checkedLocations;

            // Save it
            ArchipelagoData.SaveDataForSaveSlot(SaveSlot, apData);
        }

        /// <summary>
        /// Event handler for connection established to archipelago session.
        /// </summary>
        private async void ArchipelagoHelper_OnConnected()
        {
            Logger.LogMessage("Connected to archipelago");

            // Store relevant locations for the connected session so that smaller seeds
            // aren't checking against irrelevant location criteria
            List<string> locationNames = ArchipelagoHelper.GetAllLocationNames();
            RelevantLocations = ItemMappings.Locations
                .Where(lc => locationNames.Contains(lc.LocationName))
                .ToList();

            // Get un-checked shop checks
            List<long> uncheckedShopChecks = ArchipelagoHelper.GetUncheckedLocationsByName("Shopsanity: Item");
            if (uncheckedShopChecks.Count == 0)
            {
                return;
            }

            // Scout and store un-checked shop checks
            RemainingShopChecks = await ArchipelagoHelper.ScoutLocationsByIdAsync(uncheckedShopChecks.ToArray());
            RemainingShopChecks = RemainingShopChecks.OrderBy((kvp) => kvp.Value.LocationName).ToDictionary(x => x.Key, x => x.Value);
        }

        private void ArchipelagoHelper_OnDisconnected(string reason)
        {
            // Clear unlocked items cache
            UnlockedItemTypeCache.Clear();
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
                if (ItemMappings.Map.TryGetValue(itemInfo.ItemName, out CuedAction cuedAction))
                {
                    Logger.LogWarning($"Queueing item received action for '{itemInfo.ItemName}'...");
                    QueueAction(cuedAction.Action, itemInfo.ItemName);
                }

                if (itemInfo.ItemName.Equals("Progressive Item Rarity", StringComparison.OrdinalIgnoreCase))
                {
                    RefreshUnlockedItemTypesCache();
                }
                else
                {
                    KeyValuePair<Type, (string name, ItemRarity rarity)> itemType = ItemTypeCache.FirstOrDefault(kvp => kvp.Value.name.Equals(itemInfo.ItemName, StringComparison.OrdinalIgnoreCase));
                    if (itemType.Key != null && !UnlockedItemTypeCache.Contains(itemType.Key))
                    {
                        Logger.LogInfo($"Adding item type: {itemType.Key} to unlocked item cache");
                        UnlockedItemTypeCache.Add(itemType.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Check if an item rarity has been unlocked via progressive item rarity, if enabled.
        /// </summary>
        /// <param name="rarity">The item rarity to check.</param>
        /// <returns>True if unlocked, False if not.</returns>
        private bool IsRarityUnlocked(ItemRarity rarity)
        {
            return !ArchipelagoHelper.SlotData.ShuffleItemRarities ||
                rarity switch
                {
                    ItemRarity.Rare or ItemRarity.Legendary => ArchipelagoHelper.HasReceivedItem("Progressive Item Rarity", (int)rarity),
                    _ => true
                };
        }

        /// <summary>
        /// Re-evaluate the unlocked item types cache.
        /// </summary>
        private void RefreshUnlockedItemTypesCache()
        {
            // Cycle each item type not already in the unlocked cache
            foreach (KeyValuePair<Type, (string name, ItemRarity rarity)> itemType in ItemTypeCache.Where(kvp => !UnlockedItemTypeCache.Contains(kvp.Key)))
            {
                // If item has been received and the rarity is unlocked, add to the cache
                if (ArchipelagoHelper.HasReceivedItem(itemType.Value.name) && IsRarityUnlocked(itemType.Value.rarity))
                {
                    Logger.LogInfo($"Adding item type: {itemType.Key} to unlocked item cache");
                    UnlockedItemTypeCache.Add(itemType.Key);
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

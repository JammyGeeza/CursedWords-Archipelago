using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using Mod.Classes;
using Mod.Enums;
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
    [BepInPlugin("archipelago", "Cursed Words Archipelago", "0.5.2")]
    public class CursedWordsArchipelago : BaseUnityPlugin
    {
        #region Private Properties

        private static Harmony Harmony { get; set; }

        private ConcurrentQueue<(CuedAction, ItemInfo)> ActionQueue { get; set; } = new ConcurrentQueue<(CuedAction, ItemInfo)>();

        private Dictionary<Type, string> _characterTypeCache;

        private Dictionary<Type, (string name, ItemRarity rarity)> _itemTypeCache;

        /// <summary>
        /// Items that have been successfully handled.
        /// </summary>
        private Dictionary<string, int> HandledItems { get; set; } = new();

        /// <summary>
        /// Items that are pending being handled.
        /// </summary>
        private Dictionary<string, int> PendingItems { get; set; } = new();

        /// <summary>
        /// Items that have been received this session.
        /// </summary>
        private Dictionary<string, int> ReceivedItems { get; set; } = new();

        /// <summary>
        /// Gets or sets all locations relevant to the connected slot.
        /// </summary>
        private List<LocationCriteria> RelevantLocations { get; set; } = new List<LocationCriteria>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Cache item names to types to prevent needing to do this multiple times.
        /// </summary>
        public Dictionary<Type, string> CharacterTypeCache =>
            _characterTypeCache ??= Assembly.GetAssembly(typeof(Character))
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(Character)))
                .ToDictionary(t => t, t => (Activator.CreateInstance(t) as Character).GetName());

        /// <summary>
        /// Gets or sets the amount of items that have been handled.
        /// </summary>
        public int HandledItemCount { get; set; }

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
            // Initialize Harmony
            Logger.LogDebug("Applying patches...");

            HarmonyFileLog.Enabled = true;
            Harmony = new Harmony("archipelago");
            Harmony.PatchAll();

            // Set instance
            if (Instance != null && Instance != this)
            {
                Logger.LogWarning("Duplicate mod instance detected. Destroying...");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Logger.LogDebug("Registering event handlers...");

            // Register event handlers
            ArchipelagoConsoleHelper.Instance.OnUserInput += ArchipelagoConsoleHelper_OnMessageSubmit;

            ActionQueueHelper.Instance.OnActionCompleted += ActionQueueHelper_OnActionCompleted;
            ArchipelagoHelper.OnConnected += ArchipelagoHelper_OnConnected;
            ArchipelagoHelper.OnDisconnected += ArchipelagoHelper_OnDisconnected;
            ArchipelagoHelper.OnMessageReceived += ArchipelagoHelper_OnMessageReceived;

            // Clear all bulk unlocks (to prevent accidental item unlocking)
            BulkUnlock.AllBulkUnlocks.Clear();

            // Insert custom bulk uploads as requiring unlocks
            //Logger.LogInfo("Inserting custom bulk unlocks");
            //foreach (Type type in Lookups.ValidBulkUnlockTypes)
            //{
            //    BulkUnlock unlock = Activator.CreateInstance(type) as BulkUnlock;
            //    BulkUnlock.AllBulkUnlocks.Add(unlock);
            //}

            //Logger.LogInfo($"Stickers:");
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

            // Process next item in the queue
            ActionQueueHelper.Instance.ProcessNext();

            //if (_currentAction == null && Instance.ActionQueue.TryDequeue(out (CuedAction Action, ItemInfo Item) queuedAction))
            //{
            //    _currentAction = StartCoroutine(RunQueuedAction(queuedAction.Action, queuedAction.Item));
            //}

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
        /// Check if a character has met the current goal criteria.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns>True if met, false if not met.</returns>
        public bool HasCharacterMetGoalCriteria(Character character)
        {
            return HasCharacterMetGoalCriteria(character.GetType());
        }

        /// <summary>
        /// Check if a character has met the current goal criteria.
        /// </summary>
        /// <param name="characterType">The type of character to check.</param>
        /// <returns>True if met, false if not met.</returns>
        public bool HasCharacterMetGoalCriteria(Type characterType)
        {
            return ArchipelagoHelper.SlotData.GoalType switch
            {
                GoalType.Crowns => SaveManager.GetHighestCompletedAscension(characterType) >= ArchipelagoHelper.SlotData.CrownRequirement,
                GoalType.Michael => SaveManager.HasBeatenFinalBoss(characterType),
                GoalType.Runs => SaveManager.GetHighestCompletedAscension(characterType) >= 0,
                _ => false,
            };
        }

        /// <summary>
        /// Add an action to the action queue.
        /// </summary>
        /// <param name="action">The action to queue.</param>
        /// <param name="itemName">The optional item name to track pending actions.</param>
        public void QueueAction(Func<IEnumerator<bool>> action, string actionName = "")
        {
            // If action name, increment tracking
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                PendingItems[actionName] = PendingItems.GetValueOrDefault(actionName, 0) + 1;
            }

            // Queue action
            ActionQueueHelper.Instance.Enqueue(action, actionName);
        }

        /// <summary>
        /// Add un-processed Encounter cued items to the queue.
        /// </summary>
        public void QueueUnprocessedEncounterItems()
        {
            foreach (KeyValuePair<string, CuedAction> item in Items.AllItems.Where(item => item.Value.Cue is ActionCue.Encounter))
            {
                int timesReceived = ReceivedItems.GetValueOrDefault(item.Key);
                int timesProcessed = GetItemProcessedCount(item.Key);

                for (int i = timesReceived - timesProcessed; i > 0; i--)
                {
                    QueueAction(item.Value.Action, item.Key);
                }
            }
        }

        /// <summary>
        /// Try to check a location by its name.
        /// </summary>
        /// <param name="locationName">The name of the location.</param>
        public void TryCheckLocation(string locationName)
        {
            ActionQueueHelper.Instance.Enqueue(() => CheckLocation(locationName));
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

        #region Private Methods

        /// <summary>
        /// Get the amount of times an item has been processed (including already handled and currently queued)
        /// </summary>
        /// <param name="itemName">The name of the item to check.</param>
        /// <returns>The amount of times this item has been processed.</returns>
        private int GetItemProcessedCount(string itemName)
        {
            return HandledItems.GetValueOrDefault(itemName, 0) + PendingItems.GetValueOrDefault(itemName, 0);
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
                    UnlockedItemTypeCache.Add(itemType.Key);
                }
            }
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Check a location by its name.
        /// </summary>
        /// <param name="locationName">The name of the location to check.</param>
        private IEnumerator<bool> CheckLocation(string locationName)
        {
            bool success = false;

            try
            {
                ArchipelagoHelper.TryCheckLocation(locationName);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to check location '{locationName}'. Reason: {ex.Message}");
            }

            yield return success;
        }

        /// <summary>
        /// Handle a deathlink received.
        /// </summary>
        /// <param name="deathlink">The received deathlink object.</param>
        private IEnumerator<bool> HandleDeathlink(DeathLink deathlink)
        {
            bool success = false;

            Logger.LogDebug($"Attempting to handle Deathlink...");

            // Check if in an encounter
            if (FindFirstObjectByType<EncounterController>() is EncounterController controller && controller != null)
            {
                try
                {
                    // Ignore death as received via deathlink
                    MusicController_Patches.IgnoreDeath = true;

                    // Trigger encounter failure
                    controller.DevFailEncounter();

                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogMessage($"Failed to handle Deathlink. Reason: {ex.Message}");
                }
            }
            else
            {
                Logger.LogDebug("No currently active encounter found - skipped.");
            }

            yield return success;
        }

        /// <summary>
        /// Display a notification.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="duration">The duration to display the notification for.</param>
        private IEnumerator<bool> Notify(string text, float duration = 2.5f)
        {
            bool success = false;

            try
            {
                NotificationHelper.Instance.Enqueue(text, duration);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to display notification. Reason: {ex.Message}");
            }

            yield return success;
        }

        /// <summary>
        /// Display a notification.
        /// </summary>
        /// <param name="itemInfo">The item to display a notification for.</param>
        /// <param name="duration">The duration to display the notification for.</param>
        private IEnumerator<bool> Notify(ItemInfo itemInfo, float duration = 2.5f)
        {
            string itemPart = $"<#{Colours.GetColourForItemFlag(itemInfo.Flags)}>{itemInfo.ItemName}</color>";

            // If sent by reserved AP slot, display as 'server'
            if (itemInfo.Player.Slot <= 0)
            {
                return Notify($"Server sent your {itemPart}", 2.0f);
            }

            // Otherwise, display as sending player
            string senderPart = itemInfo.Player.Slot == ArchipelagoHelper.Slot
                ? $"<#{Colours.GetColourHex(Archipelago.MultiClient.Net.Models.Color.Magenta)}>You</color>"
                : $"<#{Colours.GetColourHex(Archipelago.MultiClient.Net.Models.Color.Yellow)}>{itemInfo.Player}</color>";

            return Notify($"{senderPart} found your {itemPart}");
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for user input in the console.
        /// </summary>
        /// <param name="message">The message submitted.</param>
        private void ArchipelagoConsoleHelper_OnMessageSubmit(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ArchipelagoHelper.TrySendMessage(message);
            }
        }

        /// <summary>
        /// Event handler for a tracked action completing.
        /// </summary>
        private void ActionQueueHelper_OnActionCompleted(bool success, string name)
        {
            // Mark as handled if success
            if (success)
            {
                HandledItems[name] = HandledItems.GetValueOrDefault(name, 0) + 1;

                // Persist handled items count
                ArchipelagoHelper.SetValue("handled_items", HandledItems);
            }

            // Remove from pending regardless
            PendingItems[name] = Math.Max(0, PendingItems.GetValueOrDefault(name, 0) - 1);
        }

        /// <summary>
        /// Event handler for checked locations updated.
        /// </summary>
        private void ArchipelagoHelper_OnCheckedLocationsUpdated(System.Collections.ObjectModel.ReadOnlyCollection<long> newCheckedLocations)
        {
            foreach (long checkedLocation in newCheckedLocations)
            {
                Logger.LogDebug($"Checked location updated: {ArchipelagoHelper.GetLocationName(checkedLocation)}");

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
            Logger.LogMessage($"Successfully connected to the archipelago room!");

            // Cache relevant locations for this session to ease the workload of small location pools
            List<string> locationNames = ArchipelagoHelper.GetAllLocationNames();
            RelevantLocations = Locations.AllLocations
                .Where(lc => locationNames.Contains(lc.LocationName))
                .ToList();

            // Get un-checked shop checks
            List<long> uncheckedShopChecks = ArchipelagoHelper.GetUncheckedLocationsByName("Shopsanity: Item");
            if (uncheckedShopChecks.Count > 0)
            {
                // Scout and store un-checked shop checks
                RemainingShopChecks = await ArchipelagoHelper.ScoutLocationsByIdAsync(uncheckedShopChecks.ToArray());
                RemainingShopChecks = RemainingShopChecks.OrderBy((kvp) => kvp.Value.LocationName).ToDictionary(x => x.Key, x => x.Value);
            }

            // Manually flush the updated locations backlog
            LocationCheckHelper checkHelper = ArchipelagoHelper.GetLocationChecksHelper();
            if (checkHelper != null)
            {
                ArchipelagoHelper_OnCheckedLocationsUpdated(checkHelper.AllLocationsChecked);
            }

            // Subscribe to further location check updates
            ArchipelagoHelper.OnCheckedLocationsUpdated += ArchipelagoHelper_OnCheckedLocationsUpdated;

            // Get persisted handled items
            HandledItems = await ArchipelagoHelper.GetValue<Dictionary<string, int>>("handled_items") ?? new();

            // Manually drain the received items backlog
            ReceivedItemsHelper rih = ArchipelagoHelper.GetItemsHelper();
            if (rih != null)
            {
                ArchipelagoHelper_OnItemsReceived(rih);
            }

            // Subscribe to further item received updates
            ArchipelagoHelper.OnItemsReceived += ArchipelagoHelper_OnItemsReceived;
            ArchipelagoHelper.OnDeathlink += ArchipelagoHelper_OnDeathlink;
        }

        private void ArchipelagoHelper_OnDisconnected(string reason)
        {
            // Unsubscribe from event handlers
            ArchipelagoHelper.OnCheckedLocationsUpdated -= ArchipelagoHelper_OnCheckedLocationsUpdated;
            ArchipelagoHelper.OnDeathlink -= ArchipelagoHelper_OnDeathlink;
            ArchipelagoHelper.OnItemsReceived -= ArchipelagoHelper_OnItemsReceived;

            // Clear tracked items
            HandledItems.Clear();
            PendingItems.Clear();
            ReceivedItems.Clear();

            // Clear unlocked items cache
            UnlockedItemTypeCache.Clear();

            // Cancel notification queue
            NotificationHelper.Instance.Cancel();
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
                Logger.LogMessage($"Received item '{itemInfo.ItemName}' from '{itemInfo.Player}'");

                // Increment in received items
                ReceivedItems[itemInfo.ItemName] = ReceivedItems.GetValueOrDefault(itemInfo.ItemName, 0) + 1;

                // If this is a new item, queue the notification
                bool isUnprocessedItem = ReceivedItems[itemInfo.ItemName] > GetItemProcessedCount(itemInfo.ItemName);
                if (isUnprocessedItem)
                {
                    // If item is unprocessed and not notifi
                    QueueAction(() => Notify(itemInfo, 2.0f));
                }

                // Attempt to find an action for this item
                if (Items.AllItems.TryGetValue(itemInfo.ItemName, out CuedAction cuedAction))
                {
                    // If new item or its cue is 'None', queue it
                    bool queueItem = cuedAction.Cue is ActionCue.None || isUnprocessedItem;
                    if (queueItem)
                    {
                        // Only track the item name if Encounter action or is newly received
                        bool trackItem = cuedAction.Cue is ActionCue.Encounter || isUnprocessedItem;
                        QueueAction(cuedAction.Action, trackItem ? itemInfo.ItemName : string.Empty);
                    }
                }
                else if (isUnprocessedItem)
                {
                    // If no mapped action, treat as handled if new
                    HandledItems[itemInfo.ItemName] = HandledItems.GetValueOrDefault(itemInfo.ItemName, 0) + 1;
                }

                // Refresh item types cache if item rarity item received
                if (itemInfo.ItemName.Equals("Progressive Item Rarity", StringComparison.OrdinalIgnoreCase))
                {
                    RefreshUnlockedItemTypesCache();
                }
                else
                {
                    KeyValuePair<Type, (string name, ItemRarity rarity)> itemType = ItemTypeCache.FirstOrDefault(kvp => kvp.Value.name.Equals(itemInfo.ItemName, StringComparison.OrdinalIgnoreCase));
                    if (itemType.Key != null && !UnlockedItemTypeCache.Contains(itemType.Key))
                    {
                        Logger.LogDebug($"Adding item type: {itemType.Key} to unlocked item cache");
                        UnlockedItemTypeCache.Add(itemType.Key);
                    }
                }
            }

            // Persist the handled items count
            ArchipelagoHelper.SetValue("handled_items", HandledItems);
        }

        /// <summary>
        /// Event handler for deathlink received from archipelago session.
        /// </summary>
        /// <param name="deathLink">The received deathlink</param>
        private void ArchipelagoHelper_OnDeathlink(DeathLink deathLink)
        {
            QueueAction(() => HandleDeathlink(deathLink));
        }

        /// <summary>
        /// Event handler for a message received from the archipelago server.
        /// </summary>
        /// <param name="logMessage">The received message.</param>
        private void ArchipelagoHelper_OnMessageReceived(Archipelago.MultiClient.Net.MessageLog.Messages.LogMessage logMessage)
        {
            ArchipelagoConsoleHelper.Instance.AddMessage(logMessage);
        }

        #endregion
    }
}

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
        private ConcurrentQueue<IEnumerator> actionQueue = new ConcurrentQueue<IEnumerator>();

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
            Instance = this;

            // Register event handlers
            ArchipelagoHelper.OnConnected += ArchipelagoHelper_OnConnected;
            ArchipelagoHelper.OnCheckedLocationsUpdated += ArchipelagoHelper_OnCheckedLocationsUpdated;
            ArchipelagoHelper.OnItemsReceived += ArchipelagoHelper_OnItemsReceived;
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        private void Update()
        {
            if (!IsInGame)
            {
                return;
            }

            // De-queue action and perform it
            if (actionQueue.TryDequeue(out IEnumerator action))
            {
                StartCoroutine(action);
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
                if (ItemMappings.Map.TryGetValue(itemInfo.ItemName, out IEnumerator action))
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
        public void QueueAction(IEnumerator action)
        {
            actionQueue.Enqueue(action);
        }

        /// <summary>
        /// Attempt to check a location by its action and amount.
        /// </summary>
        /// <param name="action">The ID of the action to check.</param>
        /// <param name="amount">The amount to check against.</param>
        public void TryCheckLocations(string action, long amount)
        {
            foreach (LocationCriteria criteria in ItemMappings.Locations.Where(l => l.Check(action, amount)))
            {
                QueueAction(CheckLocation(criteria.LocationName));
            }
        }

        static IEnumerator CheckLocation(string locationName)
        {
            ArchipelagoHelper.CheckLocation(locationName);
            yield break;
        }
    }
}

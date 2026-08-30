using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using BepInEx.Logging;
using Modd;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Helpers
{
    internal static class ArchipelagoHelper
    {
        #region Private Properties

        /// <summary>
        /// Gets or sets the current deathlink service.
        /// </summary>
        private static DeathLinkService DeathlinkService { get; set; }

        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Gets or sets the current session object.
        /// </summary>
        private static ArchipelagoSession Session { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the session is currently connected.
        /// </summary>
        public static bool IsConnected
        {
            get => Session != null && Session.Socket.Connected;
        }

        /// <summary>
        /// Gets the room info for the currently connected slot.
        /// </summary>
        public static RoomInfoPacket RoomInfo { get; private set; }

        /// <summary>
        /// Gets the slot data for the currently connected slot.
        /// </summary>
        public static ArchipelagoSlotData SlotData { get; private set; }

        /// <summary>
        /// Gets the slot for the current connection
        /// </summary>
        public static int Slot
        {
            get => Session != null ? Session.ConnectionInfo.Slot : -1;
        }

        #endregion

        #region Events

        // Forward events for checked locations updated
        public delegate void CheckedLocationsUpdatedEvent(ReadOnlyCollection<long> newCheckedLocations);
        public static event CheckedLocationsUpdatedEvent OnCheckedLocationsUpdated;

        // Forward events for connection established
        public delegate void ConnectedEvent();
        public static event ConnectedEvent OnConnected;

        // Forward events for deathlinks
        public delegate void DeathlinkEvent(DeathLink deathLink);
        public static event DeathlinkEvent OnDeathlink;

        // Forward events for disconnect
        public delegate void DisconnectedEvent(string reason);
        public static event DisconnectedEvent OnDisconnected;

        // Forward events for items received
        public delegate void ItemsReceivedEvent(ReceivedItemsHelper helper);
        public static event ItemsReceivedEvent OnItemsReceived;

        // Forward events for messages received
        public delegate void MessageReceivedEvent(LogMessage logMessage);
        public static event MessageReceivedEvent OnMessageReceived;

        

        #endregion

        static ArchipelagoHelper()
        {
            
        }

        /// <summary>
        /// Perform the login routine for the archipelago login controller dialog.
        /// </summary>
        /// <param name="controller">The controller dialog instance.</param>
        public static IEnumerator LoginRoutine(ArchipelagoLoginController controller)
        {
            Task<bool> connectTask = ConnectAsync(controller.Host, controller.Slot, controller.Password);

            while (!connectTask.IsCompleted)
            {
                yield return null;
            }

            if (connectTask.IsFaulted)
            {
                // If faulted, something went unexpectedly wrong
                controller.SetState(DialogState.Error, "An unexpected error occurred");
            }
            else if (connectTask.Result)
            {
                // If task succeeded, close
                controller.Close();
            }
            else
            {
                // If failed, 
                controller.SetState(DialogState.Error, "Failed to connect to archipelago session");
            }
        }

        /// <summary>
        /// Connect to an archipelago host.
        /// </summary>
        /// <param name="host">The host URL of the archipelago session.</param>
        /// <param name="slotName">The slot name to connect as.</param>
        /// <param name="pwd">The (optional) password to connect with.</param>
        /// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
        public static async Task<bool> ConnectAsync(string host, string slotName, string pwd = "")
        {
            try
            {
                Logger.LogDebug($"Creating archipelago session...");

                // Create session
                Session = ArchipelagoSessionFactory.CreateSession(host);
                if (Session is null)
                {
                    throw new Exception("Archipelago session unexpectedly returned null.");
                }

                // Register event handlers
                Session.Locations.CheckedLocationsUpdated += Locations_CheckedLocationsUpdated;
                Session.MessageLog.OnMessageReceived += MessageLog_OnMessageReceived;
                Session.Items.ItemReceived += Items_ItemReceived;
                Session.Socket.SocketClosed += Socket_SocketClosed;

                Logger.LogDebug($"Attempting to connect to {host}...");

                // Get room info
                RoomInfo = await Session.ConnectAsync();
                if (RoomInfo is null)
                {
                    throw new Exception("RoomInfo packet unexpectedly returned null.");
                }

                Logger.LogDebug($"Connection established, attempting to log in as '{slotName}'...");

                // Attempt to log in
                LoginResult result = await Session.LoginAsync(
                    "Cursed Words",
                    slotName,
                    ItemsHandlingFlags.AllItems,
                    password: pwd,
                    requestSlotData: true);

                if (result is null)
                {
                    throw new Exception("LoginResult unexpectedly returned null.");
                }
                else if (result is LoginFailure failure)
                {
                    throw new Exception(string.Join(", ", failure.Errors));
                }

                Logger.LogDebug($"Log-in attempt successful, retrieving Slot Data...");

                // Get slot data
                SlotData = ArchipelagoSlotData.Parse(Session.DataStorage.GetSlotData());
                if (SlotData is null)
                {
                    throw new Exception("SlotData was unexpectedly null.");
                }

                Logger.LogDebug($"Creating Deathlink service...");

                // Create deathlink service
                DeathlinkService = Session.CreateDeathLinkService();
                if (DeathlinkService is null)
                {
                    Logger.LogWarning("Failed to create deathlinks service - skipping...");
                }
                else
                {
                    DeathlinkService.OnDeathLinkReceived += DeathlinkService_OnDeathLinkReceived;

                    // Enable or disable depending on slot data
                    if (SlotData.Deathlink)
                    {
                        DeathlinkService.EnableDeathLink();
                    }
                    else
                    {
                        DeathlinkService.DisableDeathLink();
                    }
                }

                Logger.LogDebug($"Connection set-up complete!");

                // Trigger connected event
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to connect to archipelago session. Reason: {ex.Message}");

                await DisconnectAsync();

                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnect from the archipelago host.
        /// </summary>
        public static async Task DisconnectAsync()
        {
            if (Session != null)
            {
                await Session.Socket.DisconnectAsync();
            }
            else
            {
                Cleanup();
            }
        }

        /// <summary>
        /// Get the amount of times a specific item has been received.
        /// </summary>
        /// <param name="itemName">The item name to check.</param>
        /// <returns>The amount of times the item name has been received or -1 if not connected.</returns>
        public static int AmountOfItemReceived(string itemName)
        {
            if (!IsConnected)
                return -1;

            return Session.Items.AllItemsReceived.Count(item => item.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get the amount of locations that have been checked.
        /// </summary>
        /// <returns>The amount of currently checked locations.</returns>
        public static int GetCheckedLocationsCount()
        {
            if (!IsConnected)
                return 0;

            return Session.Locations.AllLocationsChecked.Count;
        }

        /// <summary>
        /// Get the names of all locations for the current connection.
        /// </summary>
        /// <returns>A list of all locations for the currently connected slot.</returns>
        public static List<string> GetAllLocationNames()
        {
            if (!IsConnected)
                return new List<string>();

            return Session.Locations.AllLocations
                .Select(l => GetLocationName(l))
                .ToList();
        }

        /// <summary>
        /// Get the slot data for the session.
        /// </summary>
        /// <returns>A populated slot data dictionary or empty if not connected.</returns>
        public static Dictionary<string, object> GetSlotData()
        {
            if (!IsConnected)
                return new Dictionary<string, object>();

            return Session.DataStorage.GetSlotData();
        }

        /// <summary>
        /// Get the received items helper.
        /// </summary>
        /// <returns>The received items helper or null if not connected.</returns>
        public static ReceivedItemsHelper GetItemsHelper()
        {
            if (!IsConnected)
                return null;

            return Session.Items as ReceivedItemsHelper;
        }

        /// <summary>
        /// Get the location check helper.
        /// </summary>
        /// <returns>The location check helper or null if not connected.</returns>
        public static LocationCheckHelper GetLocationChecksHelper()
        {
            if (!IsConnected)
                return null;

            return Session.Locations as LocationCheckHelper;
        }

        /// <summary>
        /// Get the name of a location from its location ID
        /// </summary>
        /// <param name="locationId">The ID of the location.</param>
        /// <param name="gameName">The name of the game the location belongs to.</param>
        public static string GetLocationName(long locationId, string gameName = "Cursed Words")
        {
            if (!IsConnected)
                return string.Empty;

            return Session.Locations.GetLocationNameFromId(locationId, gameName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public static List<long> GetUncheckedLocationsByName(string searchTerm, string gameName = "Cursed Words")
        {
            if (!IsConnected)
                return new List<long>();

            return Session.Locations.AllMissingLocations
                .Where(l => Session.Locations.GetLocationNameFromId(l, gameName).Contains(searchTerm))
                .ToList();
        }

        /// <summary>
        /// Get the amount of locations that have not been checked.
        /// </summary>
        /// <returns>The amount of currently un-checked locations.</returns>
        public static int GetUncheckedLocationsCount()
        {
            if (!IsConnected)
                return 0;

            return Session.Locations.AllMissingLocations.Count;
        }

        /// <summary>
        /// Set a value in the data storage.
        /// </summary>
        /// <param name="key">The key name to store under.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="scope">The scope in which to store the value.</param>
        /// <returns>True if success, False if not.</returns>
        public static async Task<T> GetValue<T>(string key, Scope scope = Scope.Slot)
        {
            if (!IsConnected)
                return default(T);

            try
            {
                JToken value = await Session.DataStorage[scope, key].GetAsync();
                return value.ToObject<T>();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to retrieve {key} from data storage. Reason: {ex.Message}");
            }

            return default(T);
        }

        /// <summary>
        /// Check if an item has been received X amount of times by its name.
        /// </summary>
        /// <param name="itemName">The name of the item to check.</param>
        /// <param name="amount">The amount of times received to check.</param>
        /// <returns>True if received greater than or equal to the amount, otherwise False.</returns>
        public static bool HasReceivedItem(string itemName, int amount = 1)
        {
            if (!IsConnected)
                return false;

            return Session.Items.AllItemsReceived.Count(item => item.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase)) >= amount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static async Task<Dictionary<long, ScoutedItemInfo>> ScoutLocationsByIdAsync(params long[] ids)
        {
            if (!IsConnected)
                return new Dictionary<long, ScoutedItemInfo>();

            return await Session.Locations.ScoutLocationsAsync(false, ids);
        }

        /// <summary>
        /// Set a value in the data storage.
        /// </summary>
        /// <param name="key">The key name to store under.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="scope">The scope in which to store the value.</param>
        /// <returns>True if success, False if not.</returns>
        public static bool SetValue<T>(string key, T value, Scope scope = Scope.Slot)
        {
            if (!IsConnected)
                return false;

            try
            {
                Session.DataStorage[scope, key] = JObject.FromObject(value);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to store {key}: {value} in data storage. {ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempt to set the player as goal achieved.
        /// </summary>
        public static void TryGoal()
        {
            if (!IsConnected)
                return;

            Session.SetGoalAchieved();
        }

        /// <summary>
        /// Attempt to set the player state.
        /// </summary>
        /// <param name="state">The state to attempt to set.</param>
        public static void TrySetPlayerStatus(ArchipelagoClientState state)
        {
            if (!IsConnected)
                return;

            Session.SetClientState(state);
        }

        /// <summary>
        /// Attempt to send a deathlink
        /// </summary>
        /// <param name="cause">The cause of the death.</param>
        public static void TrySendDeathlink(string cause)
        {
            if (!IsConnected || DeathlinkService is null || !SlotData.Deathlink)
                return;

            DeathlinkService.SendDeathLink(new DeathLink(Session.Players.ActivePlayer.Name, cause));
        }

        /// <summary>
        /// Attempt to mark a location as checked..
        /// </summary>
        /// <param name="locationName">The name of the location.</param>
        /// <param name="gameName">The game that the check belongs to.</param>
        public static void TryCheckLocation(string locationName, string gameName = "Cursed Words")
        {
            Logger.LogDebug($"Attempting to check location: '{locationName}'...");

            if (!IsConnected)
                return;

            long locationId = Session.Locations.GetLocationIdFromName(gameName, locationName);
            if (locationId > -1 && !Session.Locations.AllLocationsChecked.Contains(locationId))
            {
                Logger.LogMessage($"Checking location  '{locationName}'...");
                Session.Locations.CompleteLocationChecks(new long[] { locationId });
            }
        }

        public static void TrySendMessage(string message)
        {
            Logger.LogDebug($"Attempting to send message: '{message}'...");

            if (!IsConnected)
                return;

            // Send say packet
            Session.Say(message);
        }

        private static void Cleanup()
        {
            if (Session != null)
            {
                // Un-register event handlers
                Session.Locations.CheckedLocationsUpdated -= Locations_CheckedLocationsUpdated;
                Session.Items.ItemReceived -= Items_ItemReceived;
                Session.Socket.SocketClosed -= Socket_SocketClosed;
            }

            DeathlinkService = null;
            RoomInfo = null;
            Session = null;
            SlotData = null;
        }

        #region Event Handlers

        private static void DeathlinkService_OnDeathLinkReceived(DeathLink deathLink) => OnDeathlink?.Invoke(deathLink);

        private static void Items_ItemReceived(ReceivedItemsHelper helper) => OnItemsReceived?.Invoke(helper);
        
        private static void Locations_CheckedLocationsUpdated(ReadOnlyCollection<long> newCheckedLocations) => OnCheckedLocationsUpdated?.Invoke(newCheckedLocations);

        private static void MessageLog_OnMessageReceived(Archipelago.MultiClient.Net.MessageLog.Messages.LogMessage message) => OnMessageReceived?.Invoke(message);

        private static void Socket_SocketClosed(string reason)
        {
            Cleanup();
            OnDisconnected?.Invoke(reason);
        }

        #endregion
    }
}

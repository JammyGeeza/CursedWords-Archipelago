using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Helpers
{
    internal static class ArchipelagoHelper
    {
        private static ArchipelagoSession Session = null;
        public static RoomInfoPacket RoomInfo = null;

        // Forwardevents for connection established
        public delegate void ConnectedEvent();
        public static event ConnectedEvent OnConnected;

        // Forward events for items received
        public delegate void ItemsReceivedEvent(ReceivedItemsHelper helper);
        public static event ItemsReceivedEvent OnItemsReceived;

        // Forward events for checked locations updated
        public delegate void CheckedLocationsUpdatedEvent(ReadOnlyCollection<long> newCheckedLocations);
        public static event CheckedLocationsUpdatedEvent OnCheckedLocationsUpdated;

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
                Debug.Log($"Creating archipelago session...");

                // Create session
                Session = ArchipelagoSessionFactory.CreateSession(host);
                if (Session is null)
                {
                    throw new Exception("Archipelago session unexpectedly returned null.");
                }
                
                // Register event handlers
                Session.Locations.CheckedLocationsUpdated += Locations_CheckedLocationsUpdated;
                Session.Items.ItemReceived += Items_ItemReceived;

                Debug.Log($"Attempting to connect to archipelago session...");

                // Get room info
                RoomInfo = await Session.ConnectAsync();
                if (RoomInfo is null)
                {
                    throw new Exception("RoomInfo packet unexpectedly returned null.");
                }

                Debug.Log($"Attempting to log in to archipelago session...");

                // Attempt to log in
                LoginResult result = await Session.LoginAsync(
                    "Cursed Words",
                    slotName,
                    Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags.AllItems,
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

                Debug.Log($"Successfully connected to archipelago session!");

                // Trigger connected event
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect to archipelago session. Reason: {ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnect from the archipelago host.
        /// </summary>
        public static async void DisconnectAsync()
        {
            // Ignore if not already connected
            if (Session is null || !Session.Socket.Connected)
                return;

            await Session.Socket.DisconnectAsync();

            // Cleanup
            Session = null;
            RoomInfo = null;
        }

        /// <summary>
        /// Get the name of a location from its location ID
        /// </summary>
        /// <param name="locationId">The ID of the location.</param>
        /// <param name="gameName">The name of the game the location belongs to.</param>
        public static string GetLocationName(long locationId, string gameName = "Cursed Words")
        {
            return Session.Locations.GetLocationNameFromId(locationId, gameName);
        }

        public static void CheckLocation(string locationName, string gameName = "Cursed Words")
        {
            long locationId = Session.Locations.GetLocationIdFromName(gameName, locationName);
            if (locationId > -1 && !Session.Locations.AllLocationsChecked.Contains(locationId))
            {
                Session.Locations.CompleteLocationChecks(new long[] { locationId });
            }
        }

        #region Event Handlers

        private static void Items_ItemReceived(ReceivedItemsHelper helper) => OnItemsReceived?.Invoke(helper);
        private static void Locations_CheckedLocationsUpdated(ReadOnlyCollection<long> newCheckedLocations) => OnCheckedLocationsUpdated?.Invoke(newCheckedLocations);

        #endregion
    }
}

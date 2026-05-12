using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(TransitionController))]
    internal class TransitionController_Patches : PatchBase
    {
        /// <summary>
        /// When navigating to save selection, disconnect from archipelago.
        /// </summary>
        [HarmonyPatch(nameof(TransitionController.TransitionToNewScene))]
        [HarmonyPostfix]
        private static async void TransitionToNewScene_Postfix(string sceneString)
        {
            Logger.LogInfo($"{nameof(TransitionController)}.{nameof(TransitionController.TransitionToNewScene)} postfix!");

            if (sceneString.Equals(SceneNames.SaveSlotsScene))
            {
                // Disconnect
                await ArchipelagoHelper.DisconnectAsync();

                CursedWordsArchipelago.Instance.IsInGame = false;
            }
            else
            {
                CursedWordsArchipelago.Instance.IsInGame = true;

                // Set player status
                if (sceneString.Equals(SceneNames.EncounterSceneName))
                {
                    Logger.LogInfo("Setting player status to playing");
                    ArchipelagoHelper.TrySetPlayerStatus(ArchipelagoClientState.ClientPlaying);
                }
                else if (sceneString.Equals(SceneNames.MainMenuSceneName))
                {
                    Logger.LogInfo("Setting player status to connected");
                    ArchipelagoHelper.TrySetPlayerStatus(ArchipelagoClientState.ClientConnected);
                }
            }

            Logger.LogInfo($"Is in game: {CursedWordsArchipelago.Instance.IsInGame}");
        }
    }
}

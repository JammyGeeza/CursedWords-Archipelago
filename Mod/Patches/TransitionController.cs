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
    internal class TransitionController_Patches
    {
        /// <summary>
        /// When navigating to save selection, disconnect from archipelago.
        /// </summary>
        /// <param name="value"></param>
        [HarmonyPatch(nameof(TransitionController.TransitionToNewScene))]
        [HarmonyPostfix]
        private static void TransitionToNewScene_Postfix(string sceneString)
        {
            Debug.Log($"TransitionController.TransitionToNewScene Postfix!");

            if (sceneString.Equals(SceneNames.SaveSlotsScene))
            {
                ArchipelagoHelper.DisconnectAsync();

                CursedWordsArchipelago.Instance.IsInGame = false;
            }
            else
            {
                CursedWordsArchipelago.Instance.IsInGame = true;
            }
        }
    }
}

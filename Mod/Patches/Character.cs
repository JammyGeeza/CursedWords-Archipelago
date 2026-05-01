using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal class Character_Patches
    {
        /// <summary>
        /// Override unlock criteria text.
        /// </summary>
        [HarmonyPatch(nameof(Character.GetUnlockRequirementText))]
        [HarmonyPrefix]
        private static bool GetUnlockRequirementText_Prefix(Character __instance, ref string __result)
        {
            __result = $"Receive the '{__instance.GetName()}' archipelago item to unlock this character";
            return false;
        }
    }
}

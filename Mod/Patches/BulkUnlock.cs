using BepInEx.Logging;
using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(BulkUnlock))]
    internal class BulkUnlock_Patches : PatchBase
    {
        /// <summary>
        /// Override bulk unlocks and prevent them entirely.
        /// </summary>
        [HarmonyPatch(nameof(BulkUnlock.Unlock))]
        [HarmonyPrefix]
        public static bool OnUnlock_Prefix(BulkUnlock __instance)
        {
            Logger.LogInfo($"{nameof(BulkUnlock)}.{nameof(BulkUnlock.Unlock)} prefix!");

            //bool result = Lookups.ValidBulkUnlockTypes.Contains(__instance.GetType());

            //if (!result)
            //{
            //    Logger.LogInfo($"Ignoring bulk unlock of type {__instance.Name}");
            //}

            //return result;

            return false;
        }
    }
}

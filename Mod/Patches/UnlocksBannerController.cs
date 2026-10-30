using HarmonyLib;
using System;
using System.Collections;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(UnlocksBannerController))]
    internal class UnlocksBannerController_Patches : PatchBase
    {
        /// <summary>
        /// Replace banner IEnumerator with an empty enumerator so it skips the banner entirely.
        /// </summary>
        [HarmonyPatch(nameof(UnlocksBannerController.Populate))]
        [HarmonyPrefix]
        private static bool OnPopulate_Prefix(ref IEnumerator __result)
        {
            Logger.LogDebug("UnlocksBannerController.Populate prefix!");

            __result = Array.Empty<object>().GetEnumerator();
            return false;
        }
    }
}

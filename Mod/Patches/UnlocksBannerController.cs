using HarmonyLib;
using System;
using System.Collections;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(UnlocksBannerController))]
    internal class UnlocksBannerController_Patches : PatchBase
    {
        /// <summary>
        /// Ensure that Supervillain can always spawn all cursed types.
        /// </summary>
        [HarmonyPatch(typeof(UnlocksBannerController), nameof(UnlocksBannerController.Populate))]
        static class UnlocksBannerController_Patches_SkipAllBanners
        {
            /// <summary>
            /// Replace banner IEnumerator with an empty enumerator so it skips it entirely.
            /// </summary>
            static bool Prefix(ref IEnumerator __result)
            {
                __result = Array.Empty<object>().GetEnumerator();
                return false;
            }
        }
    }
}

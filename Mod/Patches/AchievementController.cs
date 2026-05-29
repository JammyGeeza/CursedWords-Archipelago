using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Achievements))]
    internal class Achievements_Patches : PatchBase
    {
        /// <summary>
        /// Need this as both public and private static methods cause ambiguous match error, so targeting private.
        /// </summary>
        [HarmonyPatch]
        internal class UnlockAchievement_Patch
        {
            static MethodBase TargetMethod() =>
                AccessTools.Method(typeof(Achievements), "UnlockAchievement", new Type[] { typeof(Achievement), typeof(bool) });

            [HarmonyPrefix]
            private static bool OnUnlockAchievement_Prefix(Achievement achievement, bool isBypassingViz, ref bool __result)
            {
                Logger.LogInfo($"{nameof(Achievements)}.{nameof(Achievements.UnlockAchievement)} prefix!");

                __result = false;

                return false;
            }
        }
    }
}

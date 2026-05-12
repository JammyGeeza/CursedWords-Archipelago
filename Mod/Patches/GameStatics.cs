using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(GameStatics))]
    internal class GameStatics_Patches : PatchBase
    {
        [HarmonyPatch(nameof(GameStatics.GetNumberOfStages))]
        [HarmonyPrefix]
        private static bool OnGetNumberOfStages_Prefix(ref int __result)
        {
            // Set all characters to 5 stages
            __result = 5;

            return false;
        }
    }
}

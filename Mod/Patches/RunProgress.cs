using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(RunProgress))]
    internal class RunProgress_Patches : PatchBase
    {
        /// <summary>
        /// Insert Normal tile as available colour for shop generation.
        /// </summary>
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPostfix]
        public static void Constructor_Postfix(ref RunProgress __instance)
        {
            Logger.LogInfo($"{nameof(RunProgress)}.Constructor postfix!");

            // Add normal colour 
            __instance.AvailableColours.Add(TileType.Normal);
        }
    }
}

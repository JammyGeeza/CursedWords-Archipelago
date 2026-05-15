using HarmonyLib;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(GridLayoutController))]
    internal class GridLayoutController_Patches : PatchBase
    {
        /// <summary>
        /// When a consumable tile is applied, attempt to check the location.
        /// </summary>
        [HarmonyPatch(nameof(GridLayoutController.ApplyConsumableTile))]
        [HarmonyPostfix]
        public static void OnApplyConsumableTile_Postfix(GridLayoutController __instance)
        {
            Logger.LogInfo($"{nameof(GridLayoutController)}.{nameof(GridLayoutController.ApplyConsumableTile)} postfix!");

            // Check the 'Use a Consumable Tile' location
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("place_tile");
        }
    }
}

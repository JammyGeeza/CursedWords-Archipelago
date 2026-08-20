using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Item))]
    internal class Item_Patches : PatchBase
    {
        /// <summary>
        /// Try to check location when item is upgraded.
        /// </summary>
        [HarmonyPatch(nameof(Item.Upgrade))]
        [HarmonyPostfix]
        private static void Upgrade_Postfix(Item __instance, int componentIndex, bool isUpgradingBoth)
        {
            Logger.LogInfo($"{nameof(Item)}.{nameof(Item.Upgrade)} postfix!");

            // If not a pin, attempt to check item upgrade location(s)
            // Pins are handled in BossRewardController_Patches, as the PinDraftController pre-emptively upgrades pins to display them.
            if (!__instance.IsPin())
            {
                // TODO: Need to remove this - items are upgraded for display in the shop which causes this to trigger too early
                CursedWordsArchipelago.Instance.TryCheckItemActionLocations("upgrade", __instance);
            }
        }
    }
}

using HarmonyLib;
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

            Logger.LogInfo($"Item '{__instance.Name}' has been upgraded {__instance.TimesUpgraded} times");
            CursedWordsArchipelago.Instance.TryCheckShopActionLocations("upgrade_item", __instance);
        }
    }
}

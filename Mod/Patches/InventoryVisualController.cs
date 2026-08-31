using HarmonyLib;
using Mod.Classes;
using Modd;
using System;
using System.Reflection;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(InventoryVisualController))]
    internal class InventoryVisualController_Patches : PatchBase
    {
        /// <summary>
        /// If build-biased stamp doesn't generate, generate a random stamp instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(InventoryVisualController.OnItemSellButtonClicked))]
        [HarmonyPrefix]
        private static void OnItemSellButtonClicked_Prefix(InventoryVisualController __instance)
        {
            Logger.LogDebug($"{nameof(InventoryVisualController)}.{nameof(InventoryVisualController.OnItemSellButtonClicked)} prefix!");

            // Send check for selling item
            Item inspectedItem = __instance.GetInspectedItem();
            CursedWordsArchipelago.Instance.TryCheckItemActionLocations($"sell", inspectedItem);
        }

        /// <summary>
        /// If build-biased stamp doesn't generate, generate a random stamp instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(InventoryVisualController.OnTileDestroyButtonClicked))]
        [HarmonyPrefix]
        private static void OnTileDestroyButtonClicked_Prefix(InventoryVisualController __instance)
        {
            Logger.LogDebug($"{nameof(InventoryVisualController)}.{nameof(InventoryVisualController.OnItemSellButtonClicked)} prefix!");

            // Send check for destroying tile
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("destroy_tile");
        }

        /// <summary>
        /// When selling an item, ensure the 'Unicorn' item ignores AP Padlocks.
        /// </summary>
        [HarmonyPatch]
        private static class Unicorn_OnSell_Patch
        {
            static MethodBase TargetMethod()
            {
                Type displayClassType = AccessTools.Inner(typeof(InventoryVisualController), "<>c");
                return AccessTools.Method(displayClassType, "<OnItemSellButtonClicked>b__80_7");
            }

            [HarmonyPostfix]
            private static void Unicorn_OnSell_Postfix(Item sticker, ref bool __result)
            {
                if (sticker is APStampPadlock or APStickerPadlock)
                {
                    __result = false;
                }
            }
        }
    }
}

using HarmonyLib;
using Modd;

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
            Logger.LogInfo($"{nameof(InventoryVisualController)}.{nameof(InventoryVisualController.OnItemSellButtonClicked)} prefix!");

            // Send check for selling item
            Item inspectedItem = __instance.GetInspectedItem();
            CursedWordsArchipelago.Instance.TryCheckGenericLocations($"sell_{(inspectedItem.IsStamp() ? "stamp" : "sticker")}");
        }

        /// <summary>
        /// If build-biased stamp doesn't generate, generate a random stamp instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(InventoryVisualController.OnTileDestroyButtonClicked))]
        [HarmonyPrefix]
        private static void OnTileDestroyButtonClicked_Prefix(InventoryVisualController __instance)
        {
            Logger.LogInfo($"{nameof(InventoryVisualController)}.{nameof(InventoryVisualController.OnItemSellButtonClicked)} prefix!");

            // Send check for destroying tile
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("destroy_tile");
        }
    }
}

using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ItemPools))]
    internal class ItemPools_Patches : PatchBase
    {
        /// <summary>
        /// If build-biased stamp doesn't generate, generate a random stamp instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomBuildBiasedStamp), typeof(List<Type>))]
        [HarmonyPostfix]
        private static void OnGetRandomBuildBiasedStamp_Postfix(List<Type> unavailableItemTypes, ref Item __result)
        {
            Logger.LogInfo($"{nameof(ItemPools)}.{nameof(ItemPools.GetRandomBuildBiasedStamp)} postfix!");

            if (__result == null)
            {
                __result = ItemPools.GetRandomStamp(unavailableItemTypes);
            }

            Logger.LogWarning($"Generated stamp: {__result?.Name}");
        }

        /// <summary>
        /// If build-biased stamp doesn't generate, generate a random stamp instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomBuildBiasedStamp), typeof(List<Type>), typeof(ItemRarity))]
        [HarmonyPostfix]
        private static void OnGetRandomBuildBiasedStamp_Overload_Postfix(List<Type> unavailableItemTypes, ItemRarity rarity, ref Item __result)
        {
            Logger.LogInfo($"{nameof(ItemPools)}.{nameof(ItemPools.GetRandomBuildBiasedStamp)}_Overload postfix!");

            if (__result == null)
            {
                __result = ItemPools.GetRandomStamp(unavailableItemTypes, rarity);
            }

            Logger.LogWarning($"Generated stamp: {__result?.Name}");
        }

        /// <summary>
        /// If build-biased sticker doesn't generate, generate a random sticker instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomBuildBiasedSticker), typeof(List<Type>))]
        [HarmonyPostfix]
        private static void OnGetRandomBuildBiasedSticker_Postfix(List<Type> unavailableItemTypes, ref Item __result)
        {
            Logger.LogInfo($"{nameof(ItemPools)}.{nameof(ItemPools.GetRandomBuildBiasedSticker)} postfix!");

            if (__result == null)
            {
                __result = ItemPools.GetRandomSticker(unavailableItemTypes);
            }

            Logger.LogWarning($"Generated sticker: {__result?.Name}");
        }

        /// <summary>
        /// If build-biased sticker doesn't generate, generate a random sticker instead to fill the gap.
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomBuildBiasedSticker), typeof(ItemRarity), typeof(List<Type>))]
        [HarmonyPostfix]
        private static void OnGetRandomBuildBiasedSticker_Overload_Postfix(ItemRarity rarity, List<Type> unavailableItemTypes, ref Item __result)
        {
            Logger.LogInfo($"{nameof(ItemPools)}.{nameof(ItemPools.GetRandomBuildBiasedSticker)}_Overload postfix!");

            if (__result == null)
            {
                __result = ItemPools.GetRandomSticker(unavailableItemTypes, rarity);
            }

            Logger.LogWarning($"Generated sticker: {__result?.Name}");
        }
    }
}

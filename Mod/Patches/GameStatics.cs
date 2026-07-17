using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(GameStatics))]
    internal class GameStatics_Patches : PatchBase
    {
        /// <summary>
        /// Override items requiring unlock with items not yet received from the multiworld.
        /// </summary>
        [HarmonyPatch(nameof(GameStatics.GetItemsRequiringUnlock))]
        [HarmonyPrefix]
        private static bool OnGetItemsRequiringUnlock_Prefix(ref List<Type> __result)
        {
            Logger.LogInfo($"{nameof(SaveManager)}.{nameof(GameStatics.GetItemsRequiringUnlock)} prefix!");

            __result = CursedWordsArchipelago.Instance.ItemTypeCache
                .Where(kvp =>
                {
                    // Check if item has been received
                    if (ArchipelagoHelper.HasReceivedItem(kvp.Value.name))
                    {
                        // Check if item rarity should also be received
                        if (ArchipelagoHelper.SlotData.ShuffleItemRarities)
                        {
                            switch (kvp.Value.rarity)
                            {
                                case ItemRarity.Rare:
                                    return !ArchipelagoHelper.HasReceivedItem("Progressive Item Rarity", 1);

                                case ItemRarity.Legendary:
                                    return !ArchipelagoHelper.HasReceivedItem("Progressive Item Rarity", 2);

                                default:
                                    return false;
                            }
                        }

                        return false;
                    }

                    return true;
                })
                //.Where(kvp => !ArchipelagoHelper.HasReceivedItem(kvp.Value.name))
                .Select(kvp => kvp.Key)
                .ToList();

            return false;
        }

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

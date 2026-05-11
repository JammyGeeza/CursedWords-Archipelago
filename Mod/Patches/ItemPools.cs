using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ItemPools))]
    internal class ItemPools_Patches
    {
        /// <summary>
        /// Prevent Stamps from appearing in the item pool if not yet received
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomStamp), typeof(List<Type>))]
        [HarmonyPrefix]
        private static void GetRandomStamp_Prefix(ref List<Type> blacklist)
        {
            Debug.Log($"{nameof(ItemPools)}.{nameof(ItemPools.GetRandomStamp)} postfix!");

            if (!ArchipelagoHelper.HasReceivedItem("Stamp Bundle - Blue Tiles"))
            {
                blacklist.AddRange(Lookups.BlueStamps);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Stamp Bundle - Number Tiles"))
            {
                blacklist.AddRange(Lookups.NumberStamps);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Stamp Bundle - Red Tiles"))
            {
                blacklist.AddRange(Lookups.RedStamps);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Stamp Bundle - Shiny Tiles"))
            {
                blacklist.AddRange(Lookups.ShinyStamps);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Stamp Bundle - Void Tiles"))
            {
                blacklist.AddRange(Lookups.VoidStamps);
            }
        }

        /// <summary>
        /// Prevent Stamps from appearing in the item pool if not yet received
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomStamp), typeof(List<Type>), typeof(ItemRarity))]
        [HarmonyPrefix]
        private static void GetRandomStamp_Overload_Prefix(ref List<Type> blacklist)
        {
            GetRandomStamp_Prefix(ref blacklist);
        }

        /// <summary>
        /// Prevent Stamps from appearing in the item pool if not yet received
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomBuildBiasedStamp), typeof(List<Type>))]
        [HarmonyPrefix]
        private static void GetRandomBuildBiasedStamp_Prefix(ref List<Type> unavailableItemTypes)
        {
            GetRandomStamp_Prefix(ref unavailableItemTypes);
        }

        /// <summary>
        /// Prevent Stickers from appearing in the item pool if not yet received
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomSticker), typeof(List<Type>))]
        [HarmonyPrefix]
        private static void GetRandomSticker_Prefix(ref List<Type> blacklist)
        {
            Debug.Log($"{nameof(ItemPools)}.{nameof(ItemPools.GetRandomSticker)} postfix!");

            if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Blue Tiles"))
            {
                blacklist.AddRange(Lookups.BlueStickers);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Number Tiles"))
            {
                blacklist.AddRange(Lookups.NumberStickers);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Red Tiles"))
            {
                blacklist.AddRange(Lookups.RedStickers);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Shiny Tiles"))
            {
                blacklist.AddRange(Lookups.ShinyStickers);
            }

            if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Void Tiles"))
            {
                blacklist.AddRange(Lookups.VoidStickers);
            }
        }

        /// <summary>
        /// Prevent Stickers from appearing in the item pool if not yet received
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomSticker), typeof(List<Type>), typeof(ItemRarity))]
        [HarmonyPrefix]
        private static void GetRandomSticker_Overload_Prefix(ref List<Type> blacklist)
        {
            GetRandomSticker_Prefix(ref blacklist);
        }

        /// <summary>
        /// Prevent Stickers from appearing in the item pool if not yet received
        /// </summary>
        [HarmonyPatch(nameof(ItemPools.GetRandomBuildBiasedSticker), typeof(List<Type>))]
        [HarmonyPrefix]
        private static void GetRandomBuildBiasedSticker_Prefix(ref List<Type> unavailableItemTypes)
        {
            GetRandomSticker_Prefix(ref unavailableItemTypes);
        }
    }
}

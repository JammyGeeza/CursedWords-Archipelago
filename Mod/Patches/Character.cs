using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal class Character_Patches : PatchBase
    {
        /// <summary>
        /// Prevent character default build items if not yet unlocked
        /// </summary>
        [HarmonyPatch(nameof(Character.GetCharacterItemTypes))]
        [HarmonyPostfix]
        private static void GetCharacterItemTypes_Postfix(Character __instance, ref List<Type> __result)
        {
            Logger.LogInfo($"{nameof(Character)}.{nameof(Character.GetCharacterItemTypes)} postfix!");

            for (int i = __result.Count; i > 0; i--)
            {
                Type type = __result[i - 1];

                // Get name from item type cache
                if (CursedWordsArchipelago.Instance.ItemTypeCache.TryGetValue(type, out string itemName) && !string.IsNullOrEmpty(itemName))
                {
                    // If not yet received, remove from types
                    if (!ArchipelagoHelper.HasReceivedItem(itemName))
                    {
                        __result.Remove(type);
                    }
                }

                //// If part of blue build and received, skip to next
                //if ((Lookups.BlueBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Blue Stamps")) || (Lookups.BlueBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Blue Stickers")))
                //{
                //    continue;
                //}

                //// If part of card build and received, skip to next
                //if ((Lookups.CardsBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Card Stamps")) || (Lookups.CardsBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Card Stickers")))
                //{
                //    continue;
                //}

                //// If part of chess build and received, skip to next
                //if ((Lookups.ChessBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Chess Stamps")) || (Lookups.ChessBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Chess Stickers")))
                //{
                //    continue;
                //}

                //// If part of chess build and received, skip to next
                //if ((Lookups.CurrencyBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Currency Stamps")) || (Lookups.CurrencyBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Currency Stickers")))
                //{
                //    continue;
                //}

                //// If part of cursed build and received, skip to next
                //if ((Lookups.CurseBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Cursed Stamps")) || (Lookups.CurseBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Cursed Stickers")))
                //{
                //    continue;
                //}

                //// If part of numbers build and received, skip to next
                //if ((Lookups.NumberBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Number Stamps")) || (Lookups.NumberBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Number Stickers")))
                //{
                //    continue;
                //}

                //// If part of reinbow build and received, skip to next
                //if ((Lookups.RainbowBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Rainbow Stamps")) || (Lookups.RainbowBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Rainbow Stickers")))
                //{
                //    continue;
                //}

                //// If part of red build and received, skip to next
                //if ((Lookups.RedBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Red Stamps")) || (Lookups.RedBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Red Stickers")))
                //{
                //    continue;
                //}

                //// If part of scattered build and received, skip to next
                //if ((Lookups.ScatteredBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Scatter Stamps")) || (Lookups.ScatteredBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Scatter Stickers")))
                //{
                //    continue;
                //}

                //// If part of shiny build and received, skip to next
                //if ((Lookups.ShinyBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Shiny Stamps")) || (Lookups.ShinyBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Shiny Stickers")))
                //{
                //    continue;
                //}

                //// If part of void build and received, skip to next
                //if ((Lookups.VoidBuildStamps.Contains(type) && ArchipelagoHelper.HasReceivedItem("Void Stamps")) || (Lookups.VoidBuildStickers.Contains(type) && ArchipelagoHelper.HasReceivedItem("Void Stickers")))
                //{
                //    continue;
                //}

                //// Otherwise, remove it
                //__result.Remove(type);
            }
        }

        /// <summary>
        /// Override unlock criteria text.
        /// </summary>
        [HarmonyPatch(nameof(Character.GetUnlockRequirementText))]
        [HarmonyPrefix]
        private static bool GetUnlockRequirementText_Prefix(Character __instance, ref string __result)
        {
            __result = $"Receive the '{__instance.GetName()}' archipelago item to unlock this character";
            return false;
        }
    }
}

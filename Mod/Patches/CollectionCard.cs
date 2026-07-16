using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(CollectionCard))]
    internal class CollectionCard_Patches : PatchBase
    {
        /// <summary>
        /// Override items requiring unlock with items not yet received from the multiworld.
        /// </summary>
        [HarmonyPatch(nameof(CollectionCard.Populate))]
        [HarmonyPostfix]
        private static void OnPopulate_Postfix(ref CollectionCard __instance, Item item, bool unobserved, bool skipLoad)
        {
            Logger.LogInfo($"{nameof(CollectionCard)}.{nameof(CollectionCard.Populate)} postfix!");

            // If item has not been received, set text
            if (!ArchipelagoHelper.HasReceivedItem(item.Name))
            {
                TextMeshProUGUI lockedTitleTMP = Traverse.Create(__instance)
                .Field("_lockedTitleTMP")
                .GetValue<TextMeshProUGUI>();

                TextMeshProUGUI lockedConditionsTMP = Traverse.Create(__instance)
                .Field("_lockedConditionsTMP")
                .GetValue<TextMeshProUGUI>();

                // Check item type and set text accoringly
                if (item.IsStamp() || item.IsSticker())
                {
                    lockedTitleTMP.SetText(item.Name);
                    lockedConditionsTMP.SetText($"Receive the '{item.Name}' archipelago item to unlock.");
                }
                else if (item.PinColors.Any())
                {
                    string characterName = string.Empty;
                    
                    // Get character associated with pin
                    switch (item)
                    {
                        case CarpStreamers carpStreamers:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(WetDennis), string.Empty);
                            break;

                        case MilkyWay milkyWay:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(NinaNix), string.Empty);
                            break;

                        case Abacus abacus:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(HayleyBayles), string.Empty);
                            break;

                        case SuperEight superEight:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(SamGambit), string.Empty);
                            break;

                        case Bicycle bicycle:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(BonesTheDog), string.Empty);
                            break;

                        case Bucket bucket:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(Octacles), string.Empty);
                            break;

                        case RandomAccessMemory ram:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(NathaServo), string.Empty);
                            break;

                        case MahjongRedDragon mrd:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(SandySaguaro), string.Empty);
                            break;

                        case WadOfCash woc:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(Spike), string.Empty);
                            break;

                        case HumanHands hands:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(SockHead), string.Empty);
                            break;

                        case Rainbow rainbow:
                            characterName = CursedWordsArchipelago.Instance.CharacterTypeCache.GetValueOrDefault(typeof(PrismaticBean), string.Empty);
                            break;

                        default:
                            characterName = "Unknown";
                            break;

                    }

                    lockedTitleTMP.SetText(item.Name);
                    lockedConditionsTMP.SetText($"Receive the '{characterName}' archipelago item to unlock.");
                }

            }
        }
    }
}

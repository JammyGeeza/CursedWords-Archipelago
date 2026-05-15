using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Modd;
using System;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class Player_Patches : PatchBase
    {
        [HarmonyPatch(nameof(Player.ChangeMoney))]
        [HarmonyPostfix]
        private static void OnChangeMoney_Postfix(Player __instance)
        {
            Logger.LogInfo($"{nameof(Player)}.{nameof(Player.ChangeMoney)} postfix!");

            // Attempt to check money location(s)
            CursedWordsArchipelago.Instance.TryCheckNumericLocations("earn_money", __instance.CurrentRunProgress.CurrentRunStatistics.TotalCashEarned);
        }

        [HarmonyPatch(nameof(Player.SetCharacter))]
        [HarmonyPostfix]
        private static void OnSetCharacter_Postfix(Player __instance)
        {
            Logger.LogInfo($"{nameof(Player)}.{nameof(Player.SetCharacter)} postfix!");

            // Add sticker padlocks to fill slots not yet received as items
            int progressiveStickerSlots = ArchipelagoHelper.AmountOfItemReceived("Progressive Sticker Slot");
            Logger.LogInfo($"Received {progressiveStickerSlots} sticker slots");
            for (int i = 0; i < (5 - progressiveStickerSlots); i++)
            {
                Logger.LogInfo($"Adding sticker padlock {i+1}");
                APStickerPadlock stickerPadlock = Activator.CreateInstance<APStickerPadlock>();
                __instance.AddItemToInventory(stickerPadlock);
            }

            // Add stamp padlocks to fill slots not yet received as items
            int progressiveStampSlots = ArchipelagoHelper.AmountOfItemReceived("Progressive Stamp Slot");
            Logger.LogInfo($"Received {progressiveStampSlots} sticker slots");
            for (int i = 0; i < (5 - progressiveStampSlots); i++)
            {
                Logger.LogInfo($"Adding stamp padlock {i+1}");
                APStampPadlock stampPadlock = Activator.CreateInstance<APStampPadlock>();
                __instance.AddItemToInventory(stampPadlock);
            }
        }
    }
}

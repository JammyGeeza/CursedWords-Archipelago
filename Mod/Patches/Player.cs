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
        /// <summary>
        /// When adding an item to inventory, ignore if it's an Archipelago Shop Item.
        /// </summary>
        [HarmonyPatch(nameof(Player.AddItemToInventory))]
        [HarmonyPrefix]
        private static bool OnAddItemToInventory_Prefix(Item item)
        {
            Logger.LogInfo($"{nameof(Player)}.{nameof(Player.AddItemToInventory)} prefix!");

            return !(item is ArchipelagoShopitem);
        }

        /// <summary>
        /// When money is changed, send location checks for total money earned.
        /// </summary>
        [HarmonyPatch(nameof(Player.ChangeMoney))]
        [HarmonyPostfix]
        private static void OnChangeMoney_Postfix(Player __instance)
        {
            Logger.LogInfo($"{nameof(Player)}.{nameof(Player.ChangeMoney)} postfix!");

            // Attempt to check money location(s)
            CursedWordsArchipelago.Instance.TryCheckNumericLocations("earn_money", __instance.CurrentRunProgress.CurrentRunStatistics.TotalCashEarned);
        }

        /// <summary>
        /// When setting up a character, add locked sticker/stamp slots.
        /// </summary>
        [HarmonyPatch(nameof(Player.SetCharacter))]
        [HarmonyPostfix]
        private static void OnSetCharacter_Postfix(Player __instance)
        {
            Logger.LogInfo($"{nameof(Player)}.{nameof(Player.SetCharacter)} postfix!");

            // Ignore if inventory slots not shuffled
            if (!ArchipelagoHelper.SlotData.ShuffleInventorySlots)
                return;

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

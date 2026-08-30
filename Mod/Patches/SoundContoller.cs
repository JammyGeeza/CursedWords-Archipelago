using HarmonyLib;
using Mod.Classes;
using Mod.Extensions;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SoundController))]
    internal class SoundController_Patches : PatchBase
    {
        /// <summary>
        /// Attempt to check location(s) when an item is purchased in the shop.
        /// </summary>
        [HarmonyPatch(nameof(SoundController.BuyItem))]
        [HarmonyPostfix]
        private static void OnBuyitem_Postfix(Item item, bool isUpgradingSticker)
        {
            Logger.LogDebug($"{nameof(SoundController)}.{nameof(SoundController.BuyItem)} postfix!");

            if (item is ArchipelagoShopitem apShopitem)
            {
                // Attempt to check shopsanity location
                CursedWordsArchipelago.Instance.TryCheckLocation(apShopitem.ItemInfo.LocationDisplayName);
            }
            else
            {
                // Attempt to check item location(s)
                CursedWordsArchipelago.Instance.TryCheckItemActionLocations(
                    isUpgradingSticker ? "upgrade" : "buy",
                    item);
            }
        }

        /// <summary>
        /// Attempt to check location(s) when a tile is purchased in the shop.
        /// </summary>
        [HarmonyPatch(nameof(SoundController.BuyTile))]
        [HarmonyPostfix]
        private static void BuyTile(Tile tile)
        {
            Logger.LogDebug($"{nameof(SoundController)}.{nameof(SoundController.BuyTile)} postfix!");
            
            CursedWordsArchipelago.Instance.TryCheckTileLocations("buy", tile);
        }


        /// <summary>
        /// Attempt to check location(s) when a fairy is received.
        /// </summary>
        [HarmonyPatch(nameof(SoundController.FairyGet))]
        [HarmonyPostfix]
        private static void OnFairyGet_Postfix()
        {
            Logger.LogDebug($"{nameof(SoundController)}.{nameof(SoundController.FairyGet)} postfix!");

            Player player = GameStatics.GetPlayer();

            // Attempt to send encounter location for fairy
            CursedWordsArchipelago.Instance.TryCheckEncounterLocations("get_fairy", player);
        }
    }
}

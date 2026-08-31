using Archipelago.MultiClient.Net.Models;
using HarmonyLib;
using Mod.Classes;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ShopItemSlot))]
    internal class ShopItemSlot_Patches : PatchBase
    {
        /// <summary>
        /// Ensure that the 'Buy' button of a Shopsanity item is always available.
        /// </summary>
        [HarmonyPatch(nameof(ShopItemSlot.Populate))]
        [HarmonyPostfix]
        private static void OnPopulate_Postfix(ShopItemSlot __instance, ItemInStock itemInStock, bool isAngelInvestmentAvailable)
        {
            Logger.LogDebug($"{nameof(ShopItemSlot)}.{nameof(ShopItemSlot.Populate)} postfix!");

            // Show 'Buy' button even if player's sticker/stamp slots are full
            if (itemInStock.MyItem is ArchipelagoShopitem archipelagoShopitem)
            {
                // Make sure it's not disabled due to not enough money
                Player player = GameStatics.GetPlayer();
                if (player.Money >= itemInStock.Cost)
                {
                    __instance.StartCoroutine(SetBuyButtonNextFrame(__instance));
                }
            }
        }

        /// <summary>
        /// Set the buy button on the next frame.
        /// </summary>
        /// <param name="shopItemSlot">The shop item slot to set the buy button for.</param>
        private static IEnumerator SetBuyButtonNextFrame(ShopItemSlot shopItemSlot)
        {
            yield return null;
            shopItemSlot.SetBuyButton("BUY", new Color32(byte.MaxValue, 200, 77, byte.MaxValue), new Color32(123, 92, 24, byte.MaxValue), true);
        }
    }
}

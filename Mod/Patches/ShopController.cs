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
    [HarmonyPatch(typeof(ShopController))]
    internal class ShopController_Patches : PatchBase
    {
        [HarmonyPatch("BuyStamp")]
        [HarmonyPostfix]
        private static void OnBuySticker_Postfix(ShopController __instance, ShopItemSlot itemSlot)
        {
            Logger.LogInfo($"{nameof(ShopController)}.BuyStamp postfix!");

            // Ignore if archipelago shop item
            if (itemSlot.MyItemInStock.MyItem is ArchipelagoShopitem archipelagoShopItem)
            {
                // Attempt to check shop item location
                CursedWordsArchipelago.Instance.TryCheckLocation(archipelagoShopItem.ItemInfo.LocationDisplayName);
            }
            else
            {
                // Attempt to check 'buy stamp' location
                CursedWordsArchipelago.Instance.TryCheckGenericLocations("buy_stamp");
            }
        }

        [HarmonyPatch("BuySticker")]
        [HarmonyPostfix]
        private static void OnBuySticker_Postfix(ShopController __instance, ShopItemSlot itemSlot, bool isHippoUpgrade, Item replacementItem)
        {
            // Ignore if archipelago shop item
            if (itemSlot.MyItemInStock.MyItem is ArchipelagoShopitem archipelagoShopItem)
            {
                // Attempt to check shop item location
                CursedWordsArchipelago.Instance.TryCheckLocation(archipelagoShopItem.ItemInfo.LocationDisplayName);
            }
            else
            {
                // Attempt to check 'buy sticker' location
                CursedWordsArchipelago.Instance.TryCheckGenericLocations("buy_sticker");
            }
        }

        /// <summary>
        /// When leaving the shop, check for frozen items and trigge check
        /// </summary>
        [HarmonyPatch(nameof(ShopController.OnLeaveShopButtonClickedCallback))]
        [HarmonyPrefix]
        private static void OnOnLeaveShopButtonClickedCallback_Prefix(ShopController __instance)
        {
            Logger.LogInfo($"{nameof(ShopController)}.{nameof(ShopController.OnLeaveShopButtonClickedCallback)} prefix!");

            // Check if any stickers have been frozen
            if (__instance.GetStickersInStock().Any(s => s != null && s.MyItem.GetType() != typeof(ArchipelagoShopitem) && s.IsFrozen))
            {
                CursedWordsArchipelago.Instance.TryCheckGenericLocations("freeze_sticker");
            }

            // Check if any stamps have been frozen
            if (__instance.GetStampsInStock().Any(s => s != null && s.MyItem.GetType() != typeof(ArchipelagoShopitem) && s.IsFrozen))
            {
                CursedWordsArchipelago.Instance.TryCheckGenericLocations("freeze_stamp");
            }
        }

        /// <summary>
        /// When generating goods in stock, re-populate the item pools so any received sticker/stamp bundles are available.
        /// </summary>
        [HarmonyPatch("GenerateGoodsInStock")]
        [HarmonyPostfix]
        private static IEnumerator OnGenerateGoodsInStock(IEnumerator __result, bool isFirstShop, bool isCascadingAnimations, bool isReroll, bool freeItem)
        {
            Logger.LogInfo($"{nameof(ShopController)}.GenerateGoodsInStock postfix!");

            // If this is a re-roll, attempt to send the check
            if (isReroll)
            {
                CursedWordsArchipelago.Instance.TryCheckGenericLocations("restock_shop");
            }

            // Re-populate item pools
            ItemPools.PopulatePools();

            // Perform existing actions in co-routine
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
        }

        /// <summary>
        /// When generating goods in stock, re-populate the item pools so any received sticker/stamp bundles are available.
        /// </summary>
        [HarmonyPatch("GenerateStampInStock")]
        [HarmonyPrefix]
        private static bool OnGenerateStampInStock_Prefix(ShopController __instance, int index, bool isFirstShop, bool freeItem)
        {
            Logger.LogInfo($"{nameof(ShopController)}.GenerateStampInStock prefix!");

            // Ignore if no shop item checks remain
            int remaining = CursedWordsArchipelago.Instance.RemainingShopStampChecks.Count;
            if (index > 0 || remaining == 0)
            {
                return true;
            }

            // Roll chance to spawn archipelago stamp item (30%, for now)
            int chance = UnityEngine.Random.Range(0, 10);
            if (chance > 10)
            {
                return true;
            }

            try
            {   
                // Randomly select location check
                int selectedCheck = UnityEngine.Random.Range(0, remaining);
                KeyValuePair<long, ScoutedItemInfo> scoutedItem = CursedWordsArchipelago.Instance.RemainingShopStampChecks.ElementAt(selectedCheck);
                
                // Add stamp to stamps in stock
                ItemInStock itemInStock = new ItemInStock(new ArchipelagoShopitem(scoutedItem.Value, false));
                Traverse.Create(__instance)
                    .Method("PopulateStampInStock", itemInStock, index, false, false)
                    .GetValue();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Unable to add archipelago shop item: {ex}");
            }

            return false;
        }

        /// <summary>
        /// When generating goods in stock, re-populate the item pools so any received sticker/stamp bundles are available.
        /// </summary>
        [HarmonyPatch("GenerateStickerInStock")]
        [HarmonyPrefix]
        private static bool OnGenerateStickerInStock_Prefix(ShopController __instance, int index, bool isFirstShop, bool freeItem)
        {
            Logger.LogInfo($"{nameof(ShopController)}.GenerateStickerInStock prefix!");

            // Ignore if no shop item checks remain
            int shopChecksRemaining = CursedWordsArchipelago.Instance.RemainingShopStickerChecks.Count;
            if (index > 0 || shopChecksRemaining == 0)
            {
                return true;
            }

            // Roll chance to spawn shop item (30%, for now)
            int chance = UnityEngine.Random.Range(0, 10);
            if (chance > 10)
            {
                return true;
            }

            try
            {
                // Randomly select location check
                int selectedCheck = UnityEngine.Random.Range(0, shopChecksRemaining);
                KeyValuePair<long, ScoutedItemInfo> scoutedItem = CursedWordsArchipelago.Instance.RemainingShopStickerChecks.ElementAt(selectedCheck);

                // Add sticker to stickers in stock
                ItemInStock itemInStock = new ItemInStock(new ArchipelagoShopitem(scoutedItem.Value));
                __instance.PopulateStickerInStock(itemInStock, index, false, false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Unable to add archipelago shop item: {ex}");
            }

            return false;
        }
    }
}

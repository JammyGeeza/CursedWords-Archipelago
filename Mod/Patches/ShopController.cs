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
        private static List<long> CurrentlyUsedShopLocations = new List<long>();

        private static int ShopCheckChance = 3;

        [HarmonyPatch("OnItemBuyButtonClicked")]
        [HarmonyPrefix]
        private static bool OnItemBuyButtonClicked_Prefix(ShopController __instance, int boughtSlotIndex, bool isStamp)
        {
            Logger.LogInfo($"{nameof(ShopController)}.OnItemBuyButtonClicked postfix!");

            try
            {
                ShopVisualController shopVisualController = __instance.GetShopVisualController();
                ShopItemSlot itemSlot = shopVisualController.GetShopItemSlotFromIndex(boughtSlotIndex, isStamp);
                if (itemSlot.MyItemInStock.MyItem is ArchipelagoShopitem archipelagoShopItem)
                {
                    // Check if player can afford
                    Player player = GameStatics.GetPlayer();
                    if (player.Money > archipelagoShopItem.Cost)
                    {
                        // Play purchase sound
                        PersistentSound.SingletonSoundController.BuyItem(archipelagoShopItem, false);

                        // Subtract money, remove frozen stamp
                        player.ChangeMoney(-archipelagoShopItem.Cost);

                        if (isStamp)
                        {
                            // Remove frozen and remove from stock
                            player.FrozenStamps[__instance.GetStampInStockIndex(itemSlot.MyItemInStock)] = null;
                            __instance.RemoveStampInStock(boughtSlotIndex);
                        }
                        else
                        {
                            player.FrozenStickers[__instance.GetStickerInStockIndex(itemSlot.MyItemInStock)] = null;
                            __instance.RemoveStickerInStock(boughtSlotIndex);
                        }

                        // Stop showing item
                        shopVisualController.ChangeSlotVisibility(itemSlot, isVisible: false);

                        // Update cash and clear inspected item
                        CharacterInfoPanel.SingletonInventoryVisualController.PopulateCash();
                        CharacterInfoPanel.SingletonInventoryVisualController.ClearInspectedItem();

                        // Re-populate shop items
                        shopVisualController.RepopulateShopItems(__instance.GetRerollPrice());

                        // Send shop check
                        CursedWordsArchipelago.Instance.TryCheckLocation(archipelagoShopItem.ItemInfo.LocationDisplayName);
                    }
                    else
                    {
                        PersistentSound.SingletonSoundController.FailedPurchase();
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred when attempting to purchase an archipelago item: {ex}");
                return false;
            }

            return true;
        }

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

            // Clear in-use shop checks
            CurrentlyUsedShopLocations.Clear();

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

            // Ignore if first shop, is not the item at Index 0 or random chance fails (30%)
            if (isFirstShop || index > 0 || UnityEngine.Random.Range(0, 10) < ShopCheckChance)
            {
                return true;
            }

            try
            {
                // Get remaining, not-in-use shop checks
                Dictionary<long, ScoutedItemInfo> remainingShopChecks = CursedWordsArchipelago.Instance.RemainingShopChecks
                    .Where(sc => !CurrentlyUsedShopLocations.Contains(sc.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

                // Ignore if none remain
                if (remainingShopChecks.Count == 0)
                {
                    return true;
                }

                // Randomly select location
                int selectedCheck = UnityEngine.Random.Range(0, remainingShopChecks.Count);
                KeyValuePair<long, ScoutedItemInfo> shopCheck = remainingShopChecks.ElementAt(selectedCheck);

                // Add to in-use shop checks
                CurrentlyUsedShopLocations.Add(shopCheck.Key);

                // Add stamp to stamps in stock
                ItemInStock itemInStock = new ItemInStock(new ArchipelagoShopitem(shopCheck.Value, false));
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

            // Ignore if first shop, is not the item at Index 0 or random chance fails (30%)
            if (isFirstShop || index > 0 || UnityEngine.Random.Range(0, 10) < ShopCheckChance)
            {
                return true;
            }

            try
            {
                // Get remaining, not-in-use shop checks
                Dictionary<long, ScoutedItemInfo> remainingShopChecks = CursedWordsArchipelago.Instance.RemainingShopChecks
                    .Where(sc => !CurrentlyUsedShopLocations.Contains(sc.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

                // Ignore if none remain
                if (remainingShopChecks.Count == 0)
                {
                    return true;
                }

                // Randomly select location check
                int selectedCheck = UnityEngine.Random.Range(0, remainingShopChecks.Count);
                KeyValuePair<long, ScoutedItemInfo> shopCheck = remainingShopChecks.ElementAt(selectedCheck);

                // Add to in-use shop checks
                CurrentlyUsedShopLocations.Add(shopCheck.Key);

                // Add sticker to stickers in stock
                ItemInStock itemInStock = new ItemInStock(new ArchipelagoShopitem(shopCheck.Value));
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

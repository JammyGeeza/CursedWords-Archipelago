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
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ShopController))]
    internal class ShopController_Patches : PatchBase
    {
        private static readonly Dictionary<GlyphType, int> ConsumableTileTypeWeighting = new Dictionary<GlyphType, int>()
        {
            { GlyphType.BespokeCard, 1 },
            { GlyphType.Blank, 1 },
            { GlyphType.Chess, 1 },
            { GlyphType.Currency, 1 },
            { GlyphType.Fraction, 1 },
            { GlyphType.Letter, 19 },
            { GlyphType.Number, 1 },
            { GlyphType.ScatteredItem, 1 },
        };

        private static List<long> CurrentlyUsedShopLocations = new List<long>();

        private static int ShopCheckChance = 33;

        /// <summary>
        /// Override logic for generating tiles for the shop
        /// </summary>
        [HarmonyPatch("GenerateTileInStock")]
        [HarmonyPrefix]
        private static bool GenerateTileInStock_Prefix(ref ShopController __instance, int index)
        {
            Logger.LogInfo($"{nameof(ShopController)}.GenerateTileInStock prefix!");

            if (GameStatics.GetPlayer().GetUnpackedItemsOfType(typeof(DeliveryTruck)).Count > 0)
            {
                return true;
            }

            // Randomly select tile type (mimicking vanilla logic)
            GlyphType glyphType = GlyphType.None;
            int num = UnityEngine.Random.Range(0, ConsumableTileTypeWeighting.Values.Sum());
            foreach (KeyValuePair<GlyphType, int> consumableTileTypeWeighting in ConsumableTileTypeWeighting)
            {
                if (num < consumableTileTypeWeighting.Value)
                {
                    glyphType = consumableTileTypeWeighting.Key;
                    break;
                }

                num -= consumableTileTypeWeighting.Value;
            }

            bool isLetter = glyphType is GlyphType.Letter;
            
            // Create tile based on selected glyph
            Tile tile = new Tile();
            switch (glyphType)
            {
                case GlyphType.BespokeCard:
                    tile.SetGlyphType(glyphType);
                    tile.SetSuit(Suit.Joker);
                    break;

                case GlyphType.Blank:
                    tile.SetGlyphType(glyphType);
                    break;

                case GlyphType.Chess:
                    tile.SetToRandomChessPiece();
                    break;

                case GlyphType.Currency:
                    tile.SetToRandomCurrency();
                    break;

                case GlyphType.Fraction:
                    tile.SetToRandomFraction();
                    break;

                case GlyphType.Letter:
                    tile.SetToRandomLetter();
                    break;

                case GlyphType.Number:
                    tile.SetToRandomNumber();
                    break;

                case GlyphType.ScatteredItem:
                    tile.SetToRandomItem();
                    break;
            }

            // Set suit using vanilla weighting
            if (UnityEngine.Random.Range(0, 10) == 0 && tile.GetSuit() != Suit.Joker)
            {
                tile.SetSuit(PlayingCardUtility.GetRandomCardSuit());
            }

            // Set colour using vanilla logic
            tile.SetTileType(isLetter ? ItemPools.GetRandomColouredTileTypeWeighted() : ItemPools.GetRandomTileTypeWeighted());

            // Create tile in stock, set and populate it
            TileInStock tileInStock = new TileInStock(tile, __instance.GetTileTypeCost(tile.GetTileType()));
            __instance.SetTileInStock(tileInStock, index);
            __instance.GetShopVisualController()
                .CallPopulateTileInStock(tileInStock, index);

            return false;
        }

        /// <summary>
        /// Ensure all tile types appear in the shop from the start
        /// </summary>
        [HarmonyPatch("GetTotalFrequency")]
        [HarmonyPrefix]
        private static bool GetTotalFrequency_Prefix()
        {
            Logger.LogInfo($"{nameof(ShopController)}.GetTotalFrequency prefix!");

            Type targetType = typeof(ShopController);
            FieldInfo typeWeightingsField = AccessTools.Field(targetType, "_consumableTileTypeWeightings");
            FieldInfo totalWeightingField = AccessTools.Field(targetType, "_consumableTileTypeTotalWeighting");
            Dictionary<GlyphType, int> weightings = (Dictionary<GlyphType, int>)typeWeightingsField.GetValue(null);

            // TODO: Suited cards don't appear yet
            //       Currency cards don't appear yet
            //       Fractions are reliant on RNG based on 'Number' glyph

            // Insert vanilla glyph type scaling
            weightings[GlyphType.Letter] = 19;
            weightings[GlyphType.Number] = 1;
            weightings[GlyphType.BespokeCard] = 1;
            weightings[GlyphType.Chess] = 1;

            // Insert missing glyphs
            weightings[GlyphType.Fraction] = 1;
            weightings[GlyphType.Currency] = 1;

            // Set total weighting as per above
            totalWeightingField.SetValue(null, weightings.Values.Sum());

            return false;
        }

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
                    if (player.Money >= archipelagoShopItem.Cost)
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

                        // Play purchase sound
                        PersistentSound.SingletonSoundController.BuyItem(itemSlot.MyItemInStock.MyItem, false);
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

        /// <summary>
        /// When leaving the shop, check for frozen items and trigge check
        /// </summary>
        [HarmonyPatch(nameof(ShopController.OnLeaveShopButtonClickedCallback))]
        [HarmonyPrefix]
        private static void OnOnLeaveShopButtonClickedCallback_Prefix(ShopController __instance)
        {
            Logger.LogInfo($"{nameof(ShopController)}.{nameof(ShopController.OnLeaveShopButtonClickedCallback)} prefix!");

            // Check if any stamps have been frozen
            if (__instance.GetStampsInStock().FirstOrDefault(s => s != null && s.MyItem.GetType() != typeof(ArchipelagoShopitem) && s.IsFrozen) is ItemInStock stampInStock)
            {
                CursedWordsArchipelago.Instance.TryCheckItemActionLocations("freeze", stampInStock.MyItem);
            }

            // Check if any stickers have been frozen
            if (__instance.GetStickersInStock().FirstOrDefault(s => s != null && s.MyItem.GetType() != typeof(ArchipelagoShopitem) && s.IsFrozen) is ItemInStock stickerInStock)
            {
                CursedWordsArchipelago.Instance.TryCheckItemActionLocations("freeze", stickerInStock.MyItem);
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

            // Ignore if Shopsanity is disabled
            if (!ArchipelagoHelper.SlotData.Shopsanity)
            {
                return true;
            }

            // Ignore if first shop, is not the item at Index 0 or random chance fails (33%)
            if (isFirstShop || index > 0 || UnityEngine.Random.Range(0, 100) > ShopCheckChance)
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

            // Ignore if Shopsanity is disabled
            if (!ArchipelagoHelper.SlotData.Shopsanity)
            {
                return true;
            }

            // Ignore if first shop, is not the item at Index 0 or random chance fails (33%)
            if (isFirstShop || index > 0 || UnityEngine.Random.Range(0, 100) > ShopCheckChance)
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

using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using BepInEx.Logging;
using Modd;

namespace Mod.Extensions
{
    public static class ShopControllerExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Call the private 'BuyStamp' method.
        /// </summary>
        /// <param name="controller">The shop controller to call the method for.</param>
        /// <param name="slot">The shop item slot to buy.</param>
        /// <returns>Success result</returns>
        public static bool CallBuyStamp(this ShopController controller, ShopItemSlot slot)
        {
            return Traverse.Create(controller)
                .Method("BuyStamp", slot)
                .GetValue<bool>();
        }

        /// <summary>
        /// Call the private 'BuySticker' method.
        /// </summary>
        /// <param name="controller">The shop controller to call the method for.</param>
        /// <param name="slot">The shop item slot to buy.</param>
        /// <returns>Success result</returns>
        public static bool CallBuySticker(this ShopController controller, ShopItemSlot slot)
        {
            return Traverse.Create(controller)
                .Method("BuySticker", slot, false)
                .GetValue<bool>();
        }

        /// <summary>
        /// Get the current re-roll price.
        /// </summary>
        /// <param name="controller">The controller to get the re-roll price from.</param>
        /// <returns>The current re-roll price.</returns>
        public static int GetRerollPrice(this ShopController controller)
        {
            return Traverse.Create(controller)
                .Method("GetRerollPrice")
                .GetValue<int>();
        }

        /// <summary>
        /// Get the visual controller for the shop.
        /// </summary>
        /// <param name="controller">The shop controller to get the visual controller for.</param>
        /// <returns>The shop visual controller.</returns>
        public static ShopVisualController GetShopVisualController(this ShopController controller)
        {
            return Traverse.Create(controller)
                .Field("_shopVisualController")
                .GetValue<ShopVisualController>();
        }

        /// <summary>
        /// Get the current stamps in stock.
        /// </summary>
        /// <param name="controller">The controller instance to get the stamps in stock for.</param>
        /// <returns>The current stamps in stock.</returns>
        public static List<ItemInStock> GetStampsInStock(this ShopController controller)
        {
            return Traverse.Create(controller)
                .Field("_stampsInStock")
                .GetValue<ItemInStock[]>()
                .ToList();
        }

        /// <summary>
        /// Get the index of a stmap in stock.
        /// </summary>
        /// <param name="controller">The controller to get the stamp index from.</param>
        /// <returns>The index of the stamp.</returns>
        public static int GetStampInStockIndex(this ShopController controller, ItemInStock itemInStock)
        {
            return GetStampsInStock(controller).IndexOf(itemInStock);
        }

        /// <summary>
        /// Get the index of a stmap in stock.
        /// </summary>
        /// <param name="controller">The controller to get the stamp index from.</param>
        /// <returns>The index of the stamp.</returns>
        public static int GetStickerInStockIndex(this ShopController controller, ItemInStock itemInStock)
        {
            return Array.IndexOf(controller.GetStickersInStock(), itemInStock);
        }

        /// <summary>
        /// Remove a stamp in stock by its index.
        /// </summary>
        /// <param name="controller">The controller to remove the stamp from.</param>
        /// <param name="index">The index of the stamp to remove.</param>
        public static void RemoveStampInStock(this ShopController controller, int index)
        {
            Traverse.Create(controller)
                .Field("_stampsInStock")
                .GetValue<ItemInStock[]>()[index] = null;
        }

        /// <summary>
        /// Remove a sticker in stock by its index.
        /// </summary>
        /// <param name="controller">The controller to remove the sticker from.</param>
        /// <param name="index">The index of the sticker to remove.</param>
        public static void RemoveStickerInStock(this ShopController controller, int index)
        {
            Traverse.Create(controller)
                .Field("_stickersInStock")
                .GetValue<ItemInStock[]>()[index] = null;
        }
    }
}

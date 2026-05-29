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
    public static class ShopItemSlotExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        public static void SetBuyButton(this ShopItemSlot shopItemSlot, string text, Color32 topColor, Color32 backgroundColor, bool interactable)
        {
            // Traverse to find private fields
            TextMeshProUGUI buyButtonTMP = Traverse.Create(shopItemSlot)
                .Field("_buyButtonTMP")
                .GetValue<TextMeshProUGUI>();

            Image buyButtonTopImage = Traverse.Create(shopItemSlot)
                .Field("_buyButtonTopImage")
                .GetValue<Image>();

            Image buyButtonBackgroundImage = Traverse.Create(shopItemSlot)
                .Field("_buyButtonBackgroundImage")
                .GetValue<Image>();

            Button buyButton = Traverse.Create(shopItemSlot)
                .Field("_buyButton")
                .GetValue<Button>();

            // Set values
            buyButtonTMP.SetText(text);
            buyButtonTopImage.color = topColor;
            buyButtonBackgroundImage.color = backgroundColor;
            buyButton.interactable = interactable;
        }
    }
}

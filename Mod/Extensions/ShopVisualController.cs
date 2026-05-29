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
    public static class ShopVisualControllerExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Hide the 'Freeze' button of a specified shop item.
        /// </summary>
        /// <param name="controller">The controller to hide the freeze button for.</param>
        /// <param name="index">The index of the freeze button to hide.</param>
        public static void HideFreezeButton(this ShopVisualController controller, int index)
        {
            GameObject[] freezeButtons = Traverse.Create(controller)
                .Field("_freezeButtons")
                .GetValue<GameObject[]>();

            freezeButtons[index].SetActive(false);
        }
    }
}

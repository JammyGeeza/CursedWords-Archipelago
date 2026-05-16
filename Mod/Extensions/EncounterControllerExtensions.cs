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
    public static class EncounterControllerExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Get the current encounter re-roll amount.
        /// </summary>
        /// <param name="controller">The controller to get the amount for</param>
        /// <returns>The current encounter re-roll amount.</returns>
        public static int GetRerollAmount(this EncounterController controller)
        {
            return Traverse.Create(controller)
                .Field("_rerollsForEncounter")
                .GetValue<int>();
        }

        /// <summary>
        /// Increment the current encounter re-roll amount.
        /// </summary>
        /// <param name="controller">The controller to set the amount for.</param>
        /// <param name="incrementAmount">The amount to increment the re-roll amount by.</param>
        public static void IncrementEncounterRerollAmount(this EncounterController controller, int incrementAmount)
        {
            int newAmount = GetRerollAmount(controller) + incrementAmount;
            SetEncounterRerollAmount(controller, newAmount);
        }

        /// <summary>
        /// Set the current encounter re-roll amount.
        /// </summary>
        /// <param name="controller">The controller to set the amount for.</param>
        /// <param name="newAmount">The amount to set the amount to.</param>
        public static void SetEncounterRerollAmount(this EncounterController controller, int newAmount)
        {
            bool isOverZero = newAmount > 0;

            Logger.LogInfo($"Setting encounter re-rolls to: {newAmount}");

            // Set re-roll count based on received items
            Traverse.Create(controller)
                .Field("_rerollsForEncounter")
                .SetValue(newAmount);

            TextMeshProUGUI _rerollTMP = Traverse.Create(controller)
                .Field("_rerollTMP")
                .GetValue<TextMeshProUGUI>();

            GameObject _rerollButtonObject = Traverse.Create(controller)
                .Field("_rerollButtonObject")
                .GetValue<GameObject>();

            GameObject _rerollLine = Traverse.Create(controller)
                .Field("_rerollLine")
                .GetValue<GameObject>();

            string wheelText = GameStatics.GetPlayer().GetUnpackedItemsOfType(typeof(Wheel)).Count > 0
                ? "<br> $1 EACH"
                : "";

            string rerollText = isOverZero
                ? $"<b>{newAmount}</b> <size=18>REROLL{Item.CheckPlural("S", newAmount)} <br>REMAINING{wheelText}"
                : "NO REROLLS<br>LEFT";

            _rerollTMP.text = rerollText;
            _rerollButtonObject.GetComponentInChildren<Button>().interactable = isOverZero;
            _rerollLine.SetActive(value: isOverZero);
        }
    }
}

using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Collections;
using TMPro;
using Modd;
using Mod.Enums;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(CharacterSelectVisualController))]
    internal class CharacterSelectVisualController_Patches : PatchBase
    {
        [HarmonyPatch(nameof(CharacterSelectVisualController.SelectCrown))]
        [HarmonyPrefix]
        private static void SelectCrown_Prefix(CharacterSelectVisualController __instance, ref Crown crown, Character character)
        {
            Logger.LogDebug($"{nameof(CharacterSelectVisualController)}.{nameof(CharacterSelectVisualController.SelectCrown)} prefix!");

            // Check if the crown level being navigated to is higher than has been received for the character
            // and if so, flick back to no crowns.
            int highestCrownReceived = ArchipelagoHelper.AmountOfItemReceived($"{character.GetName()}: Progressive Crown");
            int targetCrownIndex = (int)crown.Level;
            int adjustedIndex = targetCrownIndex > highestCrownReceived ? 0 : targetCrownIndex;

            if (targetCrownIndex != adjustedIndex)
            {
                crown = CharacterSelectController.Crowns[adjustedIndex];

                if (UnityEngine.Object.FindFirstObjectByType<CharacterSelectController>() is CharacterSelectController controller)
                {
                    // Adjust the target crown index
                    Traverse.Create(controller)
                        .Field("_crownIndex")
                        .SetValue(adjustedIndex);
                }
            }
        }

        /// <summary>
        /// Show crowns selecter for all unlocked characters (if enabled)
        /// </summary>
        [HarmonyPatch("PopulateElements")]
        [HarmonyPostfix]
        public static void PopulateElements_Postfix(CharacterSelectVisualController __instance, Character character, bool isUnlocked, ref IEnumerator __result)
        {
            Logger.LogDebug($"{nameof(CharacterSelectVisualController)}.PopulateElements postfix!");

            __result = Wrapped(__result, __instance, character, isUnlocked);
        }

        /// <summary>
        /// If character is unlocked, force the crowns selecter to be displayed.
        /// </summary>
        private static IEnumerator Wrapped(IEnumerator original, CharacterSelectVisualController controller, Character character, bool isUnlocked)
        {
            Logger.LogDebug($"Wrapped() started...");

            if (original == null)
            {
                Logger.LogError("Original instance is null");
                yield break;
            }

            // Get the crowns panel
            GameObject crownsPanelGO = Traverse.Create(controller)
                .Field("_crownsPanelGO")
                .GetValue<GameObject>();

            // Get the crown path text
            TextMeshProUGUI crownCompletionTMP = Traverse.Create(controller)
                .Field("_crownCompletionTMP")
                .GetValue<TextMeshProUGUI>();

            // Get the crown path display
            GameObject crownCompletionTextGO = Traverse.Create(controller)
                .Field("_crownCompletionTextGO")
                .GetValue<GameObject>();

            // Get count of characters that have met the goal condition
            int charactersCompleted = 0;
            foreach (string characterName in ArchipelagoHelper.SlotData.GoalRequirements)
            {
                Type characterType = CursedWordsArchipelago.Instance.CharacterTypeCache
                    .FirstOrDefault(kvp => kvp.Value.Equals(characterName))
                    .Key;

                // If completed, increment count
                if (CursedWordsArchipelago.Instance.HasCharacterMetGoalCriteria(characterType))
                {
                    charactersCompleted++;
                }
            }

            // Get crown colour
            string crownColour = ArchipelagoHelper.SlotData.CrownRequirement switch
            {
                1 => "Purple",
                2 => "Yellow",
                3 => "Orange",
                4 => "Pink",
                5 => "Green",
                6 => "Blue",
                7 => "Red",
                _ => string.Empty
            };

            // Get general goal text
            GoalType goalType = ArchipelagoHelper.SlotData.GoalType;
            string goalText = goalType switch
            {
                GoalType.Crowns => $"Clear {crownColour} Crown",
                GoalType.Michael => $"Clear Michael",
                GoalType.Runs => $"Clear Runs",
            };

            Logger.LogDebug("Completing original task...");
            while (original.MoveNext())
            {
                // Enable crowns panel if character is unlocked
                if (isUnlocked && crownsPanelGO != null && !crownsPanelGO.activeSelf)
                {
                    crownsPanelGO.SetActive(true);
                }

                // Always show completion text (attempting to re-force
                if (crownCompletionTextGO is not null && !crownCompletionTextGO.activeSelf)
                {
                    crownCompletionTextGO.SetActive(true);
                }

                if (crownCompletionTMP is not null && !crownCompletionTMP.text.Contains("Goal Condition"))
                {
                    // Set goal text
                    crownCompletionTMP.SetText($"Goal Condition - <#FFFFFF>{goalText} ({charactersCompleted} of {ArchipelagoHelper.SlotData.GoalRequirements.Length})");
                }

                yield return original.Current;
            }

            // Need to re-force some of these due to the original method's '!isUnlocked' path not yielding
            // so it prevents the elements "flashing" by doubly making sure they're set

            // Enable crowns panel if character is unlocked
            if (isUnlocked && crownsPanelGO != null && !crownsPanelGO.activeSelf)
            {
                crownsPanelGO.SetActive(true);
            }

            // Always show completion text (attempting to re-force
            if (crownCompletionTextGO is not null && !crownCompletionTextGO.activeSelf)
            {
                crownCompletionTextGO.SetActive(true);
            }

            // Set goal completion text
            if (crownCompletionTMP is not null && !crownCompletionTMP.text.Contains("Goal Condition"))
            {
                // Set goal text
                crownCompletionTMP.SetText($"Goal Condition - <#FFFFFF>{goalText} ({charactersCompleted} of {ArchipelagoHelper.SlotData.GoalRequirements.Length})");
            }
        }
    }
}

using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(CharacterSelectTile))]
    internal class CharacterSelectTile_Patches : PatchBase
    {
        /// <summary>
        /// Prevent character default build items if not yet unlocked
        /// </summary>
        [HarmonyPatch(nameof(CharacterSelectTile.Highlight))]
        [HarmonyPrefix]
        private static bool Highlight_Prefix(CharacterSelectTile __instance)
        {
            // Check if character is included in goal criteria
            string characterName = CursedWordsArchipelago.Instance.CharacterTypeCache[__instance.MyCharacter.GetType()];
            if (ArchipelagoHelper.SlotData.GoalRequirements.Contains(characterName))
            {
                Image borderImage = Traverse.Create(__instance)
                    .Field("_borderImage")
                    .GetValue<Image>();

                // Set colour based on if character has met goal criteria
                borderImage.color = CursedWordsArchipelago.Instance.HasCharacterMetGoalCriteria(__instance.MyCharacter)
                    ? new Color(0f, 1f, 0f, 0.8f)
                    : new Color(1f, 0f, 0f, 0.8f);

                return false;
            }

            return true;
        }

        [HarmonyPatch(nameof(CharacterSelectTile.Deselect))]
        [HarmonyPrefix]
        private static bool Deselect_Prefix(CharacterSelectTile __instance)
        {
            // Check if character is included in goal criteria
            string characterName = CursedWordsArchipelago.Instance.CharacterTypeCache[__instance.MyCharacter.GetType()];
            if (ArchipelagoHelper.SlotData.GoalRequirements.Contains(characterName))
            {
                Image borderImage = Traverse.Create(__instance)
                    .Field("_borderImage")
                    .GetValue<Image>();

                // Set colour based on if character has met goal criteria
                borderImage.color = CursedWordsArchipelago.Instance.HasCharacterMetGoalCriteria(__instance.MyCharacter)
                    ? new Color(0f, 1f, 0f, 0.2f)
                    : new Color(1f, 0f, 0f, 0.2f);

                return false;
            }

            return true;
        }
    }
}

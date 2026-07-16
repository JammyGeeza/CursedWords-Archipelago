using FullSerializer;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveFileUtility))]
    internal class SaveFileUtility_Patches : PatchBase
    {
        /// <summary>
        /// Override default save data from newly created saves.
        /// </summary>
        [HarmonyPatch(nameof(SaveFileUtility.GetSaveFileFromFsData))]
        [HarmonyPostfix]
        public static void GetSaveFileFromFsData_Postfix(Dictionary<string, fsData> saveFileDict, ref SaveFile __result)
        {
            // If save has not been entered before, set save data to a 'blank' state
            if (!__result.HasEnteredSaveFile)
            {
                Logger.LogInfo("Adjusting new save file");

                // Default progress wipe
                __result.CharacterHighestCompletedAscensions = new Dictionary<Type, int>();
                __result.BulkUnlocksUnlocked = new List<Type>();

                // Dialogue skips
                __result.IsTutorialComplete = true;
                __result.HasSeenBonesIntroDialogue = true;
                __result.HasSeenChallengeUnlockDialogue = true;
                __result.HasSeenChessUnlockDialogue = true;
                __result.HasSeenCrownsUnlockDialogue = true;
                __result.HasSeenCurrencyFirstTimeDialogue = true;
                __result.HasSeenFairyInClubhouseDialogue = true;
                __result.HasSeenFirstBossDraftDialogue = true;
                __result.HasSeenFirstPinDraftDialogue = true;
                __result.HasSeenFloorFourUnlockDialogue = true;
                __result.HasSeenFractionUnlockDialogue = true;
                __result.HasSeenGlitchTileDialogue = true;
                __result.HasSeenNatIntroDialogue = true;
                __result.HasSeenNinaIntroDialogue = true;
                __result.HasSeenNumbersUnlockDialogue = true;
                __result.HasSeenOctaclesIntroDialogue = true;
                __result.HasSeenScatteredItemDialogue = true;
                __result.HasSeenUpgradeFirstTimeDialogue = true;
                __result.HasSeenWeirdColourDialogue = true;
                __result.HasSeenWobblyFirstTimeDialogue = true;
            };
        }
    }
}

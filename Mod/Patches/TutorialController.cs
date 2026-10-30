using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(TutorialController))]
    internal class TutorialController_Patches : PatchBase
    {
        /// <summary>
        /// Prevent secret character unlock dialogues from unlocking the secret character.
        /// </summary>
        [HarmonyPatch(nameof(TutorialController.SecretCharacterUnlockDialogue), MethodType.Enumerator)]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnSecretCharacterUnlockDialogue_Prefix(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceCall(
                instructions,
                typeof(SaveManager),
                nameof(SaveManager.AddCharacterToUnlockedCharacters),
                OpCodes.Ldc_I4_1);
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Supervillain))]
    internal class Supervillain_Patches : PatchBase
    {
        /// <summary>
        /// Ensure that Supervillain can always spawn all cursed types.
        /// </summary>
        [HarmonyPatch(typeof(Supervillain), nameof(Supervillain.ApplyStartOfGridEffect))]
        static class Supervillain_Patches_ForceAllCurseTypes
        {
            static readonly MethodInfo TargetMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.IsBulkUnlockUnlocked));
            static readonly MethodInfo ReplacementMethod = AccessTools.Method(typeof(Supervillain_Patches_ForceAllCurseTypes), nameof(IsBulkUnlockUnlockedOverride));

            /// <summary>
            /// Always return 'true' for bulk unlocks being unlocked to ensure all curse types can be spawned.
            /// </summary>
            static bool IsBulkUnlockUnlockedOverride(Type unlockType) => true;

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
            {
                foreach (CodeInstruction instruction in codeInstructions)
                {
                    yield return instruction.Calls(TargetMethod)
                        ? new CodeInstruction(OpCodes.Call, ReplacementMethod)
                        : instruction;
                }
            }
        }
    }
}

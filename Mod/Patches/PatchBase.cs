using BepInEx.Logging;
using HarmonyLib;
using Modd;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Mod.Patches
{
    public abstract class PatchBase
    {
        protected static ManualLogSource Logger => CursedWordsArchipelago.Instance.LogSource;

        /// <summary>
        /// Replace a method call.
        /// </summary>
        /// <param name="codeInstructions">The code instructions from the transpiled method.</param>
        /// <param name="declaringType">The type the method call belongs to.</param>
        /// <param name="methodName">The method name to replace.</param>
        /// <param name="result">(OPTIONAL) The result to replace.</param>
        /// <exception cref="InvalidOperationException">Throws if multiple calls to the same method detected.</exception>
        protected static IEnumerable<CodeInstruction> ReplaceCall(
            IEnumerable<CodeInstruction> codeInstructions,
            Type declaringType,
            string methodName,
            OpCode? result = null)
        {
            int patched = 0;
            foreach (CodeInstruction instruction in codeInstructions)
            {
                // Check for call to matching method
                if ((instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt)
                    && instruction.operand is MethodInfo m
                    && m.DeclaringType == declaringType
                    && m.Name == methodName)
                {
                    // How many args in the method?
                    int args = m.GetParameters().Length + (m.IsStatic ? 0 : 1);

                    // Remove arguments instead of calling method 
                    instruction.opcode = OpCodes.Pop;
                    instruction.operand = null;
                    yield return instruction;

                    for (int i = 1; i < args; i++)
                        yield return new CodeInstruction(OpCodes.Pop);

                    // If method returned a value, replace the value
                    if (m.ReturnType != typeof(void))
                        yield return new CodeInstruction(result ?? (m.ReturnType.IsValueType ? OpCodes.Ldc_I4_0 : OpCodes.Ldnull));

                    patched++;
                    continue;
                }

                // Continue through un-touched calls
                yield return instruction;
            }

            // If call found multiple times, something's gone wrong
            if (patched != 1)
                throw new InvalidOperationException($"Transpiler for {methodName} patched {patched} calls instead of one.");

        }
    }
}

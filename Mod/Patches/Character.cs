using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal class Character_Patches
    {
        [HarmonyPatch(nameof(Character.GetCharacterItemTypes))]
        [HarmonyPostfix]
        private static void GetCharacterItemTypes_Postfix(Character __instance, ref List<Type> __result)
        {
            Debug.Log($"{nameof(Character)}.{nameof(Character.GetCharacterItemTypes)} postfix!");

            switch (__instance)
            {
                case WetDennis wetDennis:
                    if (!ArchipelagoHelper.HasReceivedItem("Rodman Sticker Bundle"))
                    {
                        __result.Clear();
                    }
                    return;

                case NinaNix ninaNix:
                    return;
            }
        }

        /// <summary>
        /// Override unlock criteria text.
        /// </summary>
        [HarmonyPatch(nameof(Character.GetUnlockRequirementText))]
        [HarmonyPrefix]
        private static bool GetUnlockRequirementText_Prefix(Character __instance, ref string __result)
        {
            __result = $"Receive the '{__instance.GetName()}' archipelago item to unlock this character";
            return false;
        }
    }
}

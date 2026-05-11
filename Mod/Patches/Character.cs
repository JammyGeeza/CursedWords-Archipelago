using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal class Character_Patches
    {
        /// <summary>
        /// Prevent character default build items if not yet unlocked
        /// </summary>
        [HarmonyPatch(nameof(Character.GetCharacterItemTypes))]
        [HarmonyPostfix]
        private static void GetCharacterItemTypes_Postfix(Character __instance, ref List<Type> __result)
        {
            Debug.Log($"{nameof(Character)}.{nameof(Character.GetCharacterItemTypes)} postfix!");

            for (int i = __result.Count - 1; i >= 0; i--)
            {
                Type type = __result[i];

                if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Blue Tiles") && Lookups.BlueStickers.Contains(type))
                {
                    __result.Remove(type);
                }

                if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Number Tiles") && Lookups.NumberStickers.Contains(type))
                {
                    __result.Remove(type);
                }

                if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Red Tiles") && Lookups.RedStickers.Contains(type))
                {
                    __result.Remove(type);
                }

                if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Shiny Tiles") && Lookups.ShinyStickers.Contains(type))
                {
                    __result.Remove(type);
                }

                if (!ArchipelagoHelper.HasReceivedItem("Sticker Bundle - Void Tiles") && Lookups.VoidStickers.Contains(type))
                {
                    __result.Remove(type);
                }
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

using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(CharacterSelectVisualController))]
    internal class CharacterSelectVisualController_Patches : PatchBase
    {
        [HarmonyPatch(nameof(CharacterSelectVisualController.SelectCrown))]
        [HarmonyPrefix]
        private static void SelectCrown_Prefix(CharacterSelectVisualController __instance, ref Crown crown, Character character)
        {
            Logger.LogInfo($"{nameof(CharacterSelectVisualController)}.{nameof(CharacterSelectVisualController.SelectCrown)} postfix!");

            // Check if the crown level being navigated to is higher than has been received for the character
            // and if so, flick back to no crowns.
            int highestCrownReceived = ArchipelagoHelper.AmountOfItemReceived($"{character.GetName()}: Progressive Crown");
            if ((int)crown.Level > highestCrownReceived)
            {
                crown = CharacterSelectController.Crowns[0];
            }
        }
    }
}

using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SoundController))]
    internal class SoundController_Patches : PatchBase
    {
        /// <summary>
        /// Check location when a fairy is received.
        /// </summary>
        [HarmonyPatch(nameof(SoundController.FairyGet))]
        [HarmonyPostfix]
        private static void OnFairyGet_Postfix()
        {
            Logger.LogInfo($"{nameof(SoundController)}.{nameof(SoundController.FairyGet)} postfix!");

            Player player = GameStatics.GetPlayer();

            // Attempt to send encounter location for fairy
            CursedWordsArchipelago.Instance.TryCheckEncounterLocations("get_fairy", player);
        }
    }
}

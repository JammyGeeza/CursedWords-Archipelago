using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(BossRewardController))]
    internal class BossRewardController_Patches : PatchBase
    {
        /// <summary>
        /// Attempt to check location(s) when a Pin is upgraded
        /// </summary>
        [HarmonyPatch(nameof(BossRewardController.TakeUpgradeButtonCallback))]
        [HarmonyPostfix]
        private static void OnTakeUpgradeButtonCallback_Postfix(BossRewardController __instance)
        {
            Logger.LogDebug($"{nameof(BossRewardController)}.{nameof(BossRewardController.TakeUpgradeButtonCallback)} postfix!");

            // Attempt to check item action for pin upgrade
            CursedWordsArchipelago.Instance.TryCheckItemActionLocations("upgrade", __instance.GetPinItem());
        }

        /// <summary>
        /// Attempt to check location(s) when a Pin is upgraded
        /// </summary>
        [HarmonyPatch(nameof(BossRewardController.TakeUpgradeBothButtonCallback))]
        [HarmonyPostfix]
        private static void OnTakeUpgradeBothButtonCallback_Postfix(BossRewardController __instance)
        {
            Logger.LogDebug($"{nameof(BossRewardController)}.{nameof(BossRewardController.TakeUpgradeBothButtonCallback)} postfix!");

            // Attempt to check item action for pin upgrade
            CursedWordsArchipelago.Instance.TryCheckItemActionLocations("upgrade", __instance.GetPinItem());
        }
    }
}

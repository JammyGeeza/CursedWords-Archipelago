using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Collections;
using TMPro;
using Modd;
using Mod.Enums;
using Mod.Mappings;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(CharacterSelectController))]
    internal class CharacterSelectController_Patches : PatchBase
    {
        // Live accessors for the private fields (cache once)
        static readonly AccessTools.FieldRef<CharacterSelectController, GameObject> Blocker =
            AccessTools.FieldRefAccess<CharacterSelectController, GameObject>("_raycastBlockerGO");

        static readonly AccessTools.FieldRef<CharacterSelectController, bool> MoveOn =
            AccessTools.FieldRefAccess<CharacterSelectController, bool>("_moveOn");

        static readonly AccessTools.FieldRef<CharacterSelectController, DialogueController> Dialogue =
            AccessTools.FieldRefAccess<CharacterSelectController, DialogueController>("_dialogueController");

        /// <summary>
        /// Prevent basement access (for now)
        /// </summary>
        [HarmonyPatch(nameof(CharacterSelectController.SecrectClubhouseButtonCallBack))]
        [HarmonyPrefix]
        private static bool SecretClubhouseButtonCallback_Prefix(CharacterSelectController __instance)
        {
            if (!SaveManager.GetIsDisablingDLCContent())
            {
                __instance.StartCoroutine(CustomBasementLockedDialogue(__instance));
            }

            // Don't trigger the original method
            return false;
        }

        /// <summary>
        /// Display custom dialogue for attempting to access the basement.
        /// </summary>
        private static IEnumerator CustomBasementLockedDialogue(CharacterSelectController __instance)
        {
            Blocker(__instance).SetActive(true);
            yield return new WaitForSeconds(0.35f);

            Dialogue(__instance).DialogueEvent(("The basement is locked?!", Emotions.ShopkeeperConfused));
            while (!MoveOn(__instance))
            {
                yield return null;
            }

            MoveOn(__instance) = false;
            yield return new WaitForSeconds(0.35f);

            Dialogue(__instance).DialogueEvent(("It's not supported for Archipelago yet, but will be soon!", Emotions.ShopkeeperExplaining));
            while (!MoveOn(__instance))
            {
                yield return null;
            }

            MoveOn(__instance) = false;
            Blocker(__instance).SetActive(false);
        }
    }
}

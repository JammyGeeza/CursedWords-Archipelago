using Archipelago.MultiClient.Net.Models;
using FullSerializer;
using HarmonyLib;
using Mod.GameObjects;
using Mod.Helpers;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ShopVisualController))]
    internal class ShopVisualController_Patches
    {
        /// <summary>
        /// Produce dialog to enter/adjust archipelago credentials before loading save.
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="slotIndex"></param>
        //[HarmonyPatch(nameof(ShopVisualController.ChangeStampSlotVisibility))]
        //[HarmonyPrefix]
        //private static void ChangeStampSlotVisibility_Prefix(bool isVisible)
        //{
        //    Debug.Log($"{nameof(ShopVisualController)}.{nameof(ShopVisualController.ChangeStampSlotVisibility)} prefix!");

        //    isVisible = false;
        //}
    }
}

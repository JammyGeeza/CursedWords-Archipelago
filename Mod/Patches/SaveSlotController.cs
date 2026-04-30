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
    [HarmonyPatch(typeof(SaveSlotController))]
    internal class SaveSlotController_Patches
    {
        ///// <summary>
        ///// Produce dialog to enter/adjust archipelago credentials before loading save.
        ///// </summary>
        ///// <param name="__result"></param>
        ///// <param name="slotIndex"></param>
        [HarmonyPatch("SelectSaveFile")]
        [HarmonyPostfix]
        public static void SelectSaveFile_Postfix(ref IEnumerator __result, int slotIndex)
        {
            Debug.Log("SaveSlotController.SelectSaveFile Postfix!");

            __result = Wrapped(__result, slotIndex);
        }

        private static IEnumerator Wrapped(IEnumerator original, int slotIndex)
        {
            Debug.Log("Wrapped() started");

            if (original == null)
            {
                Debug.LogError("Original IEnumerator is null");
                yield break;
            }

            Debug.Log("Getting archipelago data...");

            // Create login controller
            ArchipelagoData archipelagodata = ArchipelagoData.GetDataForSaveSlot(slotIndex);
            ArchipelagoLoginController controller = ArchipelagoLoginController.Create(archipelagodata);

            // 'Connect' click handler
            controller.OnConnect = delegate
            {
                controller.SetState(DialogState.Connecting, null);

                // TODO: Reject if any fields missing data

                // Attempt to connect
                controller.StartCoroutine(ArchipelagoHelper.LoginRoutine(controller));

                // Overwrite archipelago data
                ArchipelagoData.SaveDataForSaveSlot(slotIndex, new ArchipelagoData
                {
                    Host = controller.Host,
                    Slot = controller.Slot,
                    Password = controller.Password
                });
            };

            // 'Cancel' click handler
            controller.OnCancel = delegate
            {
                controller.Close();
            };

            // Wait for menu to close
            yield return controller.WaitForFinish();

            // If cancelled, don't continue
            if (controller.Cancelled)
            {
                yield break;
            }

            // Continue with original method's tasks
            Debug.Log("Completing original task...");
            while (original.MoveNext())
                yield return original.Current;
        }
    }
}

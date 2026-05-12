using HarmonyLib;
using Mod.Helpers;
using System.Collections;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveSlotController))]
    internal class SaveSlotController_Patches : PatchBase
    {
        /// <summary>
        /// Produce dialog to enter/adjust archipelago credentials before loading save.
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="slotIndex"></param>
        [HarmonyPatch("SelectSaveFile")]
        [HarmonyPostfix]
        public static void SelectSaveFile_Postfix(ref IEnumerator __result, int slotIndex)
        {
            Logger.LogInfo("SaveSlotController.SelectSaveFile Postfix!");

            __result = Wrapped(__result, slotIndex);
        }

        /// <summary>
        /// Wrap IEnumerator to perform tasks before enumerating.
        /// </summary>
        /// <param name="original">Original <see cref="IEnumerator"/> to wrap.</param>
        /// <param name="slotIndex">The save slot index.</param>
        /// <returns></returns>
        private static IEnumerator Wrapped(IEnumerator original, int slotIndex)
        {
            Logger.LogInfo("Wrapped() started");

            if (original == null)
            {
                Debug.LogError("Original IEnumerator is null");
                yield break;
            }

            Logger.LogInfo("Getting archipelago data...");

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
            Logger.LogInfo("Completing original task...");
            while (original.MoveNext())
                yield return original.Current;
        }
    }
}

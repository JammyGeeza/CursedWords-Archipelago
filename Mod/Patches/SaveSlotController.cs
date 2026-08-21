using HarmonyLib;
using Mod.Helpers;
using Modd;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveSlotController))]
    internal class SaveSlotController_Patches : PatchBase
    {
        [HarmonyPatch(nameof(SaveSlotController.Populate))]
        [HarmonyPostfix]
        public static void Populate_Postfix(SaveSlotController __instance, SaveFile saveFile, bool isNewFile)
        {
            Logger.LogInfo($"{nameof(SaveSlotController)}.{nameof(SaveSlotController.Populate)} postfix!");

            // Attempt to get the controller
            if (UnityEngine.Object.FindFirstObjectByType<SaveSlotsController>() is SaveSlotsController controller && controller != null)
            {
                // Get save slots
                SaveSlotController[] slots = Traverse.Create(controller)
                    .Field("_saveSlots")
                    .GetValue<SaveSlotController[]>();

                // Get index of save slot
                int slotIndex = slots.ToList().IndexOf(__instance);

                // Get the Archipelago Data for the slot
                ArchipelagoData apData = ArchipelagoData.GetDataForSaveSlot(slotIndex + 1);
                foreach (TextMeshProUGUI textMesh in __instance.GetComponentsInChildren<TextMeshProUGUI>().Where(tmp => tmp.name == "SlotX" || tmp.name == "CompletionPercentage"))
                {
                    switch (textMesh.name)
                    {
                        case "SlotX":
                            {
                                if (apData.LocationsTotal > 0)
                                {
                                    textMesh.SetText($"{apData.LocationsCheckedTotal} / {apData.LocationsTotal}");
                                }
                                else
                                {
                                    textMesh.SetText($"??? / ???");
                                }
                            }
                            break;

                        case "CompletionPercentage":
                            {
                                textMesh.fontSize = 26;
                                textMesh.SetText(apData.Slot);
                            }
                            
                            break;
                    }
                }
            }
        }

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

            Logger.LogInfo($"Getting archipelago data for save slot {slotIndex}...");

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
                    Password = controller.Password,
                });

                // Store selected save slot
                CursedWordsArchipelago.SaveSlot = slotIndex;
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

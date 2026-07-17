using HarmonyLib;
using Mod.Helpers;
using Mod.Mappings;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveManager))]
    internal class SaveManager_Patches : PatchBase
    {
        /// <summary>
        /// Override unlocked characters list with only those that have been received from the multiworld.
        /// </summary>
        //[HarmonyPatch(nameof(SaveManager.GetUnlockedCharacters))]
        //[HarmonyPrefix]
        public static bool OnGetUnlockedCharacters_Prefix(ref List<Type> __result)
        {
            Logger.LogInfo($"{nameof(SaveManager)}.{nameof(SaveManager.GetUnlockedCharacters)} prefix!");

            __result = CursedWordsArchipelago.Instance.CharacterTypeCache
                .Where(kvp => ArchipelagoHelper.HasReceivedItem(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            return false;
        }

        /// <summary>
        /// Override unlocked items list with only those that have been received from the multiworld.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.GetUnlockedItems))]
        [HarmonyPrefix]
        public static bool OnGetUnlockedItems_Prefix(ref List<Type> __result)
        {
            Logger.LogInfo($"{nameof(SaveManager)}.{nameof(SaveManager.GetUnlockedItems)} prefix!");

            __result = CursedWordsArchipelago.Instance.ItemTypeCache
                .Where(kvp => ArchipelagoHelper.HasReceivedItem(kvp.Value.name))
                .Where(kvp =>
                {
                    // If item rarities shuffled, exclude items of rarities not yet available
                    if (ArchipelagoHelper.SlotData.ShuffleItemRarities)
                    {
                        switch (kvp.Value.rarity)
                        {
                            case ItemRarity.Rare:
                                return ArchipelagoHelper.HasReceivedItem("Progressive Item Rarity", 1);

                            case ItemRarity.Legendary:
                                return ArchipelagoHelper.HasReceivedItem("Progressive Item Rarity", 2);

                            default:
                                return true;
                        }
                    }

                    return true;
                })
                .Select(kvp => kvp.Key)
                .ToList();

            return false;
        }

        /// <summary>
        /// Override character unlock check with a check against items received from the multiworld.
        /// </summary>
        //[HarmonyPatch(nameof(SaveManager.IsCharacterUnlocked), typeof(Type))]
        //[HarmonyPrefix]
        public static bool OnIsCharacterUnlocked_Prefix(ref bool __result, Type type)
        {
            Logger.LogInfo($"{nameof(SaveManager)}.{nameof(SaveManager.GetUnlockedCharacters)} prefix!");

            // If at the save slot scene, pull from the save (as we're not connected to AP at this point and we are adding the unlocked characters o the save)
            if (SceneManager.GetActiveScene().name == SceneNames.SaveSlotsScene)
            {

            }

            // Check if character exists in character type cache and if it has been received from the multiworld
            __result = CursedWordsArchipelago.Instance.CharacterTypeCache.TryGetValue(type, out string characterName)
                && ArchipelagoHelper.HasReceivedItem(characterName);

            return false;
        }

        /// <summary>
        /// Override bulk unlocks with custom ones.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.UnlockBulkUnlock))]
        [HarmonyPrefix]
        public static bool OnUnlockBulkUnlock(BulkUnlock bulkUnlock)
        {
            Logger.LogInfo($"{nameof(SaveManager)}.{nameof(SaveManager.UnlockBulkUnlock)} prefix!");

            bool result = Lookups.ValidBulkUnlockTypes.Contains(bulkUnlock.GetType());

            if (!result)
            {
                Logger.LogInfo($"Ignoring bulk unlock of type {bulkUnlock.Name}");
            }

            return result;
        }

        /// <summary>
        /// Re-direct save path to archipelago-specific save file.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.GetSaveFileName))]
        [HarmonyPrefix]
        public static bool GetSaveFileName_Prefix(int slotIndex, ref string __result)
        {
            __result = $"{GameStatics.SaveDirectory}/slot_{((slotIndex == 0) ? GameStatics.SaveSlot : slotIndex)}_archipelago.sav";
            return false;
        }

        /// <summary>
        /// Reset archipelago data when a save is reset.
        /// </summary>
        [HarmonyPatch(nameof(SaveManager.ResetSave))]
        [HarmonyPostfix]
        public static void ResetSave_Postfix(int slotIndex)
        {
            ArchipelagoData.ResetDataForSaveSlot(slotIndex);
        }
    }
}

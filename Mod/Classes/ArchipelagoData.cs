using FullSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Helpers
{
    public class ArchipelagoData
    {
        /// <summary>
        /// The archipelago host URL.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The archipelago slot name.
        /// </summary>
        public string Slot { get; set; }

        /// <summary>
        /// The (optional) archipelago password.
        /// </summary>
        public string Password { get; set; }

        public ArchipelagoData() { }

        /// <summary>
        /// Get archipelago data for slot index.
        /// </summary>
        /// <param name="saveSlotIndex">The slot index to get the archipelago data for.</param>
        /// <returns>Populated <see cref="ArchipelagoData"/> object.</returns>
        public static ArchipelagoData GetDataForSaveSlot(int saveSlotIndex)
        {
            string savePath = GetPathForSaveSlot(saveSlotIndex);
            if (!File.Exists(savePath))
            {
                // If no file for this slot exists, create new
                SaveDataForSaveSlot(saveSlotIndex, new ArchipelagoData());
            }

            // Read and parse stored data
            string content = File.ReadAllText(savePath);
            return StringSerializer.Deserialize(typeof(ArchipelagoData), content) as ArchipelagoData;
        }

        /// <summary>
        /// Get the full save path for archipelago data.
        /// </summary>
        /// <param name="saveSlotIndex">The save slot index to retrieve the archipelago data for.</param>
        /// <returns>Path to the archipelago data file.</returns>
        public static string GetPathForSaveSlot(int saveSlotIndex)
        {
            return Path.Combine(
                Application.persistentDataPath,
                $"{GameStatics.SaveDirectory}/slot_{((saveSlotIndex == 0) ? GameStatics.SaveSlot : saveSlotIndex)}_archipelago_data.json");
        }

        /// <summary>
        /// Reset archipelago data for a save slot.
        /// </summary>
        /// <param name="saveSlotIndex">The save slot index to reset the data for.</param>
        public static void ResetDataForSaveSlot(int saveSlotIndex)
        {
            SaveDataForSaveSlot(saveSlotIndex, new ArchipelagoData());
        }

        /// <summary>
        /// Save archipelago data for a save slot.
        /// </summary>
        /// <param name="saveSlotIndex">The save slot index to store the data for.</param>
        /// <param name="data">The archipelago data to be saved.</param>
        public static void SaveDataForSaveSlot(int saveSlotIndex, ArchipelagoData data)
        {
            string savePath = GetPathForSaveSlot(saveSlotIndex);
            string contents = StringSerializer.Serialize(typeof(ArchipelagoData), data);
            File.WriteAllText(savePath, contents);
        }
    }
}

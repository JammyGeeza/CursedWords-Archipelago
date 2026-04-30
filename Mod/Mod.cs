using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;
using Mod.Helpers;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Modd
{
    [BepInPlugin("archipelago", "Cursed Words Archipelago", "0.0.0")]
    public class CursedWorldsArchipelago : BaseUnityPlugin
    {
        public static CursedWorldsArchipelago Instance = null;
        private static Harmony Harmony = null;

        private void Awake()
        {
            Logger.LogInfo("Success, the mod has loaded!");

            // Initialize config
            ConfigHelper.Initialize(Config, Logger);

            // Initialize Harmony
            Logger.LogInfo("Applying patches...");

            HarmonyFileLog.Enabled = true;
            Harmony = new Harmony("archipelago");
            Harmony.PatchAll();

            Logger.LogInfo("Patches applied");

            // Set instance
            Instance = this;
        }

        public async Task<bool> TryLoginAsync()
        {
            // Attempt to create archipelago session
            return await ArchipelagoHelper.ConnectAsync(
                ConfigHelper.Host.Value,
                ConfigHelper.SlotName.Value,
                ConfigHelper.Password.Value);
        }
    }
}

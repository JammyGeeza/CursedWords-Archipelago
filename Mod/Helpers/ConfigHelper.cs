using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Mod.Helpers
{
    internal static class ConfigHelper
    {
        private static ConfigFile Config = null;
        private static ManualLogSource Logger = null;

        // Connection config
        public static ConfigEntry<string> Host = null;
        public static ConfigEntry<string> SlotName = null;
        public static ConfigEntry<string> Password = null;

        public static void Initialize(ConfigFile config, ManualLogSource logger)
        {
            Config = config;
            Logger = logger;

            Host = Config.Bind("Connection", "HostName", "localhost:38281", new ConfigDescription("The full URL of the archipelago multiworld"));
            SlotName = Config.Bind("Connection", "SlotName", "Player1", new ConfigDescription("The name of the player slot to connect as"));
            Password = Config.Bind("Connection", "Password", "", new ConfigDescription("The (optional) password to connect to the multiworld"));

            Logger.LogInfo($"Configuration initialized");
        }
    }
}

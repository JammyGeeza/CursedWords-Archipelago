using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using BepInEx.Logging;
using Modd;

namespace Mod.Extensions
{
    public static class ItemExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Check if this item is a Pin.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if yes, False if no</returns>
        public static bool IsPin(this Item item)
        {
            return item.UpgradeableComponents.Count > 1;
        }

        /// <summary>
        /// Check if this item has at least one component with a level greater than or equal to a specified level.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <param name="level">The level to check against</param>
        /// <returns>True if yes, False if no</returns>
        public static bool IsLevelOrHigher(this Item item, int level)
        {
            return item.UpgradeableComponents
                .Any(uc => uc.Level >= level);
        }

        /// <summary>
        /// Check if this item's specific component has a level greater than or equal to a specified level.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <param name="level">The level to check against</param>
        /// <returns>True if yes, False if no</returns>
        public static bool IsComponentLevelOrHigher(this Item item, int componentIndex, int level)
        {
            try
            {
                return item.UpgradeableComponents[componentIndex].Level >= level;
            }
            catch
            {
                return false;
            }
        }
    }
}

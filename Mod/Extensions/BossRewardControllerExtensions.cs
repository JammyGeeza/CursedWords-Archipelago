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
    public static class BossRewardControllerExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Get the current character's pin item.
        /// </summary>
        /// <param name="controller">The controller to get the pin item from</param>
        /// <returns>The current character's pin item.</returns>
        public static Item GetPinItem(this BossRewardController controller)
        {
            return Traverse.Create(controller)
                .Field("_characterItem")
                .GetValue<Item>();
        }
    }
}

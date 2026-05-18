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
    public static class GridLayoutControllerExtensions
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        /// <summary>
        /// Get the tile objects.
        /// </summary>
        /// <returns>The current set of tile objects.</returns>
        public static TileObject[] GetTileObjects(this GridLayoutController controller)
        {
            return Traverse.Create(controller)
                .Field("_tileObjects")
                .GetValue<TileObject[]>();
        }
    }
}

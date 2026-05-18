using HarmonyLib;
using Mod.Helpers;
using Modd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(GridLayoutController))]
    internal class GridLayoutController_Patches : PatchBase
    {
        /// <summary>
        /// When a consumable tile is applied, attempt to check the location.
        /// </summary>
        [HarmonyPatch(nameof(GridLayoutController.ApplyConsumableTile))]
        [HarmonyPostfix]
        public static void OnApplyConsumableTile_Postfix(GridLayoutController __instance)
        {
            Logger.LogInfo($"{nameof(GridLayoutController)}.{nameof(GridLayoutController.ApplyConsumableTile)} postfix!");

            // Check the 'Use a Consumable Tile' location
            CursedWordsArchipelago.Instance.TryCheckGenericLocations("place_tile");
        }

        /// <summary>
        /// When a grid is generated, hide tiles if not yet unlocked.
        /// </summary>
        [HarmonyPatch(nameof(GridLayoutController.GenerateGrid))]
        [HarmonyPostfix]
        public static void OnGenerateGrid_Postfix(GridLayoutController __instance)
        {
            Logger.LogInfo($"{nameof(GridLayoutController)}.{nameof(GridLayoutController.ApplyConsumableTile)} postfix!");

            int receivedTilePositions = ArchipelagoHelper.AmountOfItemReceived("Progressive Tile Position");
            for (int i = receivedTilePositions; i < ArchipelagoHelper.SlotData.ProgressiveTilePositions.Count; i++)
            {
                (int x, int y) coordinate = ArchipelagoHelper.SlotData.ProgressiveTilePositions[i];

                try
                {
                    TileObject tile = __instance.GetTileObjects()
                        .First(t => t.GridCoordinate == new Vector2Int { x = coordinate.x, y = coordinate.y });

                    Logger.LogInfo($"Locking tile at co-ordinate {coordinate.x},{coordinate.y}");

                    GameObject tileGO = Traverse.Create(tile)
                        .Field("_tileGO")
                        .GetValue<GameObject>();

                    // Hide the tile
                    tileGO.SetActive(false);
                }
                catch
                {
                    // Ignore, tile may not exist due to Progressive Grid Size
                }
            }
        }
    }
}

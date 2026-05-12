using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(Tile))]
    internal class Tile_Patches : PatchBase
    {
        /// <summary>
        /// Prevent tiles with specific glyph types appearing if not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetGlyphType))]
        [HarmonyPrefix]
        public static void SetGlyphType_Prefix(ref GlyphType glyphType)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetGlyphType)} prefix!");

            switch (glyphType)
            {
                case GlyphType.Blank:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Blank Tiles") ? glyphType : GlyphType.Letter;
                    break;

                case GlyphType.Chess:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Chess Tiles") ? glyphType : GlyphType.Letter;
                    break;

                case GlyphType.Currency:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Currency Tiles") ? glyphType : GlyphType.Letter;
                    break;

                case GlyphType.Fraction:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Fraction Tiles") ? glyphType : GlyphType.Letter;
                    break;

                case GlyphType.Number:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Number Tiles") ? glyphType : GlyphType.Letter;
                    break;

                 // Add other types here, when implemented
            }
        }

        /// <summary>
        /// Prevent tiles with specific colour types appearing if not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetTileType))]
        [HarmonyPrefix]
        public static void SetTileType_Prefix(ref TileType tileType)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetTileType)} prefix!");

            switch (tileType)
            {
                case TileType.Blue:
                    tileType = ArchipelagoHelper.HasReceivedItem("Blue Tiles") ? tileType : TileType.Normal;
                    break;

                case TileType.Shiny:
                    tileType = ArchipelagoHelper.HasReceivedItem("Shiny Tiles") ? tileType : TileType.Normal;
                    break;

                case TileType.Red:
                    tileType = ArchipelagoHelper.HasReceivedItem("Red Tiles") ? tileType : TileType.Normal;
                    break;

                case TileType.Void:
                    tileType = ArchipelagoHelper.HasReceivedItem("Void Tiles") ? tileType : TileType.Normal;
                    break;

                // Add other types here, when implemented
            }
        }
    }
}

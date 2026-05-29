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
        /// Prevent chess piece being set if tile type not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetChessPiece), typeof(ChessPiece), typeof(bool))]
        [HarmonyPrefix]
        public static bool OnSetChessPiece_Prefix(Tile __instance, ref ChessPiece piece, ref bool isWhite)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetChessPiece)} prefix!");
            return ArchipelagoHelper.HasReceivedItem("Chess Tiles");
        }

        /// <summary>
        /// Prevent currency being set if tile type not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetCurrency))]
        [HarmonyPrefix]
        public static bool OnSetCurrency_Prefix(Tile __instance, string currency)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetCurrency)} prefix!");
            return ArchipelagoHelper.HasReceivedItem("Currency Tiles");
        }

        /// <summary>
        /// Prevent fraction being set if tile type not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetFractionNumbers))]
        [HarmonyPrefix]
        public static bool OnSetFractionNumbers_Prefix(Tile __instance, List<int> fractionNumbers)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetCurrency)} prefix!");
            return ArchipelagoHelper.HasReceivedItem("Fraction Tiles");
        }

        /// <summary>
        /// Prevent tiles with specific glyph types appearing if not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetGlyphType))]
        [HarmonyPrefix]
        public static void SetGlyphType_Prefix(Tile __instance, ref GlyphType glyphType)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetGlyphType)} prefix!");
            Logger.LogInfo($"Input glyph type: {glyphType}");

            GlyphType inputGlyphType = glyphType;

            switch (glyphType)
            {
                case GlyphType.Arrow:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Arrow Tiles") ? glyphType : GlyphType.Letter;
                    break;

                case GlyphType.BespokeCard:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Card Tiles") ? glyphType : GlyphType.Letter;
                    break;

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

                case GlyphType.ScatteredItem:
                    glyphType = ArchipelagoHelper.HasReceivedItem("Scattered Tiles") ? glyphType : GlyphType.Letter;
                    break;

                // Add other types here, when implemented
            }

            // If glyph type was replaced with Letter type, generate letter
            if (inputGlyphType != glyphType && glyphType == GlyphType.Letter)
            {
                // Generate new letter if it doesn't have one
                if (string.IsNullOrEmpty(__instance.Letter) || !Vocabulary.ActiveLanguageVocabulary.LanguageAlphabet.AllLetters.Contains(__instance.Letter))
                {
                    __instance.SetLetter(Vocabulary.ActiveLanguageVocabulary.LanguageAlphabet.GetRandomLetterWeighted());
                }

                // Wipe anything else
                __instance.CactusGrowth = new ScorePacket(0L);
                __instance.CardSuit = Suit.None;
                __instance.FractionNumbers = new List<int> { };
                __instance.Number = -1000;
                __instance.PieceType = ChessPiece.None;
            }

            Logger.LogInfo($"Output glyph type: {glyphType}");
        }

        /// <summary>
        /// Prevent number being set if tile type not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetNumber))]
        [HarmonyPrefix]
        public static bool OnSetNumber_Prefix(Tile __instance, int number)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetCurrency)} prefix!");
            return ArchipelagoHelper.HasReceivedItem("Number Tiles");
        }

        /// <summary>
        /// Prevent tiles with suits appearing if not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetSuit))]
        [HarmonyPrefix]
        public static void OnSetSuit_Prefix(Tile __instance, ref Suit suit)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetSuit)} prefix!");
            Logger.LogInfo($"Input tile suit: {suit}");

            if (!ArchipelagoHelper.HasReceivedItem("Card Tiles"))
            {
                suit = Suit.None;
            }

            Logger.LogInfo($"Output tile suit: {suit}");
        }

        /// <summary>
        /// Prevent tiles with specific colour types appearing if not yet received.
        /// </summary>
        [HarmonyPatch(nameof(Tile.SetTileType))]
        [HarmonyPrefix]
        public static void SetTileType_Prefix(Tile __instance, ref TileType tileType)
        {
            Logger.LogInfo($"{nameof(Tile)}.{nameof(Tile.SetTileType)} prefix!");
            Logger.LogInfo($"Input tile type: {tileType}");

            // A bunch of tile types are incompatible with blank glyph type - stick to normal to be safe
            if (__instance.GetGlyphType() == GlyphType.Blank)
            {
                tileType = TileType.Normal;
            }
            else
            {
                switch (tileType)
                {
                    case TileType.Blue:
                        tileType = ArchipelagoHelper.HasReceivedItem("Blue Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Cactus:
                        tileType = ArchipelagoHelper.HasReceivedItem("Cactus Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Glitch:
                        tileType = ArchipelagoHelper.HasReceivedItem("Glitch Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Gold:
                        tileType = ArchipelagoHelper.HasReceivedItem("Gold Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Green:
                        tileType = ArchipelagoHelper.HasReceivedItem("Green Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Pink:
                        tileType = ArchipelagoHelper.HasReceivedItem("Pink Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Purple:
                        tileType = ArchipelagoHelper.HasReceivedItem("Purple Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Red:
                        tileType = ArchipelagoHelper.HasReceivedItem("Red Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Shiny:
                        tileType = ArchipelagoHelper.HasReceivedItem("Shiny Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.Void:
                        tileType = ArchipelagoHelper.HasReceivedItem("Void Tiles") ? tileType : TileType.Normal;
                        break;

                    case TileType.White:
                        tileType = ArchipelagoHelper.HasReceivedItem("White Tiles") ? tileType : TileType.Normal;
                        break;

                    // Add other types here, when implemented
                }
            }

            Logger.LogInfo($"Output tile type: {tileType}");
        }
    }
}

using HarmonyLib;
using Mod.Extensions;
using Mod.Helpers;
using Mod.Mappings;
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
        /// Override random curses to ensure all supported curse types are selected from.
        /// (Items like Amphora would only produce Blank or Currency tiles)
        /// </summary>
        [HarmonyPatch(nameof(Tile.RandomlyCurseTile))]
        [HarmonyPrefix]
        private static bool RandomlyCurseTile_Prefix(ref Tile tile, bool isAllowingScatteredItems)
        {
            Logger.LogDebug($"{nameof(Tile)}.{nameof(Tile.RandomlyCurseTile)} prefix!");

            // Get supported curse types (excluding scattered item if not allowing)
            List<GlyphType> curseTypes = SupportedMappings.CurseTypes
                .Where(ct => isAllowingScatteredItems || ct != GlyphType.ScatteredItem)
                .ToList();

            // Select random supported curse type and apply it
            GlyphType curseType = curseTypes.GetRandom();
            switch (curseType)
            {
                case GlyphType.BespokeCard:
                    {
                        tile.SetGlyphType(curseType);

                        if (UnityEngine.Random.Range(0, 10) == 0)
                        {
                            tile.SetSuit(Suit.Joker);
                        }
                        else
                        {
                            tile.SetToRandomLetter();
                        }
                    }
                    break;

                case GlyphType.Blank:
                    tile.SetGlyphType(curseType);
                    break;

                case GlyphType.Chess:
                    tile.SetChessPiece(ChessPieces.GetRandomChessPiece());
                    break;

                case GlyphType.Currency:
                    tile.SetLetter(Currency.GetRandomCurrency());
                    tile.SetGlyphType(curseType);
                    break;

                case GlyphType.Fraction:
                    string randomFraction = Alphabet.GetRandomFraction();
                    tile.SetFractionNumbers(Alphabet.GetFractionNumbers(randomFraction));
                    break;

                case GlyphType.Number:
                    tile.SetNumber(UnityEngine.Random.Range(1, 9));
                    break;

                case GlyphType.ScatteredItem:
                    tile.SetScatteredItem(ScatteredItemPools.GetRandomItem());
                    break;

                default:
                    Logger.LogWarning($"Random curse type: {curseType} is not currently supported, defaulting to letter.");
                    tile.SetToRandomLetter();
                    break;
            }

            // if not joker, apply a random suit
            if (tile.GetSuit() is Suit.None)
            {
                tile.SetSuit(PlayingCardUtility.GetRandomCardSuit());
            }

            return false;
        }
    }
}

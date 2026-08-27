using System;
using System.Collections.Generic;
using System.Text;

namespace Mod.Mappings
{
    public static class SupportedMappings
    {
        public static readonly List<GlyphType> CurseTypes = new()
        {
            GlyphType.Blank,
            GlyphType.Chess,
            GlyphType.Currency,
            GlyphType.Number,
            GlyphType.Fraction,
            GlyphType.BespokeCard,
            GlyphType.ScatteredItem
        };

        public static readonly List<TileType> ColourTypes = new()
        {
            TileType.Blue,
            TileType.Normal,
            TileType.Red,
            TileType.Shiny,
            TileType.Void,
        };
    }
}

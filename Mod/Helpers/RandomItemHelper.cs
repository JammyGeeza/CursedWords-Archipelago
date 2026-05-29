using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Helpers
{
    public static class RandomItemHelper
    {
        public static Tile GenerateRandomLetterTile()
        {
            Tile tile = new Tile();
            tile.SetTileType(TileType.Normal);
            tile.SetGlyphType(GlyphType.Letter);
            tile.SetLetter(Vocabulary.ActiveLanguageVocabulary.LanguageAlphabet.GetRandomLetterWeighted());

            return tile;
        }
    }
}

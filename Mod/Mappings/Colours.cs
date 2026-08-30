using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using System.Collections.Generic;

namespace Mod.Mappings
{
    public static class Colours
    {
        private static readonly Dictionary<Color, string> ColourCodes = new()
        {
            { Color.Black, "000000" },
            { Color.Blue, "6495ED" },
            { Color.Cyan, "00EEEE" },
            { Color.Green, "00FF7F" },
            { Color.Magenta, "EE00EE" },
            { Color.Plum, "AF99EF" },
            { Color.Red, "EE0000" },
            { Color.SlateBlue, "6D8BE8" },
            { Color.Salmon, "FA8072" },
            { Color.White, "FFFFFF" },
            { Color.Yellow, "FAFAD2" },
        };

        private static readonly Dictionary<ItemFlags, string> ItemFlagColourCodes = new()
        {
            { ItemFlags.None, GetColourHex(Color.Cyan) },
            { ItemFlags.Trap, GetColourHex(Color.Salmon) },
            { ItemFlags.NeverExclude, GetColourHex(Color.SlateBlue) },
            { ItemFlags.Advancement, GetColourHex(Color.Plum) },
            { ItemFlags.Advancement | ItemFlags.NeverExclude, ColourCodes[Color.Plum] }
        };

        /// <summary>
        /// Get the colour hex code for an archipelago color.
        /// </summary>
        /// <param name="colour">The color to get the hex string for.</param>
        /// <returns>The colour hex string or 'FFFFFF' if not found.</returns>
        public static string GetColourHex(Color colour)
        {
            if (colour == null)
            {
                return "FFFFFF";
            }

            return ColourCodes.GetValueOrDefault(colour, "FFFFFF");
        }

        /// <summary>
        /// Get the colour hex for a specified item flag combination.
        /// </summary>
        /// <param name="flag">The item flag to get the hex colour for.</param>
        /// <returns>The hex colour string or '#FFFFFF' if not found.</returns>
        public static string GetColourForItemFlag(ItemFlags flag)
        {
            return ItemFlagColourCodes.GetValueOrDefault(flag, GetColourHex(Color.White));
        }
    }
}

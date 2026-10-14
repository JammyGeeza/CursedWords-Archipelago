using Mod.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mod.Mappings
{
    internal static class Crowns
    {
        public static readonly Dictionary<int, string> CrownNames = new()
        {
            { -1, "None" },
            { 0, "None" },
            { 1, "Purple" },
            { 2, "Yellow" },
            { 3, "Orange" },
            { 4, "Pink" },
            { 5, "Green" },
            { 6, "Blue" },
            { 7, "Red" },
        };

        /// <summary>
        /// Get the name for a crown.
        /// </summary>
        /// <param name="crown">The crown to get the name for.</param>
        /// <returns>The goal type name or 'Unknown' if not found.</returns>
        public static string GetCrowmnName(int crown)
        {
            return CrownNames.GetValueOrDefault(crown, "Unknown");
        }
    }
}

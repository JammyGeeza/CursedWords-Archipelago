using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Mappings
{
    public static class Lookups
    {
        /// <summary>
        /// Lookup character types by character name.
        /// </summary>
        public static readonly Dictionary<string, Type> CharacterNameToType = new Dictionary<string, Type>()
        {
            { "Rodman", typeof(WetDennis) },
            { "Nina Nix", typeof(NinaNix) },
            { "Hayley Bayles", typeof(HayleyBayles) },

            // TODO: Add the rest here
        };
    }
}

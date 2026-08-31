using System;
using System.Collections.Generic;
using System.Text;

namespace Mod.Classes
{
    public class LocationCriteria
    {
        /// <summary>
        /// Gets or sets the criteria for a Generic action to check this location.
        /// </summary>
        public Func<string, bool> OnGenericAction { get; set; } = (action) => false;

        /// <summary>
        /// Gets or sets the criteria for an Encounter action to check this location.
        /// </summary>
        public Func<string, Player, List<BossModifier>, object, bool> OnEncounterAction { get; set; } = (action, player, modifiers, args) => false;

        /// <summary>
        /// Gets or sets the criteria for an Item action to check this location.
        /// </summary>
        public Func<string, Item, bool> OnItemAction { get; set; } = (action, item) => false;

        /// <summary>
        /// Gets or sets the criteria for a Numeric action to check this location.
        /// </summary>
        public Func<string, long, bool> OnNumericAction { get; set; } = (action, value) => false;

        /// <summary>
        /// Gets or sers the criteria for a Tile action to check this location.
        /// </summary>
        public Func<string, Tile, bool> OnTileAction { get; set; } = (action, tile) => false;

        /// <summary>
        /// Gets or sets the name of the Location.
        /// </summary>
        public string LocationName { get; set; }

        public LocationCriteria(string locationName)
        {
            LocationName = locationName;
        }
    }
}

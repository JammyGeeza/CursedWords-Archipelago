using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Mod.Enums;
using System.Collections.Generic;

namespace Mod.Mappings
{
    public static class Goals
    {
        private static readonly Dictionary<GoalType, string> GoalNames = new()
        {
            { GoalType.Crowns, "Crown" },
            { GoalType.Michael, "Michael" },
            { GoalType.Runs, "Run" },
        };

        /// <summary>
        /// Get the name for a goal type.
        /// </summary>
        /// <param name="type">The goal type to get the name for.</param>
        /// <returns>The goal type name or 'Unknown' if not found.</returns>
        public static string GetGoalName(GoalType type)
        {
            return GoalNames.GetValueOrDefault(type, "Unknown");
        }
    }
}

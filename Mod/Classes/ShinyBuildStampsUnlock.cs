using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class ShinyBuildStampsUnlock : BulkUnlock
    {
        public ShinyBuildStampsUnlock()
        {
            Name = "Shiny Stamps";
            PreItemTextStrings = new List<string> { "You received Shiny Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Shiny Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.ShinyBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

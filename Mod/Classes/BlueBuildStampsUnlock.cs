using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class BlueBuildStampsUnlock : BulkUnlock
    {
        public BlueBuildStampsUnlock()
        {
            Name = "Blue Stamps";
            PreItemTextStrings = new List<string> { "You received Blue Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Blue Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.BlueBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

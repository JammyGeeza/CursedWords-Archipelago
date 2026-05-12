using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class RedBuildStampsUnlock : BulkUnlock
    {
        public RedBuildStampsUnlock()
        {
            Name = "Red Stamps";
            PreItemTextStrings = new List<string> { "You received Red Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Red Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.RedBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

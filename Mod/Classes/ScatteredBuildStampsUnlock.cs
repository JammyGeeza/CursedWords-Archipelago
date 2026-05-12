using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class ScatteredBuildStampsUnlock : BulkUnlock
    {
        public ScatteredBuildStampsUnlock()
        {
            Name = "Scattered Stamps";
            PreItemTextStrings = new List<string> { "You received Scattered Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Scattered Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.ScatteredBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

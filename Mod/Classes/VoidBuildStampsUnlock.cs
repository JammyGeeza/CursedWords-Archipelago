using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class VoidBuildStampsUnlock : BulkUnlock
    {
        public VoidBuildStampsUnlock()
        {
            Name = "Void Stamps";
            PreItemTextStrings = new List<string> { "You received Void Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Void Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.VoidBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

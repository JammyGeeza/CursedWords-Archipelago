using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class CurseBuildStampsUnlock : BulkUnlock
    {
        public CurseBuildStampsUnlock()
        {
            Name = "Curse Stamps";
            PreItemTextStrings = new List<string> { "You received Curse Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Curse Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.CurseBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

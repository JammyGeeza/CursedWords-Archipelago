using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class ChessBuildStampsUnlock : BulkUnlock
    {
        public ChessBuildStampsUnlock()
        {
            Name = "Chess Stamps";
            PreItemTextStrings = new List<string> { "You received Chess Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Chess Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.ChessBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class ScatteredBuildStickersUnlock : BulkUnlock
    {
        public ScatteredBuildStickersUnlock()
        {
            Name = "Scattered Stickers";
            PreItemTextStrings = new List<string> { "You received Scattered Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Scattered Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.ScatteredBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

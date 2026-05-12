using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class RedBuildStickersUnlock : BulkUnlock
    {
        public RedBuildStickersUnlock()
        {
            Name = "Red Stickers";
            PreItemTextStrings = new List<string> { "You received Red Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Red Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.RedBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class BlueBuildStickersUnlock : BulkUnlock
    {
        public BlueBuildStickersUnlock()
        {
            Name = "Blue Stickers";
            PreItemTextStrings = new List<string> { "You received Blue Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Blue Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.BlueBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

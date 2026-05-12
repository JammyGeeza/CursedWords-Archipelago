using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class CurseBuildStickersUnlock : BulkUnlock
    {
        public CurseBuildStickersUnlock()
        {
            Name = "Curse Stickers";
            PreItemTextStrings = new List<string> { "You received Curse Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Curse Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.CurseBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

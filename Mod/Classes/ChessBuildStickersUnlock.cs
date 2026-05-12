using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class ChessBuildStickersUnlock : BulkUnlock
    {
        public ChessBuildStickersUnlock()
        {
            Name = "Chess Stickers";
            PreItemTextStrings = new List<string> { "You received Chess Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Chess Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.ChessBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class VoidBuildStickersUnlock : BulkUnlock
    {
        public VoidBuildStickersUnlock()
        {
            Name = "Void Stickers";
            PreItemTextStrings = new List<string> { "You received Void Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Void Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.VoidBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

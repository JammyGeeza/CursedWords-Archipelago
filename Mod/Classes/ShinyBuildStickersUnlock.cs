using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class ShinyBuildStickersUnlock : BulkUnlock
    {
        public ShinyBuildStickersUnlock()
        {
            Name = "Shiny Stickers";
            PreItemTextStrings = new List<string> { "You received Shiny Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Shiny Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.ShinyBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

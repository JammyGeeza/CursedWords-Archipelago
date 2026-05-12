using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class CardsBuildStickersUnlock : BulkUnlock
    {
        public CardsBuildStickersUnlock()
        {
            Name = "Cards Stickers";
            PreItemTextStrings = new List<string> { "You received Cards Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Cards Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.CardsBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

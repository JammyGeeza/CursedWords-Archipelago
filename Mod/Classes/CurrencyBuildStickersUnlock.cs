using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class CurrencyBuildStickersUnlock : BulkUnlock
    {
        public CurrencyBuildStickersUnlock()
        {
            Name = "Currency Stickers";
            PreItemTextStrings = new List<string> { "You received Currency Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Currency Stickers' Archipelago item to unlock";
            ItemsToUnlock = Lookups.CurrencyBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

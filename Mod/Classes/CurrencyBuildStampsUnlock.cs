using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class CurrencyBuildStampsUnlock : BulkUnlock
    {
        public CurrencyBuildStampsUnlock()
        {
            Name = "Currency Stamps";
            PreItemTextStrings = new List<string> { "You received Currency Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Currency Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.CurrencyBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

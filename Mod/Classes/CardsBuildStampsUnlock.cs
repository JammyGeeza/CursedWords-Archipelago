using Mod.Mappings;
using System;
using System.Collections.Generic;

namespace Mod.Classes
{
    public class CardsBuildStampsUnlock : BulkUnlock
    {
        public CardsBuildStampsUnlock()
        {
            Name = "Cards Stamps";
            PreItemTextStrings = new List<string> { "You received Cards Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Cards Stamps' Archipelago item to unlock";
            ItemsToUnlock = Lookups.CardsBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

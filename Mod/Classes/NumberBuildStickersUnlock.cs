using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Classes
{
    public class NumberBuildStickersUnlock : BulkUnlock
    {
        public NumberBuildStickersUnlock()
        {
            Name = "Number Stickers";
            PreItemTextStrings = new List<string> { "You received Number Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Number Stickers' AP item to unlock";
            ItemsToUnlock = Lookups.NumberBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

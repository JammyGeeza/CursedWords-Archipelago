using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Classes
{
    public class RainbowBuildStickersUnlock : BulkUnlock
    {
        public RainbowBuildStickersUnlock()
        {
            Name = "Rainbow Stickers";
            PreItemTextStrings = new List<string> { "You received Rainbow Stickers" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Rainbow Stickers' AP item to unlock";
            ItemsToUnlock = Lookups.RainbowBuildStickers;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

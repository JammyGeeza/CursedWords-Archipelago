using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Classes
{
    public class RainbowBuildStampsUnlock : BulkUnlock
    {
        public RainbowBuildStampsUnlock()
        {
            Name = "Rainbow Stamps";
            PreItemTextStrings = new List<string> { "You received Rainbow Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Rainbow Stamps' AP item to unlock";
            ItemsToUnlock = Lookups.RainbowBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

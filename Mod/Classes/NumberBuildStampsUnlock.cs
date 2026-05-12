using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Classes
{
    public class NumberBuildStampsUnlock : BulkUnlock
    {
        public NumberBuildStampsUnlock()
        {
            Name = "Number Stamps";
            PreItemTextStrings = new List<string> { "You received Number Stamps" };
            FeatureTextStrings = new List<string> { };
            CollectionItemUnlockText = "??";
            CollectionItemUnlockText = "Receive the 'Number Stamps' AP item to unlock";
            ItemsToUnlock = Lookups.NumberBuildStamps;
            CharactersToUnlock = new List<Type> { };
            BossModifiersToUnlock = new List<Type> { };
            ChallengeRunsToUnlock = new List<Type> { };
        }
    }
}

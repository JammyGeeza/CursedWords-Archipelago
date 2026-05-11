using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Classes
{
    public class APStampPadlock : StampPadlock
    {
        public APStampPadlock() : base()
        {
            IsSellingPrevented = true;
        }

        public override void Upgrade(int componentIndex, bool upgradingBoth = false)
        {
        }

        public override void Downgrade(int componentIndex)
        {
        }

        public override string GetDescription()
        {
            return "Removed by receiving the 'Progressive Stamp Slot' item";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Classes
{
    public class APStickerPadlock : Item
    {
        public APStickerPadlock()
        {
            Name = "Slot Locked";
            SpriteData.Add(new ItemSpriteData(ItemSpriteUsage.Default, "Padlock"));
            UpgradeableComponents = new List<UpgradeableComponent>
            {
                new UpgradeableComponent(1, 8, 8)
            };
            Rarity = ItemRarity.Unique;
            Cost = 0;
            SellCost = 0;
            IsSellingPrevented = true;
            CostsMoneyToSell = false;
            ItemFunctionTags = new List<ItemFunctionTag> { ItemFunctionTag.Tech };
        }

        public override void Upgrade(int componentIndex, bool upgradingBoth = false)
        {
        }

        public override void Downgrade(int componentIndex)
        {
        }

        public override string GetDescription()
        {
            return "Removed by receiving the 'Progressive Sticker Slot' item";
        }
    }
}

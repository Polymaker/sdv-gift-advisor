using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor
{
	public abstract class ItemGivingAction
	{
		public StardewValley.Object Item { get; set; }
		public NPC TargetNPC { get; set; }
		public abstract bool IsGift { get; }
		public bool IsQuest => !IsGift;
        public bool IsWithinRange { get; set; }

		public ItemGivingAction(StardewValley.Object item, NPC target)
		{
			Item = item;
			TargetNPC = target;
		}

		public virtual bool Matches(StardewValley.Object item, NPC target)
		{
			if(item != null && target != null)
			{
				return item.ParentSheetIndex == Item.ParentSheetIndex && target.Name == TargetNPC.Name;
			}
			return false;
		}

		public bool Matches(StardewValley.Object item)
		{
			return item?.ParentSheetIndex == Item.ParentSheetIndex;
		}

        public abstract string GetTooltipText();

        public virtual bool CanGiveItem()
        {
            return true;
        }
	}
}

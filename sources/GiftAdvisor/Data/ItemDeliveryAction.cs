using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Quests;

namespace GiftAdvisor
{
	public class ItemDeliveryAction : ItemGivingAction
	{
		public Quest Quest { get; private set; }
		public int Quantity { get; private set; }
		public override bool IsGift => false;

		//public ItemDeliveryAction(StardewValley.Object item, int quantity, NPC target) : base(item, target)
		//{
		//	Quantity = quantity;
		//}

		private ItemDeliveryAction(Quest quest, StardewValley.Object item, int quantity, NPC target) : base(item, target)
		{
			Quest = quest;
            
            Quantity = quantity;
		}

		public ItemDeliveryAction(ItemDeliveryQuest quest) 
			: this(quest, quest.deliveryItem.Value ?? new StardewValley.Object(quest.item.Value, 1), quest.number.Value, Game1.getCharacterFromName(quest.target.Value))
		{

		}

		public ItemDeliveryAction(LostItemQuest quest) 
			: this(quest, new StardewValley.Object(quest.itemIndex.Value, 1), 1, Game1.getCharacterFromName(quest.npcName.Value))
		{

		}

		public ItemDeliveryAction(FishingQuest quest)
			: this(quest, new StardewValley.Object(quest.whichFish.Value, 1), quest.numberToFish.Value, Game1.getCharacterFromName(quest.target.Value))
		{

		}

		public bool CanCompleteDelivery(StardewValley.Object item, NPC npc)
		{
			if (!Matches(item, npc))
				return false;

			if (Quest is FishingQuest fq)
				return fq.numberFished.Value >= Quantity;

			return item.Stack >= Quantity;
		}

        public override bool CanGiveItem()
        {
            if (Quest is FishingQuest fq)
                return fq.numberFished.Value >= Quantity;
            return base.CanGiveItem();
        }

        public override string GetTooltipText()
        {
            return $"{Quest.questTitle}: {Item.name} x{Quantity}";
        }
    }
}

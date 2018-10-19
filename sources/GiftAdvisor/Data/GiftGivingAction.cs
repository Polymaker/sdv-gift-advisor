using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace GiftAdvisor
{
	public class GiftGivingAction : ItemGivingAction
	{
		public GiftTaste Taste { get; set; }
		public int FriendshipAmount { get; set; }
		public override bool IsGift => true;

		public GiftGivingAction(StardewValley.Object item, NPC target) : base(item, target)
		{
			Taste = (GiftTaste)target.getGiftTasteForThisItem(item);
			FriendshipAmount = GiftHelper.GetGiftFriendshipValue(target, item);
		}

		public bool CanGiveGift()
		{
			var npcSocialData = Game1.player.friendshipData[TargetNPC.Name];
			return npcSocialData.GiftsToday == 0 && npcSocialData.GiftsThisWeek < 2;
		}

		public void RecalculateFriendshipAmount()
		{
			FriendshipAmount = GiftHelper.GetGiftFriendshipValue(TargetNPC, Item);
		}

        public override string GetTooltipText()
        {
			var npcSocialData = Game1.player.friendshipData[TargetNPC.Name];
			if (npcSocialData.GiftsToday > 0)
				return $"You already gave a gift to {TargetNPC.Name} today";
			else if (npcSocialData.GiftsThisWeek >= 2)
				return $"You already gave two gifts to {TargetNPC.Name} this week";

			return $"{Taste}: {FriendshipAmount:+#;-#;0} Friendship";
        }
    }
}

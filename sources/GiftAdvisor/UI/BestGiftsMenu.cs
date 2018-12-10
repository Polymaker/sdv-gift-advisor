using GiftAdvisor.Data;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor.UI
{
    public class BestGiftsMenu : IClickableMenu
    {
		private static List<GiftGivingAction> BestGiftsOfTheDay;
		//private static GameDate LastUpdated;

		static BestGiftsMenu()
		{
			BestGiftsOfTheDay = new List<GiftGivingAction>();
		}

		public BestGiftsMenu()
		{
			//CalculateBestGifts();
		}

		public BestGiftsMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
		{
			//CalculateBestGifts();
		}

		private void CalculateBestGifts()
		{
            //if(LastUpdated == GameDate.MinDate || GameDate.Today != LastUpdated)
            //{
            //	BestGiftsOfTheDay.Clear();
            //             var giftableItems = GiftAdvisorMod.InventoryTrackerModule.GetAllItems().Where(i => i.canBeGivenAsGift()).ToList();
            //	LastUpdated = GameDate.Today;
            //}
            BestGiftsOfTheDay.Clear();

            if(GiftAdvisorMod.InventoryTrackerModule != null)
            {
                var allGiftableItems = GiftAdvisorMod.InventoryTrackerModule.GetAllItems().OfType<StardewValley.Object>().Where(i => i.canBeGivenAsGift());
                var groupedItems = allGiftableItems.GroupBy(i => new { i.ParentSheetIndex, i.Quality });

                foreach (var npc in Utility.getAllCharacters())
                {
                    if (!npc.IsGiftable())
                        continue;
                    var giftsForNpc = new List<GiftGivingAction>();

                    foreach(var item in groupedItems)
                    {
                        giftsForNpc.Add(new GiftGivingAction(new StardewValley.Object(item.Key.ParentSheetIndex, 1, quality: item.Key.Quality), npc));
                    }
                    BestGiftsOfTheDay.AddRange(giftsForNpc.OrderByDescending(g => g.FriendshipAmount).Take(10));
                }

                //BestGiftsOfTheDay = BestGiftsOfTheDay.OrderBy(g => g.TargetNPC.Name).ThenByDescending(g => g.FriendshipAmount).ToList();

                foreach (var npcGifts in BestGiftsOfTheDay.GroupBy(g => g.TargetNPC.Name).OrderByDescending(g => g.Max(i => i.FriendshipAmount)))
                {
                    GiftAdvisorMod.InventoryTrackerModule.Monitor.Log($"Best gifts for {npcGifts.Key}: " + string.Join(", ", npcGifts.Take(3).Select(g => $"{g.Item.Name} {g.FriendshipAmount:+#;-#;0}")));
                }
            }
        }
	}
}

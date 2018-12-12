using GiftAdvisor.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI;
using Polymaker.SdvUI.Controls;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor.UI
{
    public class BestGiftsMenu : SdvForm//IClickableMenu
    {
		private static List<GiftGivingAction> BestGiftsOfTheDay;
        //private ClickableTextureComponent RefreshButton;
        //private static GameDate LastUpdated;
        private SdvLabel FirstTabLabel;
        private SdvButton RefreshButton;
        private SdvContainer BestItemListView;

		static BestGiftsMenu()
		{
			BestGiftsOfTheDay = new List<GiftGivingAction>();
		}

		public BestGiftsMenu()
		{
            IntiliazeMenu();
        }

		public BestGiftsMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
		{
            IntiliazeMenu();
        }

        private void IntiliazeMenu()
        {
            //CalculateBestGifts();

            //var textSize = Game1.smallFont.MeasureString("Refresh");
            //RefreshButton = new ClickableTextureComponent("Refresh",
            //    new Rectangle(xPositionOnScreen + width - 44 - (int)textSize.X - 32, yPositionOnScreen + 100,
            //    (int)textSize.X + 32, (int)textSize.Y + 16),
            //    "Refresh", null, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), 4f, false)
            //{
            //};
            RefreshButton = new SdvButton() { Text = "Refresh", Font = Game1.smallFont };
            RefreshButton.X = Width - RefreshButton.Width - GameMenuPadding.Right;
            RefreshButton.Y = GameMenuPadding.Top + 20;

            FirstTabLabel = new SdvLabel() { Text = "Best all-around gifts", Font = Game1.dialogueFont };
            FirstTabLabel.X = GameMenuPadding.Left + 20;
            FirstTabLabel.Y = GameMenuPadding.Top + 20;


            BestItemListView = new SdvContainer()
            {
                X = GameMenuPadding.Left + GAME_MENU_BORDER,
                Y = GameMenuPadding.Top + 100,
                Width = Width - GameMenuPadding.Horizontal - GAME_MENU_BORDER * 2
            };

            BestItemListView.Height = Height - BestItemListView.Y - GAME_MENU_BORDER - GameMenuPadding.Bottom;
            BestItemListView.Controls.Add(new SdvButton() { Text = "Hello" });
            BestItemListView.Controls.Add(new SdvButton() { Text = "World", X = 100, Y = 600 });

            Controls.Add(RefreshButton);
            Controls.Add(FirstTabLabel);
            Controls.Add(BestItemListView);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        //public override void draw(SpriteBatch b)
        //{
        //    base.draw(b);
        //    Utility.drawTextWithShadow(b, "Best all-around gifts", Game1.dialogueFont, new Vector2(xPositionOnScreen + 40, yPositionOnScreen + 100), Game1.textColor);

        //    drawTextureBox(b, RefreshButton.texture, RefreshButton.sourceRect,
        //        RefreshButton.bounds.X, RefreshButton.bounds.Y, RefreshButton.bounds.Width, RefreshButton.bounds.Height, Color.White, RefreshButton.scale, true);
        //    Utility.drawTextWithShadow(b, RefreshButton.label, 
        //        Game1.smallFont, new Vector2((float)RefreshButton.bounds.Center.X, (float)(RefreshButton.bounds.Center.Y + 4)) - Game1.smallFont.MeasureString(RefreshButton.label) / 2f, Game1.textColor, 1f, -1f, -1, -1, 0f, 3);

        //}

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

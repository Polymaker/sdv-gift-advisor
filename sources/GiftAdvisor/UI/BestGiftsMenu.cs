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
        private SdvLabel FirstTabLabel;
        private SdvButton RefreshButton;
        private SdvScrollableControl BestItemListView;

        private List<GiftGivingAction> BestGiftsOfTheDay => GiftAdvisorMod.ItemGivingTrackerModule.BestGiftsOfTheDay;
        private int FirstColumnWidth = 280;
        private int SecondColumnWidth = 120;

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
            RefreshButton = new SdvButton() { Text = "Refresh", Font = Game1.smallFont };
            RefreshButton.X = Width - RefreshButton.Width - GameMenuPadding.Right - GAME_MENU_BORDER - 8;
            RefreshButton.Y = GameMenuPadding.Top + GAME_MENU_BORDER + 8;
            RefreshButton.MouseClick += RefreshButton_MouseClick;

            FirstTabLabel = new SdvLabel() { Text = "Best all-around gifts", Font = Game1.dialogueFont };
            FirstTabLabel.X = GameMenuPadding.Left + GAME_MENU_BORDER + 8;
            FirstTabLabel.Y = GameMenuPadding.Top + GAME_MENU_BORDER + 8;

            var itemLabel = new SdvLabel() { Text = "Item", X = FirstTabLabel.X, Y = FirstTabLabel.Bounds.Bottom + 16 };
            var qualityLabel = new SdvLabel() { Text = "Quality", X = itemLabel.X + FirstColumnWidth, Y = itemLabel.Y};
            var friendshipLabel = new SdvLabel() { Text = "Avg. Friendship", X = qualityLabel.X + SecondColumnWidth, Y = itemLabel.Y };

            Controls.Add(itemLabel);
            Controls.Add(qualityLabel);
            Controls.Add(friendshipLabel);


            BestItemListView = new SdvScrollableControl()
            {
                X = GameMenuPadding.Left + GAME_MENU_BORDER,
                Y = itemLabel.Bounds.Bottom + 8,
                Width = Width - GameMenuPadding.Horizontal - (GAME_MENU_BORDER * 2) - 8
            }; 

            BestItemListView.Height = Height - BestItemListView.Y - GAME_MENU_BORDER - GameMenuPadding.Bottom;

            var itemHeight = (int)Game1.smallFont.MeasureString("Test").Y + 8;

            BestItemListView.Padding = new Padding(8, 0, 0, itemHeight);

            BestItemListView.VScrollBar.SmallChange = itemHeight / 2;
            BestItemListView.VScrollBar.LargeChange = itemHeight;
            BestItemListView.VScrollBar.WheelScrollLarge = true;
            //BestItemListView.HScrollBar.BackColor = Color.Red;
            //BestItemListView.VScrollBar.BackColor = Color.Red;

            Controls.Add(RefreshButton);
            Controls.Add(FirstTabLabel);
            Controls.Add(BestItemListView);

            
            UpdateBestGiftList();
        }

        private void UpdateBestGiftList()
        {
            BestItemListView.Controls.Clear();

            var groupedItems = BestGiftsOfTheDay.GroupBy(i => new { i.Item.ParentSheetIndex, i.Item.Quality }).OrderByDescending(g => g.Average(x => x.FriendshipAmount));

            int currentY = 0;

            foreach (var itemGroup in groupedItems.Take(30))
            {
                var item = itemGroup.First().Item;
                var averageFriendShip = (int)Math.Round(itemGroup.Average(x => x.FriendshipAmount));
                if (averageFriendShip <= 10)
                    break;

                Rectangle imageSourceRect;
                Texture2D imageTexture;

                if (item.bigCraftable.Value)
                {
                    imageSourceRect = StardewValley.Object.getSourceRectForBigCraftable(item.ParentSheetIndex);
                    imageTexture = Game1.bigCraftableSpriteSheet;
                }
                else
                {
                    imageSourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, 16, 16);
                    imageTexture = Game1.objectSpriteSheet;
                }

                var itemLabel = new SdvLabel()
                {
                    Text = $"{item.Name}",
                    X = 0,
                    Y = currentY,
                    Image = new SdvImage(imageTexture, imageSourceRect, 2f),
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    TextAlign = ContentAlignment.MiddleLeft,
                    TextImageSpacing = 8
                };
                
                BestItemListView.Controls.Add(itemLabel);

                var qualityLabel = new SdvLabel() { Text = $"{GetQualityName(itemGroup.Key.Quality)}" };
                qualityLabel.X = FirstColumnWidth + 20;
                qualityLabel.Y = currentY + (itemLabel.Height - qualityLabel.Height) / 2;

                BestItemListView.Controls.Add(qualityLabel);

                var friendShipLabel = new SdvLabel() { Text = $"{averageFriendShip}" };
                friendShipLabel.X = FirstColumnWidth + SecondColumnWidth + 20;
                friendShipLabel.Y = currentY + (itemLabel.Height - friendShipLabel.Height) / 2;

                BestItemListView.Controls.Add(friendShipLabel);

                currentY += itemLabel.Height + 8;
            }
        }
        
        private void RefreshButton_MouseClick(object sender, MouseEventArgs e)
        {
            GiftAdvisorMod.ItemGivingTrackerModule.CalculateBestGifts();
            UpdateBestGiftList();
        }

        private string GetQualityName(int quality)
        {
            switch (quality)
            {
                default:
                case 0:
                    return string.Empty;
                case 1:
                    return "Silver";
                case 2:
                    return "Gold";
                case 4:
                    return "Iridium";
            }
        }
    }
}

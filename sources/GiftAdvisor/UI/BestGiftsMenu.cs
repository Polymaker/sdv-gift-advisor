using GiftAdvisor.Data;
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
		private static GameDate LastUpdated;

		static BestGiftsMenu()
		{
			BestGiftsOfTheDay = new List<GiftGivingAction>();
		}

		public BestGiftsMenu()
		{
			CalculateBestGifts();
		}

		public BestGiftsMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
		{
			CalculateBestGifts();
		}

		private void CalculateBestGifts()
		{
			if(LastUpdated == GameDate.MinDate || GameDate.Today != LastUpdated)
			{
				BestGiftsOfTheDay.Clear();

				LastUpdated = GameDate.Today;
			}
		}
	}
}

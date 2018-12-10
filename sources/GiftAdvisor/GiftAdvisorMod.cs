using GiftAdvisor.API;
using GiftAdvisor.Modules;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor
{
    public class GiftAdvisorMod : Mod
    {
        private IGameMenuExtenderAPI MenuExtender;
		internal static InventoryTracker InventoryTrackerModule;
        internal static ItemGivingTracker ItemGivingTrackerModule;

		public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
			InventoryTrackerModule = new InventoryTracker(this);
			ItemGivingTrackerModule = new ItemGivingTracker(this);
		}

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            MenuExtender = Helper.ModRegistry.GetApi<IGameMenuExtenderAPI>("Polymaker.GameMenuExtender");
            if (MenuExtender != null)
                MenuExtender.RegisterTabPageExtension("Social", "BestGiftsMenu", "Gift Advisor", typeof(UI.BestGiftsMenu));
            else
                Monitor.Log("GameMenuExtender is required for full experience!", LogLevel.Error);

            if (MenuExtender != null)
                InventoryTrackerModule.AttachGameEvents();
            ItemGivingTrackerModule.AttachGameEvents();
        }
    }
}

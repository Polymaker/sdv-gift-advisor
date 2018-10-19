using GiftAdvisor.API;
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
		private ItemGivingAction CurrentAction;
		private List<ItemDeliveryAction> ActiveItemQuests;
        private List<GiftGivingAction> GiftList;

		public override void Entry(IModHelper helper)
        {
            GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
            GameEvents.EighthUpdateTick += GameEvents_EighthUpdateTick;
            GraphicsEvents.OnPreRenderHudEvent += GraphicsEvents_OnPreRenderHudEvent;
			TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
			ActiveItemQuests = new List<ItemDeliveryAction>();
            GiftList = new List<GiftGivingAction>();

        }

		private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
		{
			UpdateActiveQuests();
            GiftList.Clear();

        }

		class GiftGivingInfo
        {
            public StardewValley.Object Gift { get; set; }
            public NPC TargetNPC { get; set; }
            public GiftTaste Taste { get; set; }
            public int FriendshipAmount { get; set; }
        }
		

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            MenuExtender = Helper.ModRegistry.GetApi<IGameMenuExtenderAPI>("Polymaker.GameMenuExtender");
            if (MenuExtender != null)
                MenuExtender.RegisterTabPageExtension("Social", "BestGiftsMenu", "Gift Advisor", typeof(UI.BestGiftsMenu));
            else
                Monitor.Log("GameMenuExtender is required!", LogLevel.Error);
        }

		private void UpdateActiveQuests()
		{
			ActiveItemQuests.Clear();
			foreach (var quest in Game1.player.questLog)
			{
				if (quest.completed.Value)
					continue;
				if(quest is ItemDeliveryQuest idq)
					ActiveItemQuests.Add(new ItemDeliveryAction(idq));
				else if (quest is LostItemQuest liq)
					ActiveItemQuests.Add(new ItemDeliveryAction(liq));
				else if (quest is FishingQuest fq)
					ActiveItemQuests.Add(new ItemDeliveryAction(fq));
			}

            foreach (var q in ActiveItemQuests)
                Monitor.Log($"Acitve Quest: \"{q.Quest.questTitle}\" NPC: {q.TargetNPC.Name} Item: {q.Item.name} x{q.Quantity} ");
		}

        private void CheckCanGiftItem()
        {
            
            ItemGivingAction currentGiftAction = null;
            
            if (Game1.player.ActiveObject != null && Game1.currentLocation != null)
            {
				var heldObject = Game1.player.ActiveObject;

                bool isGiftable = heldObject.canBeGivenAsGift();
				bool isQuestItem = heldObject.questItem.Value;

				if (isGiftable || isQuestItem)
				{
					NPC targetNPC = null;
                    var cursorPos = Helper.Input.GetCursorPosition();

                    if (Utility.checkForCharacterInteractionAtTile(cursorPos.Tile, Game1.player))
						targetNPC = Game1.currentLocation.isCharacterAtTile(cursorPos.Tile);
					else if (Utility.checkForCharacterInteractionAtTile(cursorPos.Tile + new Vector2(0f, 1f), Game1.player))
						targetNPC = Game1.currentLocation.isCharacterAtTile(cursorPos.Tile + new Vector2(0f, 1f));

					if (targetNPC != null)
					{
						var questItemAction = ActiveItemQuests.FirstOrDefault(q => q.CanCompleteDelivery(heldObject, targetNPC));

                        if (questItemAction != null)
                        {
                            currentGiftAction = questItemAction;
                        }
                        else if (isGiftable)
                        {
                            var giftAction = GiftList.FirstOrDefault(g => g.Matches(heldObject, targetNPC));
                            if (giftAction == null)
                            {
                                giftAction = new GiftGivingAction(heldObject, targetNPC);
                                GiftList.Add(giftAction);
                            }
                            currentGiftAction = giftAction;
                        }
					}
				}
            }

            CurrentAction = currentGiftAction;
            if(CurrentAction != null)
            {
                var cursorPos = Helper.Input.GetCursorPosition();
                CurrentAction.IsWithinRange = Utility.tileWithinRadiusOfPlayer((int)cursorPos.Tile.X, (int)cursorPos.Tile.Y, 1, Game1.player);
            }
        }

        private void GameEvents_EighthUpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            CheckCanGiftItem();
        }

        private void GraphicsEvents_OnPreRenderHudEvent(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (CurrentAction != null)
            {
                IClickableMenu.drawHoverText(
                    Game1.spriteBatch,
                    CurrentAction.GetTooltipText(),
                    Game1.smallFont,
                    xOffset: 64,
                    alpha: CurrentAction.IsWithinRange ? 1f : 0.5f);
            }
        }
    }
}

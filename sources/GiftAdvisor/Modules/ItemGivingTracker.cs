using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;

namespace GiftAdvisor.Modules
{
	class ItemGivingTracker : ModuleBase
	{
		private ItemGivingAction CurrentGivingAction;
		private List<GiftGivingAction> PreviousGiftTentatives;
		private List<ItemDeliveryAction> ActiveItemQuests;

		public ItemGivingTracker(IMod mod) : base(mod)
		{
			ActiveItemQuests = new List<ItemDeliveryAction>();
			PreviousGiftTentatives = new List<GiftGivingAction>();
		}

		#region Game Events

		public override void AttachGameEvents()
		{
			TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
			GameEvents.EighthUpdateTick += GameEvents_EighthUpdateTick;
		}

		public override void DettachGameEvents()
		{
			TimeEvents.AfterDayStarted -= TimeEvents_AfterDayStarted;
			GameEvents.EighthUpdateTick -= GameEvents_EighthUpdateTick;
		}

		private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
		{
			PreviousGiftTentatives.Clear();
			RefreshActiveItemQuests();
		}

		private void GameEvents_EighthUpdateTick(object sender, EventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			CheckCanGiveItem();
		}

		#endregion

		private void RefreshActiveItemQuests()
		{
			ActiveItemQuests.Clear();
			foreach (var quest in Game1.player.questLog)
			{
				if (quest.completed.Value)
					continue;
				if (quest is ItemDeliveryQuest idq)
					ActiveItemQuests.Add(new ItemDeliveryAction(idq));
				else if (quest is LostItemQuest liq)
					ActiveItemQuests.Add(new ItemDeliveryAction(liq));
				else if (quest is FishingQuest fq)
					ActiveItemQuests.Add(new ItemDeliveryAction(fq));
			}

			foreach (var q in ActiveItemQuests)
				Monitor.Log($"Acitve Quest: \"{q.Quest.questTitle}\" NPC: {q.TargetNPC.Name} Item: {q.Item.name} x{q.Quantity} ");
		}

		private bool CheckCanGiveItem()
		{
			ItemGivingAction currentItemAction = null;

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

					if (targetNPC != null && targetNPC.IsGiftable())
					{
						var questItemAction = ActiveItemQuests.FirstOrDefault(q => q.CanCompleteDelivery(heldObject, targetNPC));

						if (questItemAction != null)
						{
							currentItemAction = questItemAction;
						}
						else if (isGiftable)
						{
							var giftAction = PreviousGiftTentatives.FirstOrDefault(g => g.Matches(heldObject, targetNPC));
							if (giftAction == null)
							{
								giftAction = new GiftGivingAction(heldObject, targetNPC);
								PreviousGiftTentatives.Add(giftAction);
							}
							currentItemAction = giftAction;
						}
					}

					if (currentItemAction != null)
						currentItemAction.IsWithinRange = Utility.tileWithinRadiusOfPlayer((int)cursorPos.Tile.X, (int)cursorPos.Tile.Y, 1, Game1.player);
				}
			}

			CurrentGivingAction = currentItemAction;

			return CurrentGivingAction != null;
		}

		#region Rendering

		//private void GraphicsEvents_OnPreRenderHudEvent(object sender, EventArgs e)
		//{
		//    if (!Context.IsWorldReady)
		//        return;

		//    if (CurrentAction != null)
		//    {
		//        IClickableMenu.drawHoverText(
		//            Game1.spriteBatch,
		//            CurrentAction.GetTooltipText(),
		//            Game1.smallFont,
		//            xOffset: 64,
		//            alpha: CurrentAction.IsWithinRange ? 1f : 0.5f);
		//    }
		//}

		#endregion
	}
}

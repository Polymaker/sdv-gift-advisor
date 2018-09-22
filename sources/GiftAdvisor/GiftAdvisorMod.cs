using GiftAdvisor.API;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
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
        private GiftGivingInfo CurrentGiftInfo;

        public override void Entry(IModHelper helper)
        {
            GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
            GameEvents.EighthUpdateTick += GameEvents_EighthUpdateTick;
            GraphicsEvents.OnPreRenderHudEvent += GraphicsEvents_OnPreRenderHudEvent;
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

        private void CheckCanGiftItem()
        {
            var cursorPos = Helper.Input.GetCursorPosition();

            if (Game1.player.ActiveObject != null && Game1.currentLocation != null)
            {
                if(CurrentGiftInfo != null && Game1.player.ActiveObject == CurrentGiftInfo.Gift)
                {
                    var npcTile = CurrentGiftInfo.TargetNPC.getTileLocation();
                    if (npcTile.X == cursorPos.Tile.X && (npcTile.Y == cursorPos.Tile.Y || npcTile.Y + 1 == cursorPos.Tile.Y))
                        return;
                }

                if (Game1.player.ActiveObject.canBeGivenAsGift())
                {
                    NPC targetNPC = null;

                    if (Utility.checkForCharacterInteractionAtTile(cursorPos.Tile, Game1.player))
                        targetNPC = Game1.currentLocation.isCharacterAtTile(cursorPos.Tile);
                    else if (Utility.checkForCharacterInteractionAtTile(cursorPos.Tile + new Vector2(0f, 1f), Game1.player))
                        targetNPC = Game1.currentLocation.isCharacterAtTile(cursorPos.Tile + new Vector2(0f, 1f));

                    if (Game1.mouseCursor == 3 && targetNPC != null)
                    {
                        CurrentGiftInfo = new GiftGivingInfo
                        {
                            Gift = Game1.player.ActiveObject,
                            TargetNPC = targetNPC,
                            Taste = (GiftTaste)targetNPC.getGiftTasteForThisItem(Game1.player.ActiveObject),
                            FriendshipAmount = GiftHelper.GetGiftFriendshipValue(targetNPC, Game1.player.ActiveObject)
                        };
                    }
                    else
                        CurrentGiftInfo = null;
                }
                else
                    CurrentGiftInfo = null;
            }
            else if(CurrentGiftInfo != null)
                CurrentGiftInfo = null;
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

            if (CurrentGiftInfo != null)
            {
                var text = CurrentGiftInfo.Taste.ToString() + ": ";
                text += (CurrentGiftInfo.FriendshipAmount > 0 ? "+" : "") + $"{CurrentGiftInfo.FriendshipAmount} Friendship";

                IClickableMenu.drawHoverText(
                               Game1.spriteBatch,
                               text,
                               Game1.smallFont,
                               xOffset: 64,
                               alpha: Game1.mouseCursorTransparency);
            }
        }
    }
}

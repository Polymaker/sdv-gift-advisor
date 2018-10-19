using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor
{
    static class GiftHelper
    {
        public static bool IsGiftable(this NPC npc)
        {
            return Game1.player.friendshipData.ContainsKey(npc.Name);
        }

        public static int GetGiftFriendshipValue(NPC npc, StardewValley.Object gift)
        {
            var giftTaste = (GiftTaste)npc.getGiftTasteForThisItem(gift);

            float friendshipMultiplier = 1f;
            if (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                friendshipMultiplier = 8f;

            float qualityMultiplier = 1f;
            switch (gift.Quality)
            {
                case 1:
                    qualityMultiplier = 1.1f;
                    break;
                case 2:
                    qualityMultiplier = 1.25f;
                    break;
                case 4:
                    qualityMultiplier = 1.5f;
                    break;
            }

            switch (giftTaste)
            {
                case GiftTaste.Love:
                    return (int)(80f * friendshipMultiplier * qualityMultiplier);
                case GiftTaste.Like:
                    return (int)(45f * friendshipMultiplier * qualityMultiplier);
                case GiftTaste.Dislike:
                    return (int)(-20f * friendshipMultiplier);
                case GiftTaste.Hate:
                    return (int)(-40f * friendshipMultiplier);
                case GiftTaste.Neutral:
                    return (int)(20f * friendshipMultiplier);
            }

            return 0;
        }
    }
}

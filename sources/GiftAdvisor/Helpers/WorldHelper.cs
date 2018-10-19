using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor.Helpers
{
	public static class WorldHelper
	{
		public static IEnumerable<GameLocation> GetAllLocations()
		{
			if (!Context.IsWorldReady)
				return new GameLocation[0];

			return Game1.locations
				.Concat(
					from location in Game1.locations.OfType<BuildableGameLocation>()
					from building in location.buildings
					where building.indoors.Value != null
					select building.indoors.Value
				);
		}
	}
}

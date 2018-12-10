using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiftAdvisor.Data;
using GiftAdvisor.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace GiftAdvisor.Modules
{
	class InventoryTracker : ModuleBase
	{
		private bool IsInitialized;
		private List<InventoryLocation> Inventories;
		//private GameDate InitializationDate;

		public InventoryTracker(IMod mod) : base(mod)
		{
			Inventories = new List<InventoryLocation>();
		}

		public override void AttachGameEvents()
		{
            Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            Helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            Helper.Events.World.ObjectListChanged += World_ObjectListChanged;
		}

        public override void DettachGameEvents()
		{
            Helper.Events.GameLoop.SaveLoaded -= GameLoop_SaveLoaded;
            Helper.Events.GameLoop.ReturnedToTitle -= GameLoop_ReturnedToTitle;
            Helper.Events.World.ObjectListChanged -= World_ObjectListChanged;
        }
        
        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
			InitializeInventoriesList();
		}

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            IsInitialized = false;
            Inventories.Clear();
        }

        private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            foreach (var addedChest in e.Added.Where(x => x.Value is Chest))
            {
                if (!Inventories.Any(i => i.Tile == addedChest.Key && i.Location == e.Location))
                {
                    Inventories.Add(new InventoryLocation((Chest)addedChest.Value, e.Location, addedChest.Key));
                }
            }

            foreach (var removedChest in e.Removed.Where(x => x.Value is Chest))
            {
                Inventories.RemoveAll(i => i.Location == e.Location && i.Tile == removedChest.Key);
            }
        }

		private void InitializeInventoriesList()
		{
			Inventories.Clear();

			foreach (var location in WorldHelper.GetAllLocations())
			{
				foreach(var groundObject in location.Objects.Pairs)
				{
					if (groundObject.Value is Chest chest)
						Inventories.Add(new InventoryLocation(chest, location, groundObject.Key));
				}

				if(location is FarmHouse house && house.fridge.Value != null)
				{
					Inventories.Add(new InventoryLocation(house.fridge.Value, location, house.fridge.Value.TileLocation));
				}
			}
			IsInitialized = true;
			//InitializationDate = GameDate.Today;
		}

        public IEnumerable<Item> GetAllItems()
        {
            foreach(var inventory in Inventories)
            {
                foreach(var item in inventory.Chest.items)
                    yield return item;
            }
        }

		private class InventoryLocation
		{
			public Chest Chest { get; set; }
			public GameLocation Location { get; set; }
			public Vector2 Tile { get; set; }

			public InventoryLocation(Chest chest, GameLocation location)
			{
				Chest = chest;
				Location = location;
			}

			public InventoryLocation(Chest chest, GameLocation location, Vector2 tile) : this(chest, location)
			{
				Tile = tile;
			}
		}
	}
}

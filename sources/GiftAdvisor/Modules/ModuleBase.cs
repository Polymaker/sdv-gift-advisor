using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor.Modules
{
	class ModuleBase
	{
		public IMod Mod { get; private set; }

		public IMonitor Monitor => Mod.Monitor;

		public IModHelper Helper => Mod.Helper;

		public ModuleBase(IMod mod)
		{
			Mod = mod;
		}

		public virtual void AttachGameEvents()
		{

		}

		public virtual void DettachGameEvents()
		{

		}
	}
}

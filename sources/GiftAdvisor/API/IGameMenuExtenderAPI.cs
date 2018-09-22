using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor.API
{
    public interface IGameMenuExtenderAPI
    {
        event EventHandler CurrentTabPageChanged;

        /// <summary>
        /// Registers a custom tab in the game menu.
        /// </summary>
        /// <param name="tabName">The tab's identifier.</param>
        /// <param name="label">The tab's tooltip text and the tab's main page label.</param>
        /// <param name="pageMenuClass">The class of the page menu IClickableMenu.</param>
        void RegisterCustomTabPage(string tabName, string label, Type pageMenuClass);

        /// <summary>
        /// Registers an additional page for an existing tab. It is possible to extend both custom and vanilla tabs.
        /// <para>The standard (vanilla) tab names are: Inventory, Skills, Social, Map, Crafting, Collections, Options, Exit</para>
        /// <para>To extend a custom tab, use the following format: ModUniqueID.TabName</para>
        /// </summary>
        /// <param name="tabName">The tab's name.</param>
        /// <param name="pageName">The page's identifier.</param>
        /// <param name="pageLabel">The page label.</param>
        /// <param name="pageMenuClass">A type desce</param>
        void RegisterTabPageExtension(string tabName, string pageName, string pageLabel, Type pageMenuClass);

        /// <summary>
        /// Gets the current TabPage displayed in the GameMenu
        /// </summary>
        /// <returns></returns>
        IClickableMenu GetCurrentTabPage();

        /// <summary>
        /// Gets the current TabPage name displayed in the GameMenu
        /// </summary>
        /// <returns></returns>
        string GetCurrentTabPageName();
    }
}

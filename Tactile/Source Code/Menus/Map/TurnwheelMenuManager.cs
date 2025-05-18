using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Tactile.Menus.Options;
using Tactile.Windows.Map;
using Tactile.Windows.UserInterface.Command;

namespace Tactile.Menus.Map.Turnwheel
{
    class TurnwheelMenuManager : InterfaceHandledMenuManager<IMapMenuHandler>
    {
        public TurnwheelMenuManager(IMapMenuHandler handler)
            : base(handler)
        {
            var turnwheelMenu = new TurnwheelMenu();
            turnwheelMenu.Selected += turnwheelMenu_Selected;
            turnwheelMenu.Canceled += menu_ClosedCanceled;
            AddMenu(turnwheelMenu);
        }

        void menu_ClosedCanceled(object sender, EventArgs e)
        {
            Menus.Clear();
            Global.game_temp.menuing = false;
            Global.game_map.highlight_test();
        }

        // Selected an item in the turnwheel menu
        private void turnwheelMenu_Selected(object sender, EventArgs e)
        {
            Global.game_system.play_se(System_Sounds.Confirm);

            var turnwheelMenu = (sender as TurnwheelMenu);

            int index = turnwheelMenu.Index;
            Global.rewind_turnwheel(index);

            Menus.Clear();
            Global.game_temp.menuing = false;
            Global.game_map.highlight_test();
        }
    }
}

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
            turnwheelMenu.IndexChanged += turnwheelMenu_IndexChanged;
            AddMenu(turnwheelMenu);
        }

        void menu_ClosedCanceled(object sender, EventArgs e)
        {
            Menus.Clear();
            Global.game_temp.menuing = false;
            Global.game_map.highlight_test();
            Global.game_temp.clear_turnwheel_preview();
        }

        // Selected an item in the turnwheel menu
        private void turnwheelMenu_Selected(object sender, EventArgs e)
        {
            var turnwheelMenu = (sender as TurnwheelMenu);
            int index = turnwheelMenu.Index;

            Turnwheel_Snapshot snapshot = TurnwheelMenu.snapshots[index];
            if (snapshot.index == Global.turnwheel.current_snapshot.index)
            {
                Global.game_system.play_se(System_Sounds.Buzzer); // Can't rewind time to the present
            }
            else
            {
                Global.game_system.play_se(System_Sounds.Confirm);
                Global.rewind_turnwheel(snapshot.index);
                Menus.Clear();
                Global.game_temp.menuing = false;
                Global.game_map.highlight_test();
                Global.game_temp.clear_turnwheel_preview();
            }
        }
        private void turnwheelMenu_IndexChanged(object sender, EventArgs e)
        {
            var turnwheelMenu = (sender as TurnwheelMenu);
            int index = turnwheelMenu.Index;

            Turnwheel_Snapshot snapshot = TurnwheelMenu.snapshots[index];
            Global.game_temp.turnwheel_preview = snapshot;

            Global.scene_change("Rewind_Turnwheel");
            Global.game_map.highlight_test();
            Global.init_map();
            Global.game_map.refresh_alpha();
            Global.game_temp.menuing = true;
            Global.game_system.Instant_Move = true;
            Global.game_map.update_scroll_position();
        }
    }
}

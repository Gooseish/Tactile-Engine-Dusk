using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Help;
using Tactile.Windows.Command;

namespace Tactile.Menus.Map.Turnwheel
{
    class TurnwheelMenu : CommandMenu
    {
        public TurnwheelMenu() : base(new_map_window(56))
        {

        }

        private static Window_Command new_map_window(int width)
        {
            List<string> commands = new List<string> { };
            List<Turnwheel_Snapshot> excluded_snapshots = new List<Turnwheel_Snapshot> { Global.turnwheel.current_snapshot }; // Can't rewind time to the present
            foreach (Turnwheel_Snapshot snapshot in Global.turnwheel.snapshots.Except(excluded_snapshots))
                commands.Add("foo");
            var window = new Window_Command(
                new Vector2(8 + (show_menu_on_right ?
                    (Config.WINDOW_WIDTH - (width + 16)) : 0), 24),
                width, commands);
            window.stereoscopic = Config.MAPCOMMAND_WINDOW_DEPTH;
            window.help_stereoscopic = Config.MAPCOMMAND_HELP_DEPTH;
            return window;
        }
        private static bool show_menu_on_right
        {
            get
            {
                return true;
            }
        }
    }
}

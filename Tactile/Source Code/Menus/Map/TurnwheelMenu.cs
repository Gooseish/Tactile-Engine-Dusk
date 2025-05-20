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
        public static List<Turnwheel_Snapshot> snapshots { get { return Enumerable.Reverse(Global.turnwheel.snapshots).ToList<Turnwheel_Snapshot>(); } }
        public TurnwheelMenu() : base(new_map_window(100))
        {
            create_cancel_button();
        }

        private static Window_Command_Scrollbar new_map_window(int width)
        {
            List<string> commands = new List<string> { };
            foreach (Turnwheel_Snapshot snapshot in snapshots)
                commands.Add(snapshot.name);
            var window = new Window_Command_Scrollbar(
                new Vector2(8 + (show_menu_on_right ?
                    (Config.WINDOW_WIDTH - (width + 16)) : 0), 24),
                width, 8, commands);
            window.stereoscopic = Config.MAPCOMMAND_WINDOW_DEPTH;
            window.help_stereoscopic = Config.MAPCOMMAND_HELP_DEPTH;
            //window.still_cursor = true;
            return window;
        }
        private static bool show_menu_on_right
        {
            get
            {
                return true;
            }
        }
        private void create_cancel_button()
        {
            CreateCancelButton(
                show_menu_on_right ? Config.WINDOW_WIDTH - (32 + 48)-60 : 32,
                Config.MAPCOMMAND_WINDOW_DEPTH);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Help;
using Tactile.Windows.Command;
using Tactile.Windows;

namespace Tactile.Menus.Map.Turnwheel
{
    class TurnwheelMenu : CommandMenu
    {
        public static List<Turnwheel_Snapshot> snapshots { get { return Enumerable.Reverse(Global.turnwheel.snapshots).ToList<Turnwheel_Snapshot>(); } }
        public static int index_of_current_snapshot { get { return snapshots.IndexOf(Global.turnwheel.current_snapshot); } }
        private Window_Turnwheel Turn_Display;
        public TurnwheelMenu() : base(new_map_window(100))
        {
            create_cancel_button();
            create_turn_display();
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
            window.set_text_color(index_of_current_snapshot, "Grey");
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
        private void create_turn_display()
        {
            Turn_Display = new Window_Turnwheel();
            Turn_Display.loc = new Vector2(Config.WINDOW_WIDTH - 100 - 70, 50);
            update_turn_display();
        }
        private void update_turn_display()
        {
            if (snapshots.Count > Index)
                Turn_Display.turn_number = snapshots[Index].game_system.chapter_turn;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Turn_Display.draw(spriteBatch);
        }
        protected override void UpdateMenu(bool active)
        {
            base.UpdateMenu(active);
            update_turn_display();
        }
    }
}

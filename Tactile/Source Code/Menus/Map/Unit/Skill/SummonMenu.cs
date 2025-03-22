using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tactile.Menus;
using TactileLibrary;
using Tactile.Windows.Command;
using Tactile.Menus.Map.Unit;

namespace Tactile.Source_Code.Menus.Map.Unit.Skill
{
    class SummonMenu : CommandMenu
    {

        private List<int> IndexRedirect;
        internal int unitId;
        public SummonMenu(Game_Unit unit)
        {
            RefreshCommands(unit);
            unitId = unit.id;
        }

        public void RefreshCommands(Game_Unit unit)
        {
            List<string> commands = SetCommands(unit);
            var window = NewUnitWindow(commands, 56);
            Window = window;
        }

        public List<String> SetCommands(Game_Unit unit)
        {
            List<String> commands;
            commands = (Enum.GetNames(typeof(SummonMenuIds))).ToList();
            return commands;
        }
        private Window_Command NewUnitWindow(List<string> commands, int width)
        {
            var window = new Window_Command(
                new Vector2(8 + (Global.player.is_on_left() ? (Config.WINDOW_WIDTH - (width + 16)) : 0), 24), width, commands);
            window.stereoscopic = Config.MAPCOMMAND_WINDOW_DEPTH;
            window.help_stereoscopic = Config.MAPCOMMAND_HELP_DEPTH;



            return window;
        }

        public SummonMenuIds SelectedCommand
        {
            get
            {
                return (SummonMenuIds)Window.selected_index().Index;
            }
        }
    }
}

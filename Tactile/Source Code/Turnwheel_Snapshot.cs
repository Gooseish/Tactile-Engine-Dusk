using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tactile
{
    class Turnwheel_Snapshot
    {
        private Game_Battalions @Game_Battalions = new Game_Battalions();
        private Game_Actors @Game_Actors = new Game_Actors();
        private Game_System @Game_System = new Game_System();
        private Player @Player = new Player();
        private Game_State @Game_State = new Game_State();
        private Game_Map @Game_Map = new Game_Map();
        private string Name;

        public Game_Battalions game_battalions { get { return Game_Battalions; } }
        public Game_Actors game_actors { get { return Game_Actors; } }
        public Game_System game_system { get { return Game_System; } }
        public Player player { get { return Player; } }
        public Game_State game_state { get { return Game_State; } }
        public Game_Map game_map { get { return Game_Map; } }
        public string name { get { return Name; } }
        

        public void write(BinaryWriter writer)
        {
            Game_Battalions.write(writer);
            Game_Actors.write(writer);
            Game_System.write(writer);

            Player.write(writer);
            Game_State.write(writer);
            Game_Map.write(writer);
            Game_System.write_events(writer);

            writer.Write(Name);
        }
        public void read(BinaryReader reader)
        {
            Game_Battalions.read(reader);
            Game_Actors.read(reader);
            Game_System.read(reader);

            Player.read(reader);
            Game_State.read(reader);
            Game_Map.read(reader);
            Game_System.read_events(reader);

            Name = reader.ReadString();
        }
        public Turnwheel_Snapshot(string name) { Name = name; }
        public Turnwheel_Snapshot() { Name = "Unnamed"; }
        public void Get_Global_Variables()
        {
            Game_Battalions = Global.game_battalions;
            Game_Actors = Global.game_actors;
            Game_System = Global.game_system;

            Player = Global.player;
            Game_State = Global.game_state;
            Game_Map = Global.game_map;
            Game_System = Global.game_system;
        }
    }
}

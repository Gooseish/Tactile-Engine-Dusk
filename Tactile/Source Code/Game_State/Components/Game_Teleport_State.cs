using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Tactile.State
{
    // Skills: Teleport
    class Game_Teleport_State : Game_Combat_State_Component
    {
        protected bool Teleport_Calling = false;
        protected bool In_Teleport = false;
        protected int Teleport_Phase = 0;
        protected int Teleport_Timer = 0;

        protected int Teleporter_Id = -1;

        protected int Teleport_Destination = -1;




        #region Serialization
        internal override void write(BinaryWriter writer)
        {
            base.write(writer);
            writer.Write(In_Teleport);
            writer.Write(Teleport_Phase);
            writer.Write(Teleport_Timer);
            writer.Write(Teleporter_Id);
            writer.Write(Teleport_Destination);

        }

        internal override void read(BinaryReader reader)
        {
            base.read(reader);
            In_Teleport = reader.ReadBoolean();
            Teleport_Phase = reader.ReadInt32();
            Teleport_Timer = reader.ReadInt32();
            Teleporter_Id = reader.ReadInt32();
            Teleport_Destination = reader.ReadInt32();
        }
        #endregion

        #region Accessors
        public bool teleport_calling
        {
            get { return Teleport_Calling; }
            set { Teleport_Calling = value; }
        }


        public bool in_teleport { get { return In_Teleport; } }

        public int teleporter_id { get { return Teleporter_Id; } set { Teleporter_Id = value; } }
        public int teleport_destination { get { return Teleport_Destination; } set { Teleport_Destination = value; } }


        protected Game_Unit teleporter { get { return Teleporter_Id == -1 ? null : Units[Teleporter_Id]; } }

        #endregion

        internal override void update()
        {
            if (Teleport_Calling)
            {
                setup_teleport();
            }
            if (In_Teleport && !Global.game_state.switching_ai_skip && get_scene_map() != null)
            {
                update_map_teleport();
            }
        }

        protected void setup_teleport()
        {
            In_Teleport = true;
            Teleport_Calling = false;
        }

        protected void update_map_teleport()
        {
            Scene_Map scene_map = get_scene_map();
            if (scene_map == null)
                return;
            bool cont = false;
            while (!cont)
            {
                cont = true;
                switch (Teleport_Phase)
                {
                    // setup
                    case 0:
                        setup_update_loop(scene_map);
                        break;
                    // draw summoning circles
                    case 1:
                        teleport_animation(scene_map, cont);
                        break;
                    // apply the swap
                    case 2:
                        apply_teleport();
                        break;
                    default:
                        end_teleport();
                        break;
                }
            }
        }

        protected void setup_update_loop(Scene_Map scene_map)
        {
            switch (Teleport_Timer)
            {
                case 0:
                    Global.scene.suspend();
                    Global.game_system.Battler_1_Id = -1;
                    Global.game_system.Battler_2_Id = -1;
                    Teleport_Timer++;

                    teleporter.battling = true;
                    teleporter.frame = 0;
                    Global.game_map.move_range_visible = false;

                    break;
                case 6:
                    Teleport_Phase++;
                    Teleport_Timer = 0;
                    break;
                default:
                    Teleport_Timer++;
                    break;
            }
        }

        protected void teleport_animation(Scene_Map scene_map, bool cont)
        {
            switch (Teleport_Timer)
            {
                case 0:
                    scene_map.set_map_effect(teleporter.loc + new Vector2(0, -1), 2, 1);
                    scene_map.set_map_effect_2(teleporter.teleport_vector2_from_int(teleport_destination) + new Vector2(0, -1), 2, 1);
                    Teleport_Timer++;
                    break;
                default:
                    Teleport_Timer++;
                    break;
                case 40:
                    Teleport_Phase++;
                    Teleport_Timer = 0;
                    break;
            }
        }

        protected void apply_teleport()
        {
            teleporter.teleport(teleport_destination);
            Teleport_Phase++;
        }

        protected void end_teleport()
        {
            switch (Teleport_Timer)
            {
                case 0:
                    teleporter.battling = false;
                    teleporter.queue_move_range_update();
                    refresh_move_ranges();
                    Teleport_Timer++;
                    break;
                case 1:
                    if (!Global.game_system.is_interpreter_running && !Global.scene.is_message_window_active)
                    {
                        Teleport_Timer++;
                    }
                    break;
                case 2:
                    Teleport_Phase = 0;
                    Teleport_Timer = 0;
                    Teleporter_Id = -1;
                    Teleport_Destination = -1;
                    Teleport_Calling = false;
                    In_Teleport = false;
                    Global.game_map.move_range_visible = true;
                    highlight_test();
                    Global.game_state.any_trigger_events();
                    break;
            }
        }
    }
}

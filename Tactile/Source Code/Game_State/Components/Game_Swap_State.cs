using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Tactile.State
{
    class Game_Swap_State : Game_Combat_State_Component
    {
        protected bool Swap_Calling = false;
        protected bool In_Swap = false;
        protected int Swap_Phase = 0;
        protected int Swap_Timer = 0;

        protected int Swap_Target_Id = -1;
        protected int Swapper_Id = -1;




        #region Serialization
        internal override void write(BinaryWriter writer)
        {
            base.write(writer);
            writer.Write(In_Swap);
            writer.Write(Swap_Phase);
            writer.Write(Swap_Timer);
            writer.Write(Swap_Target_Id);
            writer.Write(Swapper_Id);
        }

        internal override void read(BinaryReader reader)
        {
            base.read(reader);
            In_Swap = reader.ReadBoolean();
            Swap_Phase = reader.ReadInt32();
            Swap_Timer = reader.ReadInt32();
            Swap_Target_Id = reader.ReadInt32();
            Swapper_Id = reader.ReadInt32();
        }
        #endregion

        #region Accessors
        public bool swap_calling
        {
            get { return Swap_Calling; }
            set { Swap_Calling = value; }
        }


        public bool in_swap { get { return In_Swap; } }

        public int swapper_id { get { return Swapper_Id; } set { Swapper_Id = value; } }
        public int swap_target_id { get { return Swap_Target_Id; } set { Swap_Target_Id = value; } }


        protected Game_Unit swapper { get { return Swapper_Id == -1 ? null : Units[Swapper_Id]; } }
        protected Game_Unit swap_target { get { return Swap_Target_Id == -1 ? null : Units[Swap_Target_Id]; } }

        #endregion

        internal override void update()
        {
            if (Swap_Calling)
            {
                setup_swap();
            }
            if (In_Swap && !Global.game_state.switching_ai_skip && get_scene_map() != null)
            {
                update_map_swap();
            }
        }

        protected void setup_swap()
        {
            In_Swap = true;
            Swap_Calling = false;
        }

        protected void update_map_swap()
        {
            Scene_Map scene_map = get_scene_map();
            if (scene_map == null)
                return;
            bool cont = false;
            while (!cont)
            {
                cont = true;
                switch (Swap_Phase)
                {
                    // setup
                    case 0:
                        setup_update_loop(scene_map);
                        break;
                    // draw summoning circles
                    case 1:
                        swap_animation(scene_map, cont);
                        break;
                    // apply the swap
                    case 2:
                        apply_swap();
                        break;
                    default:
                        end_swap();
                        break;
                }
            }
        }

        protected void setup_update_loop(Scene_Map scene_map)
        {
            switch (Swap_Timer)
            {
                case 0:
                    Global.scene.suspend();
                    Global.game_system.Battler_1_Id = -1;
                    Global.game_system.Battler_2_Id = -1;
                    Swap_Timer++;

                    swapper.battling = true;
                    swapper.frame = 0;
                    Global.game_map.move_range_visible = false;

                    break;
                case 6:
                    Swap_Phase++;
                    Swap_Timer = 0;
                    break;
                default:
                    Swap_Timer++;
                    break;
            }
        }

        protected void swap_animation(Scene_Map scene_map, bool cont)
        {
            switch (Swap_Timer)
            {
                case 0:
                    scene_map.set_map_effect(swapper.loc + new Vector2(0, -1), 2, 1);
                    scene_map.set_map_effect_2(swap_target.loc + new Vector2(0, -1), 2, 1);
                    Swap_Timer++;
                    break;
                default:
                    Swap_Timer++;
                    break;
                case 40:
                    Swap_Phase++;
                    Swap_Timer = 0;
                    break;
            }
        }

        protected void apply_swap()
        {
            swapper.swap(swap_target_id);
            Swap_Phase++;
        }

        protected void end_swap()
        {
            switch (Swap_Timer)
            {
                case 0:
                    swapper.battling = false;
                    swapper.queue_move_range_update();
                    swap_target.queue_move_range_update();
                    refresh_move_ranges();
                    Swap_Timer++;
                    break;
                case 1:
                    if (!Global.game_system.is_interpreter_running && !Global.scene.is_message_window_active)
                    {
                        Swap_Timer++;
                    }
                    break;
                case 2:
                    Swap_Phase = 0;
                    Swap_Timer = 0;
                    Swapper_Id = -1;
                    Swap_Target_Id = -1;
                    Swap_Calling = false;
                    In_Swap = false;
                    Global.game_map.move_range_visible = true;
                    highlight_test();
                    Global.game_state.any_trigger_events();
                    break;
            }
        }
    }
}

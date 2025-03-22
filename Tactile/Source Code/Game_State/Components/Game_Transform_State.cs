using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Tactile.State
{
    class Game_Transform_State : Game_Combat_State_Component
    {
        protected bool Transform_Calling = false;
        protected bool In_Transform = false;
        protected int Transform_Phase = 0;
        protected int Transform_Timer = 0;

        protected int Transformer_Id = -1;





        #region Serialization
        internal override void write(BinaryWriter writer)
        {
            base.write(writer);
            writer.Write(In_Transform);
            writer.Write(Transform_Phase);
            writer.Write(Transform_Timer);
            writer.Write(Transformer_Id);

        }

        internal override void read(BinaryReader reader)
        {
            base.read(reader);
            In_Transform = reader.ReadBoolean();
            Transform_Phase = reader.ReadInt32();
            Transform_Timer = reader.ReadInt32();
            Transformer_Id = reader.ReadInt32();
        }
        #endregion

        #region Accessors
        public bool transform_calling
        {
            get { return Transform_Calling; }
            set { Transform_Calling = value; }
        }


        public bool in_transform { get { return In_Transform; } }

        public int transformer_id { get { return Transformer_Id; } set { Transformer_Id = value; } }


        protected Game_Unit transformer { get { return Transformer_Id == -1 ? null : Units[Transformer_Id]; } }

        #endregion

        internal override void update()
        {
            if (Transform_Calling)
            {
                setup_transform();
            }
            if (In_Transform && !Global.game_state.switching_ai_skip && get_scene_map() != null)
            {
                update_map_transform();
            }
        }

        protected void setup_transform()
        {
            In_Transform = true;
            Transform_Calling = false;
        }

        protected void update_map_transform()
        {
            Scene_Map scene_map = get_scene_map();
            if (scene_map == null)
                return;
            bool cont = false;
            while (!cont)
            {
                cont = true;
                switch (Transform_Phase)
                {
                    // setup
                    case 0:
                        setup_update_loop(scene_map);
                        break;
                    // draw summoning circles
                    case 1:
                        transform_animation(scene_map, cont);
                        break;
                    // apply the swap
                    case 2:
                        apply_transform();
                        break;
                    default:
                        end_transform();
                        break;
                }
            }
        }

        protected void setup_update_loop(Scene_Map scene_map)
        {
            switch (Transform_Timer)
            {
                case 0:
                    Global.scene.suspend();
                    Global.game_system.Battler_1_Id = -1;
                    Global.game_system.Battler_2_Id = -1;
                    Transform_Timer++;

                    transformer.battling = true;
                    transformer.frame = 0;
                    Global.game_map.move_range_visible = false;

                    break;
                case 6:
                    Transform_Phase++;
                    Transform_Timer = 0;
                    break;
                default:
                    Transform_Timer++;
                    break;
            }
        }

        protected void transform_animation(Scene_Map scene_map, bool cont)
        {
            switch (Transform_Timer)
            {
                case 0:
                    scene_map.set_map_effect(transformer.loc + new Vector2(0, -1), 2, 1);
                    Transform_Timer++;
                    break;
                default:
                    Transform_Timer++;
                    break;
                case 40:
                    Transform_Phase++;
                    Transform_Timer = 0;
                    break;
            }
        }

        protected void apply_transform()
        {
            transformer.activate_dtransform();
            Transform_Phase++;
        }

        protected void end_transform()
        {
            switch (Transform_Timer)
            {
                case 0:
                    transformer.battling = false;
                    transformer.queue_move_range_update();
                    refresh_move_ranges();
                    Transform_Timer++;
                    break;
                case 1:
                    if (!Global.game_system.is_interpreter_running && !Global.scene.is_message_window_active)
                    {
                        Transform_Timer++;
                    }
                    break;
                case 2:
                    Transform_Phase = 0;
                    Transform_Timer = 0;
                    Transformer_Id = -1;
                    Transform_Calling = false;
                    In_Transform = false;
                    Global.game_map.move_range_visible = true;
                    highlight_test();
                    Global.game_state.any_trigger_events();
                    break;
            }
        }
    }
}

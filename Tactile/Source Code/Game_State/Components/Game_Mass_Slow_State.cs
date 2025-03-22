using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Tactile.State
{
    // Skills: Mass Slow
    class Game_Mass_Slow_State : Game_Combat_State_Component
    {
        protected bool Mass_Slow_Calling = false;
        protected bool In_Mass_Slow = false;
        protected int Mass_Slow_Phase = 0;
        protected int Mass_Slow_Timer = 0;

        protected int Caster_Id = -1;

        protected int Mass_Slow_Target = -1;




        #region Serialization
        internal override void write(BinaryWriter writer)
        {
            base.write(writer);
            writer.Write(In_Mass_Slow);
            writer.Write(Mass_Slow_Phase);
            writer.Write(Mass_Slow_Timer);
            writer.Write(Caster_Id);
            writer.Write(Mass_Slow_Target);

        }

        internal override void read(BinaryReader reader)
        {
            base.read(reader);
            In_Mass_Slow = reader.ReadBoolean();
            Mass_Slow_Phase = reader.ReadInt32();
            Mass_Slow_Timer = reader.ReadInt32();
            Caster_Id = reader.ReadInt32();
            Mass_Slow_Target = reader.ReadInt32();
        }
        #endregion

        #region Accessors
        public bool mass_slow_calling
        {
            get { return Mass_Slow_Calling; }
            set { Mass_Slow_Calling = value; }
        }


        public bool in_mass_slow { get { return In_Mass_Slow; } }

        public int caster_id { get { return Caster_Id; } set { Caster_Id = value; } }
        public int mass_slow_target { get { return Mass_Slow_Target; } set { Mass_Slow_Target = value; } }
        public HashSet<Game_Unit> mass_slow_target_units { get { return caster.mass_slow_targets(mass_slow_target); }
        }
        protected Game_Unit caster { get { return Caster_Id == -1 ? null : Units[Caster_Id]; } }

        #endregion

        internal override void update()
        {
            if (Mass_Slow_Calling)
            {
                setup_mass_slow();
            }
            if (In_Mass_Slow && !Global.game_state.switching_ai_skip && get_scene_map() != null)
            {
                update_map_mass_slow();
            }
        }

        protected void setup_mass_slow()
        {
            In_Mass_Slow = true;
            Mass_Slow_Calling = false;

            //Map_Battle = Global.game_system.Battle_Mode == Constants.Animation_Modes.Map;
        }

        protected void update_map_mass_slow()
        {
            Scene_Map scene_map = get_scene_map();
            if (scene_map == null)
                return;
            bool cont = false;
            while (!cont)
            {
                cont = true;
                switch (Mass_Slow_Phase)
                {
                    // setup
                    case 0:
                        setup_update_loop(scene_map);
                        break;
                    // draw summoning circles
                    case 1:
                        mass_slow_animation(scene_map, cont);
                        break;
                    // apply the swap
                    case 2:
                        apply_mass_slow();
                        break;
                    default:
                        end_mass_slow();
                        break;
                }
            }
        }

        protected void setup_update_loop(Scene_Map scene_map)
        {
            switch (Mass_Slow_Timer)
            {
                case 0:
                    Global.scene.suspend();
                    Global.game_system.Battler_1_Id = -1;
                    Global.game_system.Battler_2_Id = -1;
                    Mass_Slow_Timer++;
                    //caster.battling = true;
                    caster.sprite_moving = false;
                    caster.facing = 6;
                    caster.frame = 0;
                    Global.game_map.move_range_visible = false;

                    break;
                case 6:
                    Mass_Slow_Phase++;
                    Mass_Slow_Timer = 0;
                    break;
                default:
                    Mass_Slow_Timer++;
                    break;
            }
        }

        protected void mass_slow_animation(Scene_Map scene_map, bool cont)
        {
            switch (Mass_Slow_Timer)
            {
                case 0:
                    scene_map.set_map_effect(caster.teleport_vector2_from_int(mass_slow_target), 2, 2);
                    //caster.sprite_moving = false;
                    //caster.battling = true;
                    //caster.facing = 0;
                    caster.facing = 6;
                    caster.frame = 0;
                    //caster.refresh_sprite();
                    Mass_Slow_Timer++;
                    break;
                case 340:
                    caster.facing = 6;
                    caster.frame = 1;
                    //caster.refresh_sprite();
                    Mass_Slow_Timer++;
                    break;
                case 345:
                    caster.facing = 6;
                    caster.frame = 2;
                    //caster.refresh_sprite();
                    Mass_Slow_Timer++;
                    break;
                default:
                    Mass_Slow_Timer++;
                    break;
                case 560:
                    Mass_Slow_Phase++;
                    Mass_Slow_Timer = 0;
                    break;
            }
        }

        protected void apply_mass_slow()
        {
            caster.mass_slow(mass_slow_target);
            Mass_Slow_Phase++;
        }

        protected void end_mass_slow()
        {
            switch (Mass_Slow_Timer)
            {
                case 0:
                    caster.battling = false;
                    get_scene_map().re_add_map_sprites();
                    caster.queue_move_range_update();
                    refresh_move_ranges();
                    Mass_Slow_Timer++;
                    break;
                case 1:
                    if (!Global.game_system.is_interpreter_running && !Global.scene.is_message_window_active)
                    {
                        Mass_Slow_Timer++;
                    }
                    break;
                case 2:
                    Mass_Slow_Phase = 0;
                    Mass_Slow_Timer = 0;
                    Caster_Id = -1;
                    Mass_Slow_Target = -1;
                    Mass_Slow_Calling = false;
                    In_Mass_Slow = false;
                    Global.game_map.move_range_visible = true;
                    highlight_test();
                    Global.game_state.any_trigger_events();
                    break;
            }
        }
    }
}

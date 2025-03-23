using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections;
using TactileVector2Extension;

namespace Tactile.State
{
    class Game_Summon_State : Game_Combat_State_Component
    {
        protected bool Summon_Calling = false;
        protected bool In_Summon = false;
        protected int Summon_Phase = 0;
        protected int Summon_Timer = 0;

        protected int Summon_Id = -1;

        protected Vector2 Summon_Destination = new Vector2(-1, -1);




        #region Serialization
        internal override void write(BinaryWriter writer)
        {
            base.write(writer);
            writer.Write(In_Summon);
            writer.Write(Summon_Phase);
            writer.Write(Summon_Timer);
            writer.Write(Summon_Id);
            Summon_Destination.write(writer);

        }

        internal override void read(BinaryReader reader)
        {
            base.read(reader);
            In_Summon = reader.ReadBoolean();
            Summon_Phase = reader.ReadInt32();
            Summon_Timer = reader.ReadInt32();
            Summon_Id = reader.ReadInt32();
            Summon_Destination = Summon_Destination.read(reader);
        }
        #endregion

        #region Accessors
        public bool summon_calling
        {
            get { return Summon_Calling; }
            set { Summon_Calling = value; }
        }


        public bool in_summon { get { return In_Summon; } }

        public int summoner_id { get { return Summon_Id; } set { Summon_Id = value; } }
        public Vector2 summon_destination { get { return Summon_Destination; } set { Summon_Destination = value; } }


        protected Game_Unit summoner { get { return Summon_Id == -1 ? null : Units[Summon_Id]; } }

        #endregion

        internal override void update()
        {
            if (Summon_Calling)
            {
                setup_summon();
            }
            if (In_Summon && !Global.game_state.switching_ai_skip && get_scene_map() != null)
            {
                update_map_summon();
            }
        }

        protected void setup_summon()
        {
            In_Summon = true;
            Summon_Calling = false;
        }

        protected void update_map_summon()
        {
            Scene_Map scene_map = get_scene_map();
            if (scene_map == null)
                return;
            bool cont = false;
            while (!cont)
            {
                cont = true;
                switch (Summon_Phase)
                {
                    // setup
                    case 0:
                        setup_update_loop(scene_map);
                        break;
                    // draw summoning circles
                    case 1:
                        summon_animation(scene_map, cont);
                        break;
                    // apply the swap
                    case 2:
                        apply_summon();
                        break;
                    default:
                        end_summon();
                        break;
                }
            }
        }

        protected void setup_update_loop(Scene_Map scene_map)
        {
            switch (Summon_Timer)
            {
                case 0:
                    Global.scene.suspend();
                    Global.game_system.Battler_1_Id = -1;
                    Global.game_system.Battler_2_Id = -1;
                    Summon_Timer++;

                    summoner.battling = true;
                    summoner.frame = 0;
                    Global.game_map.move_range_visible = false;

                    break;
                case 6:
                    Summon_Phase++;
                    Summon_Timer = 0;
                    break;
                default:
                    Summon_Timer++;
                    break;
            }
        }

        protected void summon_animation(Scene_Map scene_map, bool cont)
        {
            switch (Summon_Timer)
            {
                case 0:

                    List<Vector2> summonLoc_offsets = new List<Vector2> { };
                    Vector2 offset = summon_destination - summoner.loc;
                    double theta = 90 * Math.PI / 180;
                    summonLoc_offsets.Add(new Vector2(offset.X * (float)Math.Cos(theta) - offset.Y * (float)Math.Sin(theta), offset.X * (float)Math.Sin(theta) + offset.Y * (float)Math.Cos(theta)));
                    summonLoc_offsets.Add(new Vector2(offset.X * (float)Math.Cos(-theta) - offset.Y * (float)Math.Sin(-theta), offset.X * (float)Math.Sin(-theta) + offset.Y * (float)Math.Cos(-theta)));


                    switch (summoner.attemptedSummon.Count)
                    {
                        case 2:
                            scene_map.set_map_effect(summon_destination + summonLoc_offsets[0] + new Vector2(0, -1), 2, 1);
                            scene_map.set_map_effect_2(summon_destination + summonLoc_offsets[1] + new Vector2(0, -1), 2, 1);
                            break;
                        case 3:
                            scene_map.set_map_effect(summon_destination + summonLoc_offsets[0] + new Vector2(0, -1), 2, 1);
                            scene_map.set_map_effect_3(summon_destination + summonLoc_offsets[1] + new Vector2(0, -1), 2, 1);
                            scene_map.set_map_effect_2(summon_destination + new Vector2(0, -1), 2, 1);
                            break;
                        default:
                            scene_map.set_map_effect(summon_destination + new Vector2(0, -1), 2, 1);
                            break;
                    }
                    
                    Summon_Timer++;
                    break;
                default:
                    Summon_Timer++;
                    break;
                case 40:
                    Summon_Phase++;
                    Summon_Timer = 0;
                    break;
            }
        }

        protected void apply_summon()
        {
            summoner.summon(summon_destination);
            Summon_Phase++;
        }

        protected void end_summon()
        {
            switch (Summon_Timer)
            {
                case 0:
                    summoner.battling = false;
                    summoner.queue_move_range_update();
                    refresh_move_ranges();
                    Summon_Timer++;
                    break;
                case 1:
                    if (!Global.game_system.is_interpreter_running && !Global.scene.is_message_window_active)
                    {
                        Summon_Timer++;
                    }
                    break;
                case 2:
                    Summon_Phase = 0;
                    Summon_Timer = 0;
                    Summon_Id = -1;
                    Summon_Destination = new Vector2(-1, -1);
                    Summon_Calling = false;
                    In_Summon = false;
                    Global.game_map.move_range_visible = true;
                    highlight_test();
                    Global.game_state.any_trigger_events();
                    break;
            }
        }
    }
}
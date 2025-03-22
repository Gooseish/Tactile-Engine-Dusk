using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections;
using ListExtension;
using HashSetExtension;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Map;

namespace Tactile.State
{
    class Game_Skill_Flash_State : Game_Combat_State_Component
    {
        protected bool Skill_Flash_Calling = false;
        protected bool In_Skill_Flash = false;
        protected int Skill_Flash_Phase = 0;
        protected int Skill_Flash_Action = 0;
        protected int Skill_Flash_Timer = 0;
        
        protected HashSet<int> Skill_Targets = new HashSet<int>();

        protected List<string> Identifier_Queue = new List<string> { };
        protected List<int> Skill_Flash_Target_Id_Queue = new List<int> { };
        protected List<int> Skill_Flasher_Id_Queue = new List<int> { };

        protected readonly Dictionary<string, int> Skill_Flash_Image_Index = new Dictionary<string, int>
        {
            {"RALLY_LUCK", 0 },
            {"RALLY_RES", 1 },
            {"DISARM", 2 },
            {"GALEFORCE", 3 },
            { "VOLLEY", 4 },
            {"POISON_KNIFE", 5 },
            {"GRISLY_WOUND", 6 },
            {"LIFETAKER",  7},
            {"INDICT", 8 },
            {"SHATTER_DEFENSE", 9 },
            {"RALLY_SPECTRUM", 10 },
            {"DRACONIC_HEX", 11 },
            {"DREADFUL_AURA", 12 }
        };

        protected List<string> Hit_Flash_Skills = new List<string>
        {
            "VOLLEY",
            "POISON_KNIFE",
            "GRISLY_WOUND"
        };

        

        #region Serialization
        internal override void write(BinaryWriter writer)
        {
            base.write(writer);
            writer.Write(In_Skill_Flash);
            writer.Write(Skill_Flash_Phase);
            writer.Write(Skill_Flash_Action);
            writer.Write(Skill_Flash_Timer);
            Skill_Targets.write(writer);
            Identifier_Queue.write(writer);
            Skill_Flash_Target_Id_Queue.write(writer);
            Skill_Flasher_Id_Queue.write(writer);
        }

        internal override void read(BinaryReader reader)
        {
            base.read(reader);
            In_Skill_Flash = reader.ReadBoolean();
            Skill_Flash_Phase = reader.ReadInt32();
            Skill_Flash_Action = reader.ReadInt32();
            Skill_Flash_Timer = reader.ReadInt32();
            Skill_Targets.read(reader);
            Identifier_Queue.read(reader);
            Skill_Flash_Target_Id_Queue.read(reader);
            Skill_Flasher_Id_Queue.read(reader);
        }
        #endregion

        #region Accessors
        public bool skill_flash_calling
        {
            get { return Skill_Flash_Calling; }
            set { Skill_Flash_Calling = value; }
        }

        public string identifier
        {
            get 
            {
                if (Identifier_Queue.Count > 0)
                    return Identifier_Queue[0];
                else
                    return "";
            }
            set { Identifier_Queue.Add(value); }
        }

        public bool in_skill_flash { get { return In_Skill_Flash; } }
        public int skill_flasher_id
        {
            get
            {
                if (Skill_Flasher_Id_Queue.Count > 0)
                    return Skill_Flasher_Id_Queue[0];
                else
                    return -1;
            }
            set
            {
                Skill_Flasher_Id_Queue.Add(value);
            }
        }
        public int skill_flash_target_id
        {
            get
            {
                if (Skill_Flash_Target_Id_Queue.Count > 0)
                    return Skill_Flash_Target_Id_Queue[0];
                else
                    return -1;
            }
            set
            {
                Skill_Flash_Target_Id_Queue.Add(value);
            }
        }

        public bool target_alive { get { return Global.game_map.units.ContainsKey(skill_flash_target_id) && !skill_flash_target.is_dead; } }

        protected Game_Unit skill_flasher { get { return skill_flasher_id == -1 ? null : Units[skill_flasher_id]; } }
        protected Game_Unit skill_flash_target { get { return skill_flash_target_id == -1 ? null : Units[skill_flash_target_id]; } }

        public int image_index{ get { return Skill_Flash_Image_Index[identifier]; } }
        public HashSet<int> skill_targets { get { return Skill_Targets; } }

        public bool do_hit_flash { get { return Hit_Flash_Skills.Contains(identifier); } }

        public bool has_target { get { return skill_flash_target_id != -1; } }
        #endregion

        internal override void update()
        {
            if (Skill_Flash_Calling)
            {
                setup_Skill_Flash();
            }
            if (In_Skill_Flash && !Global.game_state.switching_ai_skip && get_scene_map() != null)
            {
                update_map_Skill_Flash();
            }
        }

        protected void setup_Skill_Flash()
        {
            In_Skill_Flash = true;
            Skill_Flash_Calling = false;

            Skill_Targets.Clear();
        }

        protected void update_map_Skill_Flash()
        {
            Scene_Map scene_map = get_scene_map();
            if (scene_map == null)
                return;
            bool cont = false;
            while (!cont)
            {
                cont = true;
                switch (Skill_Flash_Phase)
                {
                    // setup
                    case 0:
                        setup_update_loop(scene_map);
                        break;
                    // flash the skill icon
                    case 1:
                        skill_icon_flash_animation(scene_map, cont);
                        break;
                    // apply the skill effects
                    case 2:
                        apply_skill_effect(scene_map);
                        break;
                    default:
                        end_skill_flash();
                        break;
                }
            }
        }

        protected void setup_update_loop(Scene_Map scene_map)
        {
            switch (Skill_Flash_Timer)
            {
                case 0:
                    Global.scene.suspend();
                    Global.game_system.Battler_1_Id = -1;
                    Global.game_system.Battler_2_Id = -1;
                    Skill_Flash_Timer++;

                    skill_flasher.battling = true;
                    skill_flasher.frame = 0;
                    if(has_target)
                        skill_flasher.face(skill_flash_target);
                    Global.game_map.move_range_visible = false;

                    skill_setup(scene_map);
                    break;
                case 6:
                    Skill_Flash_Phase++;
                    Skill_Flash_Action = 0;
                    Skill_Flash_Timer = 0;
                    break;
                default:
                    Skill_Flash_Timer++;
                    break;
            }
        }

        protected void skill_setup(Scene_Map scene_map)
        {
            if(identifier == "VOLLEY")
            {
                setup_volley();
                return;
            }
            if(identifier == "DREADFUL_AURA")
            {
                setup_dreadful_aura();
                return;
            }
        }

        protected void skill_icon_flash_animation(Scene_Map scene_map, bool cont)
        {
            switch (Skill_Flash_Action)
            {
                case 0:
                    // shouldn't be hardcoded //Yeti
                    scene_map.set_map_effect(skill_flasher.loc + new Vector2(0, -1), 5,
                        image_index);
                    Skill_Flash_Action++;
                    break;
                case 1:
                    switch (Skill_Flash_Timer)
                    {
                        case 40:

                            Skill_Flash_Phase++;
                            Skill_Flash_Timer = 0;
                            break;
                        default:
                            Skill_Flash_Timer++;
                            break;
                    }
                    break;
                case 2:
                    {
                        switch (Skill_Flash_Timer)
                        {
                            case 0:
                                if (!scene_map.is_map_effect_active())
                                    if (get_scene_map().combat_hud_ready())
                                    {
                                        Skill_Flash_Timer++;
                                    }
                                break;
                            case 46:
                                Skill_Flash_Action++;
                                Skill_Flash_Timer = 0;
                                break;
                            default:
                                Skill_Flash_Timer++;
                                break;
                        }
                        break;
                    }
                case 3:
                    switch (Skill_Flash_Timer)
                    {
                        case 0:

                            get_scene_map().clear_combat();
                            Skill_Flash_Phase++;
                            Skill_Flash_Action = 0;
                            Skill_Flash_Timer = 0;
                            cont = false;
                            break;
                        default:
                            Skill_Flash_Timer++;
                            break;
                    }
                    break;
            }
        }

        protected void apply_skill_effect(Scene_Map scene_map)
        {
            if (do_hit_flash)
                apply_hit_flash(scene_map);
            if (identifier == "VOLLEY")
            {
                apply_volley();
                return;
            }
            if (identifier == "RALLY_LUCK" || identifier == "RALLY_RES" || identifier == "RALLY_SPECTRUM" )
            {
                apply_rally();
                return;
            }
            if (identifier == "DISARM")
            {
                apply_disarm();
                return;
            }
            if (identifier == "GALEFORCE")
            {
                apply_galeforce();
                return;
            }
            if (identifier == "POISON_KNIFE")
            {
                apply_poison_knife();
                return;
            }
            if (identifier == "GRISLY_WOUND")
            {
                apply_grisly_wound();
                return;
            }
            if (identifier == "LIFETAKER")
            {
                apply_lifetaker();
                return;
            }
            if (identifier == "INDICT")
            {
                apply_indict();
                return;
            }
            if (identifier == "SHATTER_DEFENSE")
            {
                apply_shatter_defense();
                return;
            }
            if (identifier == "DRACONIC_HEX")
            {
                apply_draconic_hex();
                return;
            }
            if (identifier == "DREADFUL_AURA")
            {
                apply_dreadful_aura();
                return;
            }
        }

        protected void apply_hit_flash(Scene_Map scene_map)
        {
            switch (Skill_Flash_Timer)
            {
                case 1:
                    if (target_alive || Skill_Targets.Count > 0)
                        Global.Audio.play_se("Map Sounds", "Hit");
                    if (Skill_Targets.Count > 0)
                    {
                        foreach(int id in Skill_Targets)
                        {
                            Game_Unit unit = Global.game_map.units[id];
                            unit.hit_color();
                            scene_map.update_map_sprite_status(id);
                        }
                    }
                    else if (target_alive)
                    {
                        skill_flash_target.hit_color();
                        scene_map.update_map_sprite_status(skill_flash_target_id);
                    }
                    break;
                default:
                    if (Skill_Targets.Count > 0)
                    {
                        foreach (int id in Skill_Targets)
                        {
                            Game_Unit unit = Global.game_map.units[id];
                            unit.update_attack_graphics();
                        }
                    }
                    else if (target_alive)
                        skill_flash_target.update_attack_graphics(); //This only works if Scene_Map considers the target an "active unit"
                    break;
            }
        }
        // Skills: Rally Luck / Rally Res / Rally Spectrum
        protected void apply_rally()
        {
            skill_flasher.rally();

            Skill_Flash_Phase++;
        }
        // Skills: Volley
        protected void setup_volley()
        {
            foreach (int id in skill_flash_target.units_in_range(1))
            {
                Game_Unit unit = Global.game_map.units[id];
                if (skill_flasher.is_attackable_team(unit))
                {
                    Skill_Targets.Add(id);
                }
            }
            if(target_alive)
                Skill_Targets.Add(skill_flash_target_id);
        }
        protected void apply_volley()
        {
            switch (Skill_Flash_Timer)
            {
                case 4:
                    foreach (int id in Skill_Targets)
                    {
                        Game_Unit unit = Global.game_map.units[id];
                        unit.hp -= Math.Min(unit.hp - 1, (int)(unit.maxhp * 0.2));
                    }
                    Skill_Flash_Timer++;
                    break;
                case 25:
                    Skill_Flash_Phase++;
                    Skill_Flash_Timer = 0;
                    break;
                default:
                    Skill_Flash_Timer++;
                    break;
            }
        }
        // Skills: Poison Knife
        protected void apply_poison_knife()
        {
            
            switch (Skill_Flash_Timer)
            {
                case 4:
                    skill_flash_target.hp -= Math.Min(skill_flash_target.hp - 1, (int)(skill_flash_target.maxhp * 0.2));
                    Skill_Flash_Timer++;
                    break;
                case 25:
                    Skill_Flash_Phase++;
                    Skill_Flash_Timer = 0;
                    break;
                default:
                    Skill_Flash_Timer++;
                    break;
            }
        }
        // Grisly Wound
        protected void apply_grisly_wound()
        {

            switch (Skill_Flash_Timer)
            {
                case 4:
                    skill_flash_target.hp -= Math.Min(skill_flash_target.hp - 1, (int)(skill_flash_target.maxhp * 0.2));
                    Skill_Flash_Timer++;
                    break;
                case 25:
                    Skill_Flash_Phase++;
                    Skill_Flash_Timer = 0;
                    break;
                default:
                    Skill_Flash_Timer++;
                    break;
            }
        }
        // Skills: Disarm
        protected void apply_disarm()
        {
            skill_flash_target.actor.unequip();
            Skill_Flash_Phase++;
        }
        // Skills: Galeforce
        protected void apply_galeforce()
        {
            skill_flasher.activate_galeforce();
            Skill_Flash_Phase++;
        }
        
        // Skills: Lifetaker
        protected void apply_lifetaker()
        {
            switch (Skill_Flash_Timer)
            {
                case 0:
                    get_scene_map().create_hud(skill_flasher_id);
                    skill_flasher.hp += skill_flasher.maxhp / 2;
                    Skill_Flash_Timer++;
                    break;
                default:
                    if (get_scene_map().combat_hud_ready())
                    {
                        get_scene_map().clear_combat();
                        Skill_Flash_Phase++;
                        Skill_Flash_Timer = 0;
                        break;
                    }
                    Skill_Flash_Timer++;
                    break;
            }
        }
        // Skills: Indict
        protected void apply_indict()
        {
            skill_flash_target.actor.add_state(26); //Indicted Status
            Skill_Flash_Phase++;
        }
        // Skills: Shatter Defense
        protected void apply_shatter_defense()
        {
            skill_flash_target.shatter_defense();
            Skill_Flash_Phase++;
        }
        // Skills: Draconic Hex
        protected void apply_draconic_hex()
        {
            skill_flash_target.draconic_hex();
            Skill_Flash_Phase++;
        }
        // Skills: Dreadful Aura
        protected void setup_dreadful_aura()
        {
            foreach (int id in skill_flash_target.units_in_range(1))
            {
                Game_Unit unit = Global.game_map.units[id];
                if (skill_flasher.is_attackable_team(unit))
                {
                    Skill_Targets.Add(id);
                }
            }
            if (target_alive)
                Skill_Targets.Add(skill_flash_target_id);
        }
        protected void apply_dreadful_aura()
        {
            foreach (int id in Skill_Targets)
            {
                Game_Unit target = Global.game_map.units[id];
                target.actor.add_state(30);
            }
            Skill_Flash_Phase++;
        }

        protected void end_skill_flash()
        {
            switch (Skill_Flash_Timer)
            {
                case 0:
                    /*
                    if (Skill_Flasher.cantoing && Skill_Flasher.is_active_player_team) //Multi
                    {
                        Global.player.loc = Skill_Flasher.loc;
                        Global.player.instant_move = true;
                        Skill_Flasher.open_move_range();

                    }
                    else
                        Skill_Flasher.start_wait(false);
                    */
                    skill_flasher.battling = false;
                    skill_flasher.queue_move_range_update();
                    if(has_target && target_alive)
                        skill_flash_target.queue_move_range_update();
                    refresh_move_ranges();
                    Skill_Targets.Clear();
                    Skill_Flash_Timer++;
                    break;
                case 1:
                    if (!Global.game_system.is_interpreter_running && !Global.scene.is_message_window_active)
                    {
                        Skill_Flash_Timer++;
                    }
                    break;
                case 2:
                    Skill_Flash_Phase = 0;
                    Skill_Flash_Action = 0;
                    Skill_Flash_Timer = 0;
                    Identifier_Queue.RemoveAt(0);
                    Skill_Flasher_Id_Queue.RemoveAt(0);
                    Skill_Flash_Target_Id_Queue.RemoveAt(0);
                    if (Identifier_Queue.Count == 0)
                    {
                        Skill_Flash_Calling = false;
                        In_Skill_Flash = false;
                        Global.game_map.move_range_visible = true;
                        highlight_test();
                        Global.game_state.any_trigger_events();
                        break;
                    }
                    else
                    {
                        Skill_Flash_Calling = true;
                        break;
                    }
            }
        }

        public void Draw_Target_HP_Gauges(SpriteBatch sprite_batch, Vector2 hp_gauge_draw_vector, Camera camera, Dictionary<int, Character_Sprite> Map_Sprites)
        {
            if (!Global.game_system.is_interpreter_running)
            {
                foreach (int id in skill_targets)
                {
                    if (Global.game_map.units.ContainsKey(id))
                    {
                        if (!Global.game_map.units[id].visible_by() || Global.game_map.is_off_map(Global.game_map.units[id].loc))
                            continue;
                        Map_Sprites[id].draw_hp(sprite_batch, Global.game_map.display_loc - hp_gauge_draw_vector, camera.matrix);
                    }
                }
                if (skill_targets.Count == 0)
                {
                    int id = Global.game_state.skill_flash_target;
                    if (Global.game_map.units.ContainsKey(id))
                        Map_Sprites[id].draw_hp(sprite_batch, Global.game_map.display_loc - hp_gauge_draw_vector, camera.matrix);
                }
            }
        }
    }
}

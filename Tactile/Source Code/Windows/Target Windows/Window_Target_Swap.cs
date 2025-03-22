using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Map;
using Tactile.Graphics.Text;
using Tactile.Graphics.Windows;

namespace Tactile.Windows.Target
{
    class Window_Target_Swap : Window_Target_Unit
    {
        SystemWindowHeadered Window, Target_Window;
        Character_Sprite Unit_Sprite, Target_Sprite;
        TextSprite Name1, Aid_Label, Aid_Value;
        TextSprite Name2, Con_Label, Con_Value;
        Hand_Cursor Hand;
        Sprite Rescue_Icon;

        #region Accessors

        protected override int window_width
        {
            get { return 80; }
        }
        #endregion

        public Window_Target_Swap(int unit_id, Vector2 loc)
        {
            initialize(loc);
            Right_X = Config.WINDOW_WIDTH - this.window_width;
            Unit_Id = unit_id;
            List<int> targets = get_targets();
            Targets = sort_targets(targets);
            this.index = 0;
            Temp_Index = this.index;
            cursor_move_to(this.target);

            Global.player.instant_move = true;
            Global.player.update_movement();
            initialize_images();
            refresh();
            index = this.index;
        }

        protected override List<int> sort_targets(List<int> targets)
        {
            Game_Unit unit = get_unit();
            targets.Sort(delegate (int a, int b)
            {
                Vector2 loc1, loc2;
                int dist1, dist2;
                loc1 = new Vector2(a % Global.game_map.width,
                    a / Global.game_map.width);
                loc2 = new Vector2(b % Global.game_map.width,
                    b / Global.game_map.width);
                dist1 = (int)(Math.Abs(unit.loc.X - loc1.X) + Math.Abs(unit.loc.Y - loc1.Y));
                dist2 = (int)(Math.Abs(unit.loc.X - loc2.X) + Math.Abs(unit.loc.Y - loc2.Y));
                int angle1 = ((360 - unit.angle(loc1)) + 90) % 360;
                int angle2 = ((360 - unit.angle(loc2)) + 90) % 360;
                return (angle1 == angle2 ?
                    (loc1.Y == loc2.Y ? dist1 - dist2 : (int)(loc1.Y - loc2.Y)) :
                    angle1 - angle2);
            });
            return targets;
        }

        protected List<int> get_targets()
        {
            Game_Unit unit = get_unit();
            return unit.swap_targets();
        }

        protected void initialize_images()
        {
            // Windows
            Window = new SystemWindowHeadered();
            Window.width = this.window_width;
            Window.height = 48;
            Window.draw_offset = new Vector2(0, 0);
            Target_Window = new SystemWindowHeadered();
            Target_Window.width = this.window_width;
            Target_Window.height = 48;
            Target_Window.draw_offset = new Vector2(0, 0);
            // Map Sprites
            Unit_Sprite = new Character_Sprite();
            Unit_Sprite.draw_offset = new Vector2(20, 24 + 0);
            Unit_Sprite.facing_count = 3;
            Unit_Sprite.frame_count = 3;
            Target_Sprite = new Character_Sprite();
            Target_Sprite.draw_offset = new Vector2(20, 24 + 0);
            Target_Sprite.facing_count = 3;
            Target_Sprite.frame_count = 3;
            // Names
            Name1 = new TextSprite();
            Name1.draw_offset = new Vector2(32, 8 + 0);
            Name1.SetFont(Config.UI_FONT, Global.Content, "White");
            Name2 = new TextSprite();
            Name2.draw_offset = new Vector2(32, 8 + 0);
            Name2.SetFont(Config.UI_FONT, Global.Content, "White");
            //Name1, , Aid_Value;
            //Name2, Con_Label, Con_Value;
            // Labels
            Aid_Label = new TextSprite();
            Aid_Label.draw_offset = new Vector2(8, 24 + (0));
            Aid_Label.SetFont(Config.UI_FONT, Global.Content, "Yellow");
            Aid_Label.text = "Aid";
            Con_Label = new TextSprite();
            Con_Label.draw_offset = new Vector2(8, 24 + 0);
            Con_Label.SetFont(Config.UI_FONT, Global.Content, "Yellow");
            Con_Label.text = "Con";
            // Stats
            Aid_Value = new RightAdjustedText();
            Aid_Value.draw_offset = new Vector2(72, 24 + 0);
            Aid_Value.SetFont(Config.UI_FONT, Global.Content, "Blue");
            Con_Value = new RightAdjustedText();
            Con_Value.draw_offset = new Vector2(72, 24 + 0);
            Con_Value.SetFont(Config.UI_FONT, Global.Content, "Blue");
            // Hand
            Hand = new Hand_Cursor();
            Hand.offset = new Vector2(8, 0);
            Hand.draw_offset = new Vector2(48, 47);
            Hand.mirrored = true;
            Hand.angle = MathHelper.PiOver2;
            // Rescue Icon
            Rescue_Icon = new Sprite();
            Rescue_Icon.texture = Global.Content.Load<Texture2D>(@"Graphics/Characters/RescueIcon");

            set_images();
        }

        protected override void set_images()
        {


            return;
        }

        protected override void refresh()
        {
            Window.loc = Loc;
            Target_Window.loc = Loc;
            Unit_Sprite.loc = Loc;
            Target_Sprite.loc = Loc;

            Name1.loc = Loc;
            Name2.loc = Loc;
            Aid_Label.loc = Loc;
            Con_Label.loc = Loc;
            Aid_Value.loc = Loc;
            Con_Value.loc = Loc;
            Hand.loc = Loc;
            Rescue_Icon.loc = Loc;
        }

        protected override void update_end(int temp_index)
        {
            update_frame();
        }

        protected void update_frame()
        {
            int frame = Global.game_system.unit_anim_idle_frame;
            Unit_Sprite.frame = frame;
            Target_Sprite.frame = frame;
        }

        protected override void move_down()
        {
            base.move_down();
            move_timer_reset();
        }
        protected override void move_up()
        {
            base.move_up();
            move_timer_reset();
        }
        protected override void move_to(int index)
        {
            base.move_to(index);
            move_timer_reset();
        }

        protected void move_timer_reset()
        {
            // \o_O/ //Yeti
        }

        protected override void reset_cursor()
        {
            cursor_move_to(Global.game_map.units[Targets[Temp_Index]]);
        }

        protected void cursor_move_to(int target)
        {
            Vector2 loc;
            loc = Global.game_map.units[target].loc_on_map();
            Global.player.loc = loc;
        }

        internal override Vector2 target_loc(int target)
        {

            Combat_Map_Object unit1 = Global.game_map.attackable_map_object(target);
            return unit1.loc_on_map();
        }

        public override void draw(SpriteBatch sprite_batch)
        {
            /*
            if (mode != 1)
            {
                sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Window.draw(sprite_batch);
                Target_Window.draw(sprite_batch);
                Unit_Sprite.draw(sprite_batch);
                Target_Sprite.draw(sprite_batch);

                Name1.draw(sprite_batch);
                Name2.draw(sprite_batch);
                Aid_Label.draw(sprite_batch);
                Con_Label.draw(sprite_batch);
                Aid_Value.draw(sprite_batch);
                Con_Value.draw(sprite_batch);
                Hand.draw(sprite_batch);
                if (Global.game_map.icons_visible)
                    Rescue_Icon.draw(sprite_batch);
                sprite_batch.End();
            }
            */
        }
    }
}
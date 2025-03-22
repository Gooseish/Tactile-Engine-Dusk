using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TactileLibrary;

namespace Tactile.Source_Code.Sprites
{
    class Unit_Info_Skill
    {
        protected List<Icon_Sprite> SkillIcons;
        protected Vector2 loc;

        public Unit_Info_Skill()
        {

        }

        internal void refresh(Game_Unit unit)
        {
            SkillIcons = new List<Icon_Sprite>();
            if (unit != null)
            {
                int counter = 0;
                foreach (int skillid in unit.actor.skills)
                {
                    Data_Skill data = Global.data_skills[skillid];
                    refresh_skill(data, counter);

                    counter++;
                }
            }
        }

        protected virtual void refresh_skill(Data_Skill data, int counter)
        {
            Icon_Sprite SkillIcon = new Icon_Sprite();

            SkillIcon.size = new Vector2(Config.SKILL_ICON_SIZE, Config.SKILL_ICON_SIZE);
            SkillIcon.draw_offset = new Vector2(0, 0);

            SkillIcon.texture = null;

            SkillIcon.visible = true;

            SkillIcon.texture = null;
            if (Global.content_exists(@"Graphics/Icons/" + data.Image_Name))
            {
                SkillIcon.texture = Global.Content.Load<Texture2D>(@"Graphics/Icons/" + data.Image_Name);
                SkillIcon.index = data.Image_Index;
#if DEBUG
                SkillIcon.tint = Color.White;
            }
            else
            {
                SkillIcon.texture = Global.Content.Load<Texture2D>(@"Graphics/White_Square");
                SkillIcon.tint = Color.Black;
                SkillIcon.index = 0;
#endif
            }
            SkillIcon.loc = new Vector2(17f * (counter / 3), 17f * (counter % 3));

            SkillIcons.Add(SkillIcon);

        }

        public void draw(SpriteBatch sprite_batch, Vector2 draw_offset)
        {
            foreach (Icon_Sprite SkillIcon in SkillIcons)
            {
                SkillIcon.draw(sprite_batch, draw_offset);
            }
        }

        protected void update_graphics()
        {
            foreach(Icon_Sprite SkillIcon in SkillIcons)
                SkillIcon.update();
        }

    }
}

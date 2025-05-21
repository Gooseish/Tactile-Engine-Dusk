using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Text;
using TactileLibrary;

namespace Tactile.Windows
{
    class Window_Turnwheel : Stereoscopic_Graphic_Object
    {
        protected TextSprite Turn_Number;
        public int turn_number { set { Turn_Number.text = "Turn: " + value.ToString(); } }
        public Window_Turnwheel()
        {
            Turn_Number = new TextSprite();
            Turn_Number.SetFont(Config.UI_FONT, Global.Content, "White");
        }

        public void draw(SpriteBatch sprite_batch)
        {
            draw(sprite_batch, Vector2.Zero);
        }
        public virtual void draw(SpriteBatch sprite_batch, Vector2 draw_offset)
        {
            draw(sprite_batch, draw_offset, null);
        }
        public virtual void draw(SpriteBatch sprite_batch, Vector2 draw_offset, RasterizerState state)
        {
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, state);
            Turn_Number.draw(sprite_batch, draw_offset - (loc + draw_vector() + new Vector2(16, 0) - offset));
            sprite_batch.End();
        }
    }
}

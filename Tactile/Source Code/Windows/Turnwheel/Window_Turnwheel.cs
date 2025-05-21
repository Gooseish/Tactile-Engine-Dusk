using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Text;
using Tactile.Graphics.Windows;


namespace Tactile.Windows
{
    class Window_Turnwheel : Stereoscopic_Graphic_Object
    {
        protected WindowPanel Window_Img;
        protected TextSprite Turn_Number;
        public int turn_number { set { Turn_Number.text = "Turn: " + value.ToString(); } }

        public Window_Turnwheel()
        {
            Turn_Number = new TextSprite();
            Turn_Number.SetFont(Config.UI_FONT, Global.Content, "White");
            Window_Img = new WindowPanel(Global.Content.Load<Texture2D>(System_Color_Window.FILENAME));
            Window_Img.width = 55;
            Window_Img.height = 32;
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
            Window_Img.draw(sprite_batch, draw_offset - (loc + draw_vector() + new Vector2(8, -7) - offset));
            Turn_Number.draw(sprite_batch, draw_offset - (loc + draw_vector() + new Vector2(16, 0) - offset));
            sprite_batch.End();
        }
    }
}

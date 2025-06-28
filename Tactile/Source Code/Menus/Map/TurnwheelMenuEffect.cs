using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tactile.Graphics.Help;
using Tactile.Windows.Command;
using Tactile.Windows;

namespace Tactile.Menus.Map.Turnwheel
{
    class TurnwheelMenuEffect
    {
        private Texture2D @Texture;
        private Random random;
        private int Timer;
        Color[] data;
        private List<TurnwheelMenuEffectPixel> Pixels;
        private int Scale = 4;
        private Effect Ripple;

        public TurnwheelMenuEffect()
        {
            Timer = 0;
            set_pixels();
            refresh_texture();
            random = new Random();
            Ripple = Global.effect_shader();
            //Ripple.Parameters["timer"].SetValue(Timer);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            //RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT);
            //graphicsDevice.Clear(Color.CornflowerBlue);
            //graphicsDevice.SetRenderTarget(renderTarget);
            //SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);

            Ripple.CurrentTechnique = Ripple.Techniques["Ripple"];

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Ripple);
            spriteBatch.Draw(Texture, new Rectangle(0, 0, Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT), new Rectangle(0, 0, Config.WINDOW_WIDTH / Scale, Config.WINDOW_HEIGHT / Scale), Color.White);
            spriteBatch.End();
            /*
            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Ripple);
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT), Color.White);
            spriteBatch.End();*/
        }
        public void set_pixels()
        {
            Pixels = new List<TurnwheelMenuEffectPixel> { };
            int columns = Config.WINDOW_WIDTH / Scale;
            int rows = Config.WINDOW_HEIGHT / Scale;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    TurnwheelMenuEffectPixel pixel = new TurnwheelMenuEffectPixel(new Vector2(c, r));
                    Pixels.Add(pixel);
                }
            }
        }
        public void refresh_texture()
        {
            Texture = (Global.Content as ContentManagers.ThreadSafeContentManager)
                .texture_from_size(Config.WINDOW_WIDTH / Scale, Config.WINDOW_HEIGHT / Scale);
            data = new Color[Config.WINDOW_WIDTH / Scale * Config.WINDOW_HEIGHT / Scale];
            int width = Config.WINDOW_WIDTH / Scale;
            foreach (TurnwheelMenuEffectPixel pixel in Pixels)
            {
                int index = (int)pixel.loc.Y * width + (int)pixel.loc.X;
                data[index] = pixel.color;
            }

            Texture.SetData(data);
        }

        public void update()
        {
            update_pixels();
            twinkle();
            refresh_texture();
            Timer++;
            //Ripple.Parameters["timer"].SetValue(Timer);
        }
        public void update_pixels()
        {
            foreach (TurnwheelMenuEffectPixel pixel in Pixels)
            {
                pixel.update();
            }
        }
        public void twinkle()
        {
            if (Timer % 10 != 0)
                return;
            for (int n = 0; n < 10; n++)
            {
                int random_pixel = random.Next(0, Pixels.Count());
                Pixels[random_pixel].excite();
            }
        }
    }

    class TurnwheelMenuEffectPixel
    {
        private Vector2 Loc;
        private float Temperature;
        private bool Heating_Up;

        public Vector2 loc { get { return Loc; } }
        public int x { get { return (int)loc.X; } }
        public int y { get { return (int)loc.Y; } }
        public float temperature { get { return Temperature; } }
        public Color color
        {
            get
            {
                Vector4 values = new Vector4(0.55f, 0.1f, 0.55f, 0.1f);
                Vector4 base_color = new Vector4(0.08f, 0.01f, 0.08f, 0.1f);
                Vector4 result = Vector4.Lerp(base_color, values, Temperature);
                return new Color(result);
            }
        }
        public TurnwheelMenuEffectPixel(Vector2 loc)
        {
            Loc = loc;
            Temperature = 0;
        }

        public void update()
        {
            if (Heating_Up)
            {
                Temperature += 0.05f;
                if (Temperature >= 0.75f)
                    Heating_Up = false;
            }
            else
                Temperature *= (float)Math.Exp(-Temperature / 10);
        }
        public void excite()
        {
            Heating_Up = true;
        }
    }
}

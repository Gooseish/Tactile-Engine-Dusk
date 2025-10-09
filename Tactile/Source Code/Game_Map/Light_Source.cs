using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using ArrayExtension;
using HashSetExtension;
using ListExtension;
using RectangleExtension;
using TactileArrayExtension;
using TactileVector2Extension;
using TactileDictionaryExtension;
using TactileListExtension;
using TactileColorExtension;

namespace Tactile
{
    class Light_Source
    {
        private Color @Color;
        private Vector2 Loc;
        private int Max_Steps;
        private Texture2D Lightmap_Contribution;
        private byte[,] Brightness_Map;

        public Texture2D lightmap_contribution { get { return Lightmap_Contribution; } }

        public void write(BinaryWriter writer)
        {
            Color.write(writer);
            Loc.write(writer);
            Brightness_Map.write(writer);
        }
        public void read(BinaryReader reader)
        {
            Color.read(reader);
            Loc.read(reader);
            Brightness_Map = Brightness_Map.read(reader);
            refresh_light_texture();
        }

        public Color color { 
            get { return Color; } 
            set 
            { 
                Color = value;
            }
        }
        public Vector2 loc { get { return Loc; } }

        public Light_Source(Color color, Vector2 loc)
        {
            Color = color;
            Loc = loc;
        }

        public void calculate_lightmap(byte[,] cost_map)
        {
            set_max_steps();
            Brightness_Map = new byte[cost_map.GetLength(0), cost_map.GetLength(1)];
            for (int x = 0; x < cost_map.GetLength(0); x++)
                for (int y = 0; y < cost_map.GetLength(1); y++)
                {
                    Brightness_Map[x, y] = raymarch_brightness(x, y, cost_map);
                }

            refresh_light_texture();
        }

        byte raymarch_brightness(int x, int y, byte[,] cost_map)
        {
            byte brightness = Color.A;
            Vector2 pixel_location = new Vector2(x, y);
            Vector2 difference_vector = pixel_location - loc*Constants.Map.ALPHA_GRANULARITY;
            Vector2 step_vector = Vector2.Normalize(difference_vector) / Constants.Map.SUBPIXEL_GRANULARITY;

            int number_of_steps = (int)(difference_vector.Length() / step_vector.Length());
            if (number_of_steps > Max_Steps)
            {
                return 0;
            }

            Vector2 temp_vector = loc * Constants.Map.ALPHA_GRANULARITY;
            for (int n = 0; n < number_of_steps; n++)
            {
                byte brightness_cost = cost_map[(int)temp_vector.X, (int)temp_vector.Y];
                if (brightness <= brightness_cost)
                    return 0;
                brightness -= brightness_cost;
                temp_vector += step_vector;
            }
            return brightness;
        }

        public void refresh_light_texture()
        {
            Lightmap_Contribution = (Global.Content as ContentManagers.ThreadSafeContentManager).texture_from_size(Brightness_Map.GetLength(0), Brightness_Map.GetLength(1));
            Color[] texture_data = new Color[Brightness_Map.Length];
            int n = 0;
            for (int y = 0; y < Lightmap_Contribution.Height; y++)
                for (int x = 0; x < Lightmap_Contribution.Width; x++)
                {
                    float brightness_scalar = (float)Brightness_Map[x, y] / 256f;
                    Vector4 pixel_color = Color.ToVector4();

                    // Premultiplied alpha
                    pixel_color.X *= brightness_scalar;
                    pixel_color.Y *= brightness_scalar;
                    pixel_color.Z *= brightness_scalar;
                    pixel_color.W = brightness_scalar;

                    texture_data[n] = new Color(pixel_color);

                    n++;
                }
            Lightmap_Contribution.SetData(texture_data);
        }

        public void set_max_steps()
        {
            Max_Steps = Color.A / Constants.Map.BASE_SUBPIXEL_BRIGHTNESS_COST;
        }
    }
}

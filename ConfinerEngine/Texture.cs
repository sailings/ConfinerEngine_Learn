using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfinerEngine
{
    public class Texture
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector4F[] Buffer { get; set; }

        public static Texture Texture_Create(int width, int height)
        {
            int buffer_size = width * height;
            Texture texture = new Texture();
            //texture = (texture_t*)malloc(sizeof(texture_t));
            texture.Width = width;
            texture.Height = height;
            texture.Buffer = new Vector4F[buffer_size];
            //memset(texture->buffer, 0, buffer_size);
            return texture;
        }

        public Vector4F texture_repeat_sample(Vector2F texcoord)
        {
            float u = texcoord.x - (float)Math.Floor(texcoord.x);
            float v = texcoord.y - (float)Math.Floor(texcoord.y);
            int c = (int)((Width - 1) * u);
            int r = (int)((Height - 1) * v);
            int index = r * Width + c;
            return Buffer[index];
        }

        public static float uchar_to_float(byte value)
        {
            return value / 255.0f;
        }

        public static Texture Texture_From_Image(Image image)
        {
            int width = image.Width;
            int height = image.Height;
            int channels = image.Channels;
            Texture texture;
            int r, c;

            texture = Texture_Create(width, height);
            for (r = 0; r < height; r++)
            {
                for (c = 0; c < width; c++)
                {
                    int img_index = (r * width + c) * channels;
                    int tex_index = r * width + c;
                    var pixel = image.Buffer[img_index];
                    //var texel = texture.Buffer[tex_index];
                    if (channels == 1)
                    {         /* GL_LUMINANCE */
                        //float luminance = uchar_to_float(pixel[0]);
                        float luminance = uchar_to_float(image.Buffer[img_index]);
                        texture.Buffer[tex_index] = new Vector4F(luminance, luminance, luminance, 1);
                    }
                    else if (channels == 2)
                    {  /* GL_LUMINANCE_ALPHA */
                        //float luminance = uchar_to_float(pixel[0]);
                        //float alpha = uchar_to_float(pixel[1]);
                        float luminance = uchar_to_float(image.Buffer[img_index]);
                        float alpha = uchar_to_float(image.Buffer[img_index + 1]);
                        texture.Buffer[tex_index] = new Vector4F(luminance, luminance, luminance, alpha);
                    }
                    else if (channels == 3)
                    {  /* GL_RGB */
                        //float blue = uchar_to_float(pixel[0]);
                        //float green = uchar_to_float(pixel[1]);
                        //float red = uchar_to_float(pixel[2]);
                        float blue = uchar_to_float(image.Buffer[img_index]);
                        float green = uchar_to_float(image.Buffer[img_index+1]);
                        float red = uchar_to_float(image.Buffer[img_index+2]);
                        texture.Buffer[tex_index] = new Vector4F(red, green, blue, 1);
                    }
                    else if (channels == 4)
                    {  /* GL_RGBA */
                        //float blue = uchar_to_float(pixel[0]);
                        //float green = uchar_to_float(pixel[1]);
                        //float red = uchar_to_float(pixel[2]);
                        //float alpha = uchar_to_float(pixel[3]);
                        float blue = uchar_to_float(image.Buffer[img_index]);
                        float green = uchar_to_float(image.Buffer[img_index+1]);
                        float red = uchar_to_float(image.Buffer[img_index+2]);
                        float alpha = uchar_to_float(image.Buffer[img_index+3]);
                        texture.Buffer[tex_index] = new Vector4F(red, green, blue, alpha);
                    }
                }
            }

            return texture;
        }
    }
}
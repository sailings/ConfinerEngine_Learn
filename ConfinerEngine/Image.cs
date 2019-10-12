using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConfinerEngine
{
    public class Image
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int Channels { get; set; }

        public byte[] Buffer { get; set; }

        private const int TGA_HEADER_SIZE = 18;

        public int get_buffer_size()
        {
            return Width * Height * Channels;
        }

        public static Image Image_Create(int width, int height, int channels)
        {
            int buffer_size = width * height * channels;
            Image image = new Image();
            //image = (image_t*)malloc(sizeof(image_t));
            image.Width = width;
            image.Height = height;
            image.Channels = channels;
            //image->buffer = (unsigned char*)malloc(buffer_size);
            image.Buffer = new byte[buffer_size];
            //memset(image->buffer, 0, buffer_size);
            return image;
        }

        private static void Load_Tga_Rle(FileStream fs, Image image)
        {
            byte[] buffer = image.Buffer;
            int channels = image.Channels;
            int buffer_size = image.get_buffer_size();
            int elem_count = 0;
            while (elem_count < buffer_size)
            {
                //char header = read_byte(file);
                int header = fs.ReadByte();
                int rle_packet = header & 0x80;
                int pixel_count = (header & 0x7F) + 1;
                int[] pixel = new int[4];
                int i, j;
                if (rle_packet > 0)
                {  /* rle packet */
                    for (j = 0; j < channels; j++)
                    {
                        //pixel[j] = read_byte(file);
                        //pixel[j] = (char)sr.Read();
                        pixel[j] = fs.ReadByte();
                    }
                    for (i = 0; i < pixel_count; i++)
                    {
                        for (j = 0; j < channels; j++)
                        {
                            buffer[elem_count++] = (byte)pixel[j];
                        }
                    }
                }
                else
                {           /* raw packet */
                    for (i = 0; i < pixel_count; i++)
                    {
                        for (j = 0; j < channels; j++)
                        {
                            //buffer[elem_count++] = read_byte(file);
                            buffer[elem_count++] = (byte)fs.ReadByte();
                        }
                    }
                }
            }
        }

        public static Image Load_Tga(string filename)
        {
            byte[] header = new byte[TGA_HEADER_SIZE];
            int width, height, depth, channels;
            int idlength, imgtype, imgdesc;
            Image image = new Image();
            //FILE* file;
            using (FileStream fs = new FileStream(filename,FileMode.Open))
            {
                fs.Read(header, 0, TGA_HEADER_SIZE);
                width = header[12] | (header[13] << 8);
                height = header[14] | (header[15] << 8);
                depth = header[16];
                channels = depth / 8;
                image = Image_Create(width, height, channels);

                idlength = header[0];
                imgtype = header[2];
                if (imgtype == 2 || imgtype == 3)
                {           /* uncompressed */
                    //read_bytes(file, image->buffer, get_buffer_size(image));
                    fs.Read(image.Buffer, TGA_HEADER_SIZE, image.get_buffer_size());
                }
                else if (imgtype == 10 || imgtype == 11)
                {  /* run-length encoded */
                    //load_tga_rle(file, image);
                    Load_Tga_Rle(fs, image);
                }

            }
            
            //imgdesc = header[17];
            //    if (imgdesc & 0x20) {
            //        image_flip_v(image);
            //    }
            //    if (imgdesc & 0x10) {
            //        image_flip_h(image);
            //    }
            return image;
        }
    }
}
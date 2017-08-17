using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.IO
{
    
    class ContentPipe
    {
        public static List<TextureInfo> TextureInfoDatabase = new List<TextureInfo>();
        public struct TextureInfo
        {
            public string path;
            public int id;
            public TextureUnit textureUnit;

            public TextureInfo(string path, int id, TextureUnit textureUnit) : this()
            {
                this.path = path;
                this.id = id;
                this.textureUnit = textureUnit;
            }
        }
        static int txi = 0;
        public static TextureUnit GetUnitFromID(int texture)
        {
            foreach (TextureInfo tinfo in TextureInfoDatabase)
            {
                if (tinfo.id == texture)
                {
                    return tinfo.textureUnit;
                }
            }
            return TextureUnit.Texture31;
        }
        public static int TextureAlreadyLoaded(string path)
        {
            foreach(TextureInfo tinfo in TextureInfoDatabase)
            {
                if(tinfo.path == path)
                {
                    return tinfo.id;
                }
            }
            return -1;
        }
        public static int Load_and_AddTexture(string path)
        {
            if (!File.Exists(path)) return -1;
            try { Image.FromFile(path); }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
            int texture = TextureAlreadyLoaded(path);

            if (texture == -1)
            {
                texture = GL.GenTexture();
                LoadTexture(path, texture, TextureUnit.Texture0+txi);
                TextureInfoDatabase.Add(new TextureInfo(path, texture, TextureUnit.Texture0+txi));
                
                txi++;
                return texture;
            }
            return texture;
        }
        public static void LoadTexture(string path, int id, TextureUnit Unit)
        {
            GL.ActiveTexture(Unit);
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(path);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            
        }
    }
}

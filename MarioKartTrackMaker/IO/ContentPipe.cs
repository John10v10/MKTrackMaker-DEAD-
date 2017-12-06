using System;
using System.IO;
using System.Drawing;
//using System.Drawing.Imaging;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.IO
{

    /// <summary>
    /// The content pipe is the bridge to import all the textures into the viewport.
    /// </summary>
    class ContentPipe
    {
        /// <summary>
        /// This database contains all the loaded textures.
        /// </summary>
        public static List<TextureInfo> TextureInfoDatabase = new List<TextureInfo>();
        /// <summary>
        /// This structure contains all the required fields of a texture.
        /// </summary>
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
        /// <summary>
        /// I don't recommend you touch this, but it counts how many new textures are imported.
        /// </summary>
        static int txi = 0;
        /// <summary>
        /// Gets the texture unit of a texture with just the texture id.
        /// </summary>
        /// <param name="texture">The id of the texture to find the texture unit.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Using the filepath of the texture, it checks to see if it has already been loaded. If it has, it'll return the id of the already loaded texture.
        /// Otherwise it'll return -1.
        /// </summary>
        /// <param name="path">The filepath to check.</param>
        /// <returns></returns>
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
        /// <summary>
        /// This will load and add a new texture into the program's memory.
        /// </summary>
        /// <param name="path">The path of the texture file to load...</param>
        /// <returns></returns>
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
        /// <summary>
        /// Loads the texture into the graphics memory...
        /// </summary>
        /// <param name="path">The path of the texture file to load...</param>
        /// <param name="id">The target texture id...</param>
        /// <param name="Unit">The target texture unit...</param>
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

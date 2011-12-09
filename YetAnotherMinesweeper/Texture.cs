/*Copyright (c) 2011 Robert Rouhani

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

   1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.

   2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.

   3. This notice may not be removed or altered from any source
   distribution.
 
Robert Rouhani <robert.rouhani@gmail.com>*/

using System;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace YetAnotherMinesweeper
{
	public class Texture
	{
		private TextureMinFilter minFilter;
		private TextureMagFilter magFilter;
		private TextureWrapMode wrapS;
		private TextureWrapMode wrapT;


		public int ID { get; private set; }

		public TextureMinFilter MinFilter 
		{
			get
			{
				return MinFilter;
			}
			set
			{
				minFilter = value;
				GL.BindTexture(TextureTarget.Texture2D, ID);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)minFilter);
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}
		public TextureMagFilter MagFilter 
		{
			get
			{
				return magFilter;
			}
			set
			{
				magFilter = value;
				GL.BindTexture(TextureTarget.Texture2D, ID);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)magFilter);
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}
		public TextureWrapMode WrapS
		{
			get
			{
				return wrapS;
			}
			set
			{
				wrapS = value;
				GL.BindTexture(TextureTarget.Texture2D, ID);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)wrapS);
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}
		public TextureWrapMode WrapT
		{
			get
			{
				return wrapT;
			}
			set
			{
				wrapT = value;
				GL.BindTexture(TextureTarget.Texture2D, ID);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)wrapT);
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}

		public Texture(Bitmap bmp, TextureMinFilter minFilter, TextureMagFilter magFilter, TextureWrapMode wrapS, TextureWrapMode wrapT)
		{
			ID = GL.GenTexture();

			//set texture parameters
			MinFilter = minFilter;
			MagFilter = magFilter;
			WrapS = wrapS;
			WrapT = wrapT;

			GL.BindTexture(TextureTarget.Texture2D, ID);

			//upload texture to GPU
			BitmapData data = bmp.LockBits(new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);

			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		public Texture(Bitmap bmp)
			: this(bmp, TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Repeat, TextureWrapMode.Repeat)
		{
		}

		public Texture(string path, TextureMinFilter minFilter, TextureMagFilter magFilter, TextureWrapMode wrapS, TextureWrapMode wrapT)
			: this(new Bitmap(path), minFilter, magFilter, wrapS, wrapT)
		{
		}

		public Texture(string path)
			: this(path, TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Repeat, TextureWrapMode.Repeat)
		{
		}

		public void Unload()
		{
			GL.DeleteTexture(ID);
		}
	}
}

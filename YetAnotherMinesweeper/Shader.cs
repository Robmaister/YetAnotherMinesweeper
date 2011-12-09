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
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace YetAnotherMinesweeper
{
	public class Shader
	{
		public int ID { get; private set; }
		public ShaderType Type { get; private set; }

		public Shader(string path, ShaderType type)
		{
			StreamReader reader = new StreamReader(path);
			string shaderSource = reader.ReadToEnd();

			ID = GL.CreateShader(type);
			GL.ShaderSource(ID, shaderSource);

			GL.CompileShader(ID);

			int status;
			GL.GetShader(ID, ShaderParameter.CompileStatus, out status);

			//throw an exception if there's an error compiling the shader
			if (status == 0)
			{
				string message = "Error compiling shader \"" + path + "\"\n";
				message += GL.GetShaderInfoLog(ID);
				Unload();
				throw new ArgumentException(message);
			}
		}

		public void Unload()
		{
			GL.DeleteShader(ID);
			ID = 0;
		}
	}
}

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
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace YetAnotherMinesweeper
{
	public class Program
	{
		public int ID { get; private set; }
		public Dictionary<string, int> Uniforms { get; private set; }

		public Program(params Shader[] shaders)
		{
			ID = GL.CreateProgram();
			Uniforms = new Dictionary<string, int>();

			foreach (Shader s in shaders)
			{
				GL.AttachShader(ID, s.ID);
			}

			GL.BindAttribLocation(ID, 0, "in_vertex");
			GL.BindAttribLocation(ID, 1, "in_texCoord");

			GL.LinkProgram(ID);

			int status;
			GL.GetProgram(ID, ProgramParameter.LinkStatus, out status);

			if (status == 0)
			{
				string message = "Error linking program: \n";
				message += GL.GetProgramInfoLog(ID);
				Unload();
				throw new ArgumentException(message);
			}

			//gets all the uniforms and stores them in a dictionary for easier lookup.
			//stolen from the boilerplate code for another project of mine.
			int uniformCount;
			GL.GetProgram(ID, ProgramParameter.ActiveUniforms, out uniformCount);
			for (int i = 0; i < uniformCount; i++)
			{
				int size;
				ActiveUniformType type;
				string name = GL.GetActiveUniform(ID, i, out size, out type);
				int location = GL.GetUniformLocation(ID, name);
				Uniforms.Add(name, location);
			}
		}

		public void Unload()
		{
			GL.DeleteProgram(ID);
			ID = 0;
		}
	}
}

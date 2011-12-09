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

namespace YetAnotherMinesweeper
{
	/// <summary>
	/// A class that stores all the resources for the game statically. Prevents having to waste time passing around shaders, textures, etc.
	/// </summary>
	public static class Resources
	{
		public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
		public static Dictionary<string, Program> Programs = new Dictionary<string, Program>();
		public static Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();

		public static void UnloadAll()
		{
			foreach (KeyValuePair<string, Program> pair in Programs)
			{
				pair.Value.Unload();
			}

			foreach (KeyValuePair<string, Texture> pair in Textures)
			{
				pair.Value.Unload();
			}
		}
	}
}

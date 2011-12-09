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

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace YetAnotherMinesweeper
{
	public class BufferSet
	{
		private int vertex;
		private int texCoord;
		private int index;
		private int indicesLength;

		public int Vertex { get { return vertex; } }
		public int TexCoord { get { return texCoord; } }
		public int Index { get { return index; } }

		public void SetVertices(float[] vertices)
		{
			if (vertex == 0)
				GL.GenBuffers(1, out vertex);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertex);
			GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void SetTexCoords(float[] texCoords)
		{
			if (texCoord == 0)
				GL.GenBuffers(1, out texCoord);

			GL.BindBuffer(BufferTarget.ArrayBuffer, texCoord);
			GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(texCoords.Length * sizeof(float)), texCoords, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void SetIndices(byte[] indices)
		{
			if (index == 0)
				GL.GenBuffers(1, out index);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, index);
			GL.BufferData<byte>(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(byte)), indices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			indicesLength = indices.Length;
		}

		public void DrawAll()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertex);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, texCoord);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, index);
			GL.DrawElements(BeginMode.Triangles, indicesLength, DrawElementsType.UnsignedByte, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		public void Unload()
		{
			int[] buffers = new int[] { vertex, texCoord, index };
			GL.DeleteBuffers(3, buffers);

			vertex = 0;
			texCoord = 0;
			index = 0;
		}
	}
}

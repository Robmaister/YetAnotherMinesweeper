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

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace YetAnotherMinesweeper
{
	public enum TileType { EMPTY, NUMBER, MINE }

	public class Tile
	{
		private int number;
		private Texture tex, maskedTex;
		private BufferSet set;
		private bool initalized;
		private bool flagged;
		private bool emptiesChecked;

		public TileType Type { get; private set; }
		public bool Visible { get; set; }
		public Vector2 Position { get; set; }
		public bool Initialized { get { return initalized; } }
		public bool Flagged { get { return flagged; } }
		public bool EmptiesChecked { get { return emptiesChecked; } set { emptiesChecked = value; } }

		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				if (number > 0 && number <= 8)
				{
					number = value;
				}
			}
		}

		public Tile()
		{
		}

		public void Initialize(TileType type, Vector2 position, BufferSet set)
		{
			//allow only one initalization.
			if (!initalized)
			{
				Position = position;
				this.set = set;

				if (type == TileType.MINE)
				{
					Type = type;
					tex = Resources.Textures["mine"];
					//Visible = true;
				}

				else
				{
					Type = TileType.EMPTY;
					tex = Resources.Textures["emptyTile"];
				}

				maskedTex = Resources.Textures["blankTile"];
				initalized = true;
			}
			else
				throw new InvalidOperationException("Can't initalize a tile twice!");
		}

		public void GenerateNumbers(Point location, Tile[][] tileset)
		{
			if (this != tileset[location.Y][location.X])
				throw new ArgumentException("Location does not represent the current tile.");

			else if (Type != TileType.MINE)
			{
				int mines = 0;
				for (int i = location.Y - 1; i <= location.Y + 1; i++)
				{
					for (int j = location.X - 1; j <= location.X + 1; j++)
					{
						if (i >= 0 && i < tileset.Length && j >= 0 && j < tileset[0].Length && tileset[i][j].Type == TileType.MINE)
							mines++;
					}
				}

				if (mines > 0)
					this.Type = TileType.NUMBER;

				switch (mines)
				{
					case 1:
						this.tex = Resources.Textures["num1"];
						break;
					case 2:
						this.tex = Resources.Textures["num2"];
						break;
					case 3:
						this.tex = Resources.Textures["num3"];
						break;
					case 4:
						this.tex = Resources.Textures["num4"];
						break;
					case 5:
						this.tex = Resources.Textures["num5"];
						break;
					case 6:
						this.tex = Resources.Textures["num6"];
						break;
					case 7:
						this.tex = Resources.Textures["num7"];
						break;
					case 8:
						this.tex = Resources.Textures["num8"];
						break;
				}
			}
		}

		public void Hover()
		{
			maskedTex = Resources.Textures["tileHover"];
		}

		public void Unhover()
		{
			maskedTex = Resources.Textures["blankTile"];
		}

		public void MouseDown()
		{
			maskedTex = Resources.Textures["tileMouseDown"];
		}

		public void MouseUp()
		{
			maskedTex = Resources.Textures["blankTile"];

			if (!flagged)
			{
				Visible = true;
			}
		}

		public void ToggleFlag()
		{
			if (!Visible)
				flagged = !flagged;
		}

		public void Update()
		{
			if (!initalized)
				throw new InvalidOperationException("Can't update a tile if it's not initalized!");
		}

		public void Draw()
		{
			if (!initalized)
				throw new InvalidOperationException("Can't render a tile if it's not initalized");

			GL.Uniform4(Resources.Programs["baseShader"].Uniforms["position"], new Vector4(Position.X, Position.Y, 0, 1));

			if (flagged)
				GL.BindTexture(TextureTarget.Texture2D, Resources.Textures["flag"].ID);
			else if (!Visible)
				GL.BindTexture(TextureTarget.Texture2D, maskedTex.ID);
			else
				GL.BindTexture(TextureTarget.Texture2D, tex.ID);

			set.DrawAll();

			GL.BindTexture(TextureTarget.Texture2D, 0);
		}
	}
}

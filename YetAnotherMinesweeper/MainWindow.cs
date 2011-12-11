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
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace YetAnotherMinesweeper
{
	public class MainWindow : GameWindow
	{
		#region ===Main===

		public static void Main(string[] args)
		{
			int numMines = 10, tilesetX = 10, tilesetY = 10;
			foreach (string arg in args)
			{
				if (!string.IsNullOrEmpty(arg))
				{
					if (arg[0] == '-')
					{
						switch (arg.Substring(1, 1))
						{
							case "m":
								if (!int.TryParse(arg.Substring(2), out numMines))
									numMines = 10;
								break;
							case "t":
								int xIndex = arg.IndexOf('x');
								if (arg.IndexOf('x') != -1)
								{
									if (!int.TryParse(arg.Substring(2, xIndex - 2), out tilesetX))
										tilesetX = 10;
									if (!int.TryParse(arg.Substring(2, xIndex - 2), out tilesetY))
										tilesetY = 10;
								}
								break;
							case "h":
								if (arg.Substring(1) == "help")
								{
									Console.WriteLine("Proper usage for non-default mines/tileset size:");
									Console.WriteLine("\tYetAnotherMinesweeper.exe -t10x10 -m20\n");
									Console.WriteLine("Where -t__x__ is the size of the tileset and -m__ is the number of mines");
								}
								break;
						}
					}
				}
			}

			Tile[][] tileset = new Tile[tilesetY][];
			for (int i = 0; i < tileset.Length; i++)
			{
				tileset[i] = new Tile[tilesetX];

				for (int j = 0; j < tileset[0].Length; j++)
				{
					tileset[i][j] = new Tile();
				}
			}

			MainWindow window = new MainWindow(tileset, numMines);
			window.Run();
		}

		#endregion

		private bool gameOver, gameWon, gameCleaned;
		private int mines;
		private Matrix4 proj;
		private Tile[][] tileset;
		private Tile hoveringTile;
		private BufferSet squareSet, bannerSet;

		public MainWindow(Tile[][] tileset, int numMines)
			: base()
		{
			this.Title = "Yet Another Minesweeper Clone";
			this.ClientSize = new System.Drawing.Size(512, 512);
			mines = numMines;

			this.tileset = tileset;

			hoveringTile = new Tile();

			Mouse.ButtonDown += Mouse_ButtonDown;
			Mouse.ButtonUp += Mouse_ButtonUp;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			//load all shaders/programs
			Resources.Shaders.Add("baseShaderVert", new Shader(@"Shaders/baseShader.vert", ShaderType.VertexShader));
			Resources.Shaders.Add("baseShaderFrag", new Shader(@"Shaders/baseShader.frag", ShaderType.FragmentShader));
			Resources.Programs.Add("baseShader", new Program(Resources.Shaders["baseShaderVert"], Resources.Shaders["baseShaderFrag"]));

			//load all textures
			Resources.Textures.Add("mine", new Texture(@"Textures/mine.png"));
			Resources.Textures.Add("blankTile", new Texture(@"Textures/blankTile.png"));
			Resources.Textures.Add("tileHover", new Texture(@"Textures/tileHover.png"));
			Resources.Textures.Add("tileMouseDown", new Texture(@"Textures/tileMouseDown.png"));
			Resources.Textures.Add("emptyTile", new Texture(@"Textures/emptyTile.png"));
			Resources.Textures.Add("youlost", new Texture(@"Textures/youlost.png"));
			Resources.Textures.Add("youwin", new Texture(@"Textures/youwin.png"));
			Resources.Textures.Add("flag", new Texture(@"Textures/flag.png"));
			Resources.Textures.Add("num1", new Texture(@"Textures/num1.png"));
			Resources.Textures.Add("num2", new Texture(@"Textures/num2.png"));
			Resources.Textures.Add("num3", new Texture(@"Textures/num3.png"));
			Resources.Textures.Add("num4", new Texture(@"Textures/num4.png"));
			Resources.Textures.Add("num5", new Texture(@"Textures/num5.png"));
			Resources.Textures.Add("num6", new Texture(@"Textures/num6.png"));
			Resources.Textures.Add("num7", new Texture(@"Textures/num7.png"));
			Resources.Textures.Add("num8", new Texture(@"Textures/num8.png"));

			//generate a single set of VBOs for a standard-sized square
			squareSet = new BufferSet();
			bannerSet = new BufferSet();

			float[] vertices = new float[]
			{
				-.5f, .5f, //top left
				-.5f, -.5f, //bottom left
				.5f, -.5f, //bottom right
				.5f, .5f // top right
			};

			float[] texCoords = new float[]
			{
				0, 0, //top left
				0, 1, //bottom left
				1, 1, //bottom right
				1, 0 //top right
			};

			//triangles
			byte[] indices = new byte[]
			{
				0, 1, 2,
				0, 2, 3
			};

			squareSet.SetVertices(vertices);
			squareSet.SetTexCoords(texCoords);
			squareSet.SetIndices(indices);

			Random r = new Random();

			//choose a random point on the tileset that's not already chosen.
			for (int i = 0; i < mines; i++)
			{
				Point p = new Point(0, 0);
				do
				{
					p = new Point(r.Next(tileset[0].Length), r.Next(tileset.Length));
				}
				while (tileset[p.Y][p.X].Type == TileType.MINE);

				tileset[p.Y][p.X].Initialize(TileType.MINE, new Vector2(p.X, p.Y), squareSet);
			}

			for (int i = 0; i < tileset.Length; i++)
			{
				for (int j = 0; j < tileset[0].Length; j++)
				{
					Tile t = tileset[i][j];
					if (!t.Initialized)
						t.Initialize(TileType.EMPTY, new Vector2(j, i), squareSet);
				}
			}

			for (int i = 0; i < tileset.Length; i++)
			{
				for (int j = 0; j < tileset[0].Length; j++)
				{
					tileset[i][j].GenerateNumbers(new Point(j, i), tileset);
				}
			}

			/*float left = -0.25f;
			float right = (tileset[0].Length / 2) - 0.25f;*/
			float left = -0.5f;
			float right = tileset[0].Length;
			float bottom = -(right - left) / 4;
			float top = (right - left) / 4;

			float[] bannerVertices = new float[]
			{
				left, top,
				left, bottom,
				right, bottom,
				right, top
			};

			bannerSet.SetVertices(bannerVertices);
			bannerSet.SetTexCoords(texCoords);
			bannerSet.SetIndices(indices);

			GL.ClearColor(new Color4(233, 255, 210, 255));
			GL.Enable(EnableCap.CullFace);
			GL.FrontFace(FrontFaceDirection.Ccw);
			GL.CullFace(CullFaceMode.Back);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);

			//projection is based off scale of tile size = 1, add a border of 1 tile size around the screen.
			//TODO make the projection matrix keep the tiles squares and add the extra size to the longer axis.
			proj = Matrix4.CreateOrthographicOffCenter(-0.25f, ((float)tileset[0].Length / 2) - 0.25f, -0.25f, ((float)tileset.Length / 2) - 0.25f, 0, 1);

			foreach (KeyValuePair<string, Program> pair in Resources.Programs)
			{
				GL.UseProgram(pair.Value.ID);
				GL.UniformMatrix4(pair.Value.Uniforms["projection"], false, ref proj);
			}

			GL.UseProgram(0);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			//TODO put up win/lose banners
			if (gameWon && !gameCleaned)
			{
				Mouse.ButtonUp -= Mouse_ButtonUp;
				Mouse.ButtonDown -= Mouse_ButtonDown;

				gameCleaned = true;
			}

			else if (gameOver && !gameCleaned)
			{
				foreach (Tile[] ta in tileset)
				{
					foreach (Tile t in ta)
					{
						t.MouseUp();
					}
				}

				Mouse.ButtonUp -= Mouse_ButtonUp;
				Mouse.ButtonDown -= Mouse_ButtonDown;

				gameCleaned = true;
			}

			else if (!gameOver)
			{
				//mouse tracking code
				Point tileCoords = new Point((int)Math.Floor((float)Mouse.X * (float)tileset[0].Length / (float)ClientSize.Width), (int)Math.Floor((float)(ClientSize.Height - Mouse.Y) * (float)tileset.Length / (float)ClientSize.Height));
				if (tileCoords.X < tileset[0].Length && tileCoords.Y < tileset.Length)
				{
					if (hoveringTile != tileset[tileCoords.Y][tileCoords.X])
					{
						hoveringTile.Unhover();
						hoveringTile = tileset[tileCoords.Y][tileCoords.X];
						hoveringTile.Hover();
					}
				}
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			GL.UseProgram(Resources.Programs["baseShader"].ID);

			foreach (Tile[] ta in tileset)
			{
				foreach (Tile t in ta)
				{
					t.Draw();
				}
			}

			if (gameWon)
			{
				GL.BindTexture(TextureTarget.Texture2D, Resources.Textures["youwin"].ID);
				GL.Uniform4(Resources.Programs["baseShader"].Uniforms["position"], new Vector4(0, tileset.Length / 2, 0, 1));
				bannerSet.DrawAll();
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}

			else if (gameOver)
			{
				GL.BindTexture(TextureTarget.Texture2D, Resources.Textures["youlost"].ID);
				GL.Uniform4(Resources.Programs["baseShader"].Uniforms["position"], new Vector4(0, tileset.Length / 2, 0, 1));
				bannerSet.DrawAll();
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}

			GL.UseProgram(0);

			GL.DisableVertexAttribArray(0);
			GL.DisableVertexAttribArray(1);

			SwapBuffers();
		}

		void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (hoveringTile != null)
			{
				if (e.Button == MouseButton.Left)
				{
					hoveringTile.MouseUp();
					hoveringTile.Hover();

					if (hoveringTile.Type == TileType.EMPTY)
					{
						ShowAllBorderingEmpties(hoveringTile);
					}

					if (hoveringTile.Type == TileType.MINE && hoveringTile.Visible)
						gameOver = true;
					else
					{
						gameWon = true;

						//check for game won
						foreach (Tile[] ta in tileset)
						{
							foreach (Tile t in ta)
							{
								if (t.Type != TileType.MINE && !t.Visible)
									gameWon = false;
							}
						}
					}
				}
			}
		}

		void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (hoveringTile != null)
			{
				if (e.Button == MouseButton.Left)
					hoveringTile.MouseDown();
				else if (e.Button == MouseButton.Right)
					hoveringTile.ToggleFlag();
			}
		}

		private void ShowAllBorderingEmpties(Tile startingTile)
		{
			Point pt = new Point(0, 0);
			bool found = false;
			//search for the starting tile
			for (int i = 0; i < tileset.Length && !found; i++)
			{
				for (int j = 0; j < tileset[0].Length && !found; j++)
				{
					if (tileset[i][j] == startingTile)
					{
						found = true;
						pt = new Point(j, i);
						break;
					}
				}
			}

			ShowAllBorderingEmpties(pt);
		}

		private void ShowAllBorderingEmpties(Point pt)
		{
			tileset[pt.Y][pt.X].EmptiesChecked = true;
			for (int i = pt.Y - 1; i <= pt.Y + 1; i++)
			{
				for (int j = pt.X - 1; j <= pt.X + 1; j++)
				{
					if (i >= 0 && i < tileset.Length && j >= 0 && j < tileset[0].Length)
					{
						if (tileset[i][j].Type == TileType.EMPTY && !tileset[i][j].EmptiesChecked)
							ShowAllBorderingEmpties(new Point(j, i));

						tileset[i][j].MouseUp();
					}
				}
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);

			Resources.UnloadAll();
			squareSet.Unload();
			bannerSet.Unload();
		}
	}
}

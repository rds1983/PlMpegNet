using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using PlMpegNet.Samples.VideoPlayer.UI;

namespace PlMpegNet.Samples.VideoPlayer
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private readonly string _path;
		private MainFrame _mainFrame;
		private Desktop _desktop;

		public Game1(string path)
		{
			_path = path;

			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1400,
				PreferredBackBufferHeight = 960
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
			_path = path;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			MyraEnvironment.Game = this;
			_desktop = new Desktop();
			_mainFrame = new MainFrame();
			_desktop.Root = _mainFrame;

			// TODO: use this.Content to load your game content here
			_mainFrame.LoadVideo(_path);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
			// _vorbis.Dispose();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			_mainFrame.UpdateTime(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// TODO: Add your drawing code here
			/*			var font = _fontSystem.GetFont(32);
						_spriteBatch.Begin();

						_spriteBatch.DrawString(font, $"Allocations: {StbVorbis.NativeAllocations}", Vector2.Zero, Color.White);

						_spriteBatch.End();*/

			_desktop.Render();

			base.Draw(gameTime);
		}
	}
}
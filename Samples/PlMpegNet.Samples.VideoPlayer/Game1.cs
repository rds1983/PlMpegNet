using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;
using static PlMpegSharp.PlMpeg;

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
		private DynamicSoundEffectInstance _effect;
		private bool _startedPlaying;
		private plm_t _plm;
		private Point _size;
		private byte[] _dataRgba;
		private Texture2D _texture;

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

		private void OnVideoCallback(plm_t plm, plm_frame_t arg1, object arg2)
		{
			unsafe
			{
				fixed (byte* ptr = _dataRgba)
				{
					plm_frame_to_rgba(arg1, ptr, _size.X * 4);
				}
			}

			_texture.SetData(_dataRgba);
		}

		private void OnAudioCallback(plm_t plm, plm_samples_t samples, object arg)
		{
			if (samples.count == 0)
			{
				return;
			}

			var channels = 2;
			var samplesPerBuffer = samples.count;

			var size = samples.count * channels;
			byte[] audioData = new byte[size * 2];
			for (var i = 0; i < size; ++i)
			{
				var floatSample = samples.interleaved[i];
				var n = (int)(floatSample * 32768.0f);
				if (n < short.MinValue)
				{
					n = short.MinValue;
				}

				if (n > short.MaxValue)
				{
					n = short.MaxValue;
				}

				var shortSample = (short)n;

				var b1 = (byte)(shortSample >> 8);
				var b2 = (byte)(shortSample);
				if (!BitConverter.IsLittleEndian)
				{
					audioData[i * 2 + 0] = b1;
					audioData[i * 2 + 1] = b2;
				}
				else
				{
					audioData[i * 2 + 0] = b2;
					audioData[i * 2 + 1] = b1;
				}
			}

			_effect.SubmitBuffer(audioData);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			var data = File.ReadAllBytes(_path);

			unsafe
			{
				fixed (byte* ptr = data)
				{
					_plm = plm_create_with_memory(ptr, (ulong)data.Length, 0);
				}
			}


			if (plm_probe(_plm, 5000 * 1024) == 0)
			{
				throw new Exception("No MPEG video or audio streams found.");
			}

			plm_set_video_decode_callback(_plm, OnVideoCallback, null);
			plm_set_audio_decode_callback(_plm, OnAudioCallback, null);
			plm_set_loop(_plm, 1);

			plm_set_audio_enabled(_plm, 1);
			plm_set_audio_stream(_plm, 0);

			_size.X = plm_get_width(_plm);
			_size.Y = plm_get_height(_plm);

			_texture = new Texture2D(GraphicsDevice, _size.X, _size.Y);
			_dataRgba = new byte[_size.X * _size.Y * 4];

			var sampleRate = plm_get_samplerate(_plm);
			Debug.WriteLine($"Sample rate: {sampleRate}");

			_effect = new DynamicSoundEffectInstance(sampleRate, (AudioChannels)2)
			{
				Volume = 0.5f
			};

			GC.Collect();
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
			if (!_startedPlaying)
			{
				_effect.Play();
				_startedPlaying = true;
			}

			plm_decode(_plm, gameTime.ElapsedGameTime.TotalSeconds);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			/*			var font = _fontSystem.GetFont(32);
						_spriteBatch.Begin();

						_spriteBatch.DrawString(font, $"Allocations: {StbVorbis.NativeAllocations}", Vector2.Zero, Color.White);

						_spriteBatch.End();*/

			_spriteBatch.Begin(blendState: BlendState.Opaque);

			var x = (GraphicsDevice.Viewport.Width - _size.X) / 2;
			var y = (GraphicsDevice.Viewport.Height - _size.Y) / 2;

			_spriteBatch.Draw(_texture, new Vector2(x, y), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
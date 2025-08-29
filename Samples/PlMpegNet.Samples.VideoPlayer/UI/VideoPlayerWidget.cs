using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using PlMpegNet.Samples.VideoPlayer.MonoGame.Effects;
using System;
using System.Diagnostics;
using System.IO;
using static PlMpegSharp.PlMpeg;

namespace PlMpegNet.Samples.VideoPlayer.MonoGame.UI
{
	public class VideoPlayerWidget : Widget
	{
		private DynamicSoundEffectInstance _effect;
		private plm_t _plm;
		private Point _size;
		private byte[] _audioBuffer;
		private Texture2D _yTexture, _crTexture, _cbTexture;
		private float _positionInSeconds;
		private readonly VideoEffect _videoEffect = new VideoEffect();
		private readonly SpriteBatch _spriteBatch = new SpriteBatch(MyraEnvironment.GraphicsDevice);


		public float Volume
		{
			get => _effect.Volume;

			set => _effect.Volume = value;
		}

		public float PositionInSeconds
		{
			get => _positionInSeconds;

			set
			{
				if (value.EpsilonEquals(_positionInSeconds))
				{
					return;
				}

				var oldValue = _positionInSeconds;
				_positionInSeconds = value;
				plm_decode(_plm, value - oldValue);
			}
		}

		public float DurationInSeconds { get; private set; }

		private void UpdateTexture(ref Texture2D texture, plm_plane_t plane)
		{
			if (texture == null || texture.Width != plane.width || texture.Height != plane.height)
			{
				texture = new Texture2D(MyraEnvironment.GraphicsDevice, (int)plane.width, (int)plane.height, false, SurfaceFormat.Alpha8);
			}

			texture.SetData(plane.data);
		}

		private void OnVideoCallback(plm_t plm, plm_frame_t plane, object user)
		{
			UpdateTexture(ref _yTexture, plane.y);
			UpdateTexture(ref _crTexture, plane.cr);
			UpdateTexture(ref _cbTexture, plane.cb);
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

			if (_audioBuffer == null || _audioBuffer.Length != size * 2)
			{
				_audioBuffer = new byte[size * 2];
			}

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
					_audioBuffer[i * 2 + 0] = b1;
					_audioBuffer[i * 2 + 1] = b2;
				}
				else
				{
					_audioBuffer[i * 2 + 0] = b2;
					_audioBuffer[i * 2 + 1] = b1;
				}
			}

			_effect.SubmitBuffer(_audioBuffer);
		}

		public void Load(string path)
		{
			var stream = File.OpenRead(path);
			_plm = plm_create_with_stream(stream);

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

			DurationInSeconds = (float)plm_get_duration(_plm);
			_positionInSeconds = 0;

			var sampleRate = plm_get_samplerate(_plm);
			Debug.WriteLine($"Sample rate: {sampleRate}");

			_effect = new DynamicSoundEffectInstance(sampleRate, (AudioChannels)2)
			{
				Volume = 1f
			};

			_effect.Play();

			GC.Collect();
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (_yTexture == null)
			{
				return;
			}

			_videoEffect.YTexture = _yTexture;
			_videoEffect.CbTexture = _cbTexture;
			_videoEffect.CrTexture = _crTexture;

			var bounds = ActualBounds;
			var p = ToGlobal(bounds.Location);
			bounds.X = p.X;
			bounds.Y = p.Y;

			var device = MyraEnvironment.GraphicsDevice;
			var oldViewport = device.Viewport;
			try
			{
				device.Viewport = new Viewport(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				_spriteBatch.Begin(effect: _videoEffect);
				_spriteBatch.Draw(_yTexture, Vector2.Zero, Color.White);
				_spriteBatch.End();
			}
			finally
			{
				device.Viewport = oldViewport;
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			return _size;
		}
	}
}

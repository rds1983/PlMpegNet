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
		private Video _video;
		private byte[] _audioBuffer;
		private Texture2D _yTexture, _crTexture, _cbTexture;
		private readonly VideoEffect _videoEffect = new VideoEffect();
		private readonly SpriteBatch _spriteBatch = new SpriteBatch(MyraEnvironment.GraphicsDevice);
		private readonly Stopwatch _stopWatch = new Stopwatch();
		private float _positionInSeconds = 0;
		private Point _size;

		public Video Video => _video;

		public float Volume
		{
			get => _effect.Volume;

			set => _effect.Volume = value;
		}

		public float PositionInSeconds
		{
			get => _positionInSeconds;

			set => SetPositionInSeconds(value, true);
		}

		public bool IsPlaying
		{
			get => _stopWatch.IsRunning;

			set
			{
				if (value == IsPlaying)
				{
					return;
				}

				if (value)
				{
					_stopWatch.Start();
				}
				else
				{
					_stopWatch.Stop();
				}

				var ev = IsPlayingChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public float DurationInSeconds => _video.DurationInSeconds;

		public event EventHandler PositionInSecondsChanged;
		public event EventHandler IsPlayingChanged;

		private void SetPositionInSeconds(float value, bool doSeek)
		{
			if (value.EpsilonEquals(_positionInSeconds))
			{
				return;
			}

			_positionInSeconds = value;

			var ev = PositionInSecondsChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}

			if (doSeek)
			{
				_video.Seek(value, false);
			}
		}
		private void UpdateTexture(ref Texture2D texture, plm_plane_t plane)
		{
			if (texture == null || texture.Width != plane.width || texture.Height != plane.height)
			{
				texture = new Texture2D(MyraEnvironment.GraphicsDevice, (int)plane.width, (int)plane.height, false, SurfaceFormat.Alpha8);
			}

			texture.SetData(plane.data);
		}

		private void OnVideoCallback(plm_frame_t frame)
		{
			UpdateTexture(ref _yTexture, frame.y);
			UpdateTexture(ref _crTexture, frame.cr);
			UpdateTexture(ref _cbTexture, frame.cb);
		}

		private void OnAudioCallback(plm_samples_t samples)
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
			_video = Video.FromStream(stream);

			if (!_video.Probe(5000 * 1024))
			{
				throw new Exception("No MPEG video or audio streams found.");
			}

			_video.VideoCallback = OnVideoCallback;
			_video.AudioCallback = OnAudioCallback;
			_video.AudioEnabled = true;
			_video.AudioStreamNumber = 0;

			_size.X = _video.Width;
			_size.Y = _video.Height;

			SetPositionInSeconds(0, false);
			IsPlaying = true;

			var sampleRate = _video.SampleRate;
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

			if (_stopWatch.IsRunning)
			{
				var passed = (float)_stopWatch.Elapsed.TotalSeconds;
				_stopWatch.Restart();

				_video.Decode(passed);
				SetPositionInSeconds(_video.PositionInSeconds, false);
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
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
		private byte[] _dataRgba, _audioBuffer;
		private Texture2D _texture;
		private float _positionInSeconds;

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

		private void OnVideoCallback(plm_t plm, plm_frame_t arg1, object arg2)
		{
			unsafe
			{
				fixed (byte* ptr = _dataRgba)
				{
					plm_frame_to_rgba(arg1, ptr, _size.X * 4);
				}
			}

			if (_texture == null || _texture.Width != _size.X || _texture.Height != _size.Y)
			{
				_texture = new Texture2D(MyraEnvironment.GraphicsDevice, _size.X, _size.Y);
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
			var data = File.ReadAllBytes(path);

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

			_dataRgba = new byte[_size.X * _size.Y * 4];

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

			if (_texture == null)
			{
				return;
			}

			context.Draw(_texture, Vector2.Zero, Color.White);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			return _size;
		}
	}
}

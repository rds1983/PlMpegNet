using System;
using System.IO;
using static PlMpegSharp.PlMpeg;

namespace PlMpegNet
{
	public class Video : IDisposable
	{
		public plm_t Plm { get; private set; }

		public Action<plm_frame_t> VideoCallback { get; set; }
		public Action<plm_samples_t> AudioCallback { get; set; }

		public bool Loop
		{
			get => plm_get_loop(Plm).ToBool();

			set => plm_set_loop(Plm, value.ToInt());
		}

		public bool AudioEnabled
		{
			get => plm_get_audio_enabled(Plm).ToBool();

			set => plm_set_audio_enabled(Plm, value.ToInt());
		}

		public int AudioStreamNumber
		{
			set => plm_set_audio_stream(Plm, value);
		}

		public int AudioStreamCount => plm_get_num_audio_streams(Plm);

		public int Width => plm_get_width(Plm);
		public int Height => plm_get_height(Plm);

		public float PositionInSeconds => (float)plm_get_time(Plm);

		public float DurationInSeconds => (float)plm_get_duration(Plm);

		public int SampleRate => plm_get_samplerate(Plm);

		private Video(plm_t plm)
		{
			Plm = plm ?? throw new ArgumentNullException(nameof(plm));
			plm_set_video_decode_callback(Plm, OnVideoCallback, null);
			plm_set_audio_decode_callback(Plm, OnAudioCallback, null);
		}

		~Video()
		{
			Dispose();
		}

		private void OnVideoCallback(plm_t plm, plm_frame_t frame, object user)
		{
			if (VideoCallback != null)
			{
				VideoCallback(frame);
			}
		}

		private void OnAudioCallback(plm_t plm, plm_samples_t samples, object user)
		{
			if (AudioCallback != null)
			{
				AudioCallback(samples);
			}
		}


		public void Dispose()
		{
			if (Plm != null)
			{
				plm_destroy(Plm);
				Plm = null;
			}
		}

		public bool Seek(float positionInSeconds, bool seekExact) =>
			plm_seek(Plm, (double)positionInSeconds, seekExact.ToInt()).ToBool();

		public bool Probe(ulong size) => plm_probe(Plm, size).ToBool();

		public void Decode(float passedSeconds) => plm_decode(Plm, passedSeconds);

		public static Video FromStream(Stream stream)
		{
			var plm = plm_create_with_stream(stream);

			return new Video(plm);
		}

		public static Video FromBytes(byte[] bytes)
		{
			var plm = plm_create_with_bytes(bytes);

			return new Video(plm);
		}
	}
}

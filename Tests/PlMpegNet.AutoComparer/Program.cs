using PlMpegNativeWrapper;
using PlMpegNet.AutoComparer;
using System;
using System.IO;
using static PlMpegSharp.PlMpeg;

namespace PlMpegSharp.AutoComparer
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: AutoComparer <file.mpg>");
				return;
			}

			var file = Path.GetFileNameWithoutExtension(Path.GetFileName(args[0]));

			var inputStream = File.OpenRead(args[0]);
			var plm = plm_create_with_stream(inputStream);

			Native.create_with_file(args[0]);

			plm_set_audio_enabled(plm, 1);
			Native.set_audio_enabled(true);

			var width = plm_get_width(plm);
			var height = plm_get_height(plm);
			var pixels = new byte[width * height * 3];
			var pixels2 = new byte[width * height * 3];

			var audio2 = new float[10000];

			plm_frame_t frame;

			var i = 1;
			while ((frame = plm_decode_video(plm)) != null)
			{
				Console.WriteLine($"Frame #{i}");

				plm_frame_to_rgb(frame, pixels, width * 3);
				Native.get_next_frame_rgb(pixels2);

				// Compare pixels
				for (var j = 0; j < pixels.Length; j++)
				{
					if (pixels[j] != pixels2[j])
					{
						Console.WriteLine($"Different colors: #{i} frame, pos {j}");
					}
				}

				// Compare audio
				var samples = plm_decode_audio(plm);
				Native.get_next_frame_audio(audio2);
				if (samples != null)
				{
					for (var j = 0; j < samples.count * 2; ++j)
					{
						if (!samples.interleaved[i].EpsilonEquals(audio2[i]))
						{
							Console.WriteLine($"Different samples: #{i} frame, pos {j}");
						}
					}
				}

				++i;
			}
		}
	}
}

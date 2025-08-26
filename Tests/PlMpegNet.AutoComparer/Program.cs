using PlMpegNativeWrapper;
using System;
using System.IO;
using static PlMpegSharp.PlMpeg;

namespace PlMpegSharp.AutoComparer
{
	internal unsafe class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: AutoComparer <file.mpg>");
				return;
			}

			var file = Path.GetFileNameWithoutExtension(Path.GetFileName(args[0]));
			var data = File.ReadAllBytes(args[0]);

			plm_t plm;
			fixed (byte* ptr = data)
			{
				plm = plm_create_with_memory(ptr, (ulong)data.Length, 0);
			}

			Native.create_with_memory(data);

			plm_set_audio_enabled(plm, 0);
			Native.set_audio_enabled(false);

			var width = plm_get_width(plm);
			var height = plm_get_height(plm);
			var pixels = new byte[width * height * 3];
			var pixels2 = new byte[width * height * 3];

			plm_frame_t frame;

			var i = 1;
			while ((frame = plm_decode_video(plm)) != null)
			{
				Console.WriteLine($"Frame #{i}");

				fixed (byte* ptr = pixels)
				{
					plm_frame_to_rgb(frame, ptr, width * 3);
				}

				Native.get_next_frame_rgb(pixels2);

				for (var j = 0; j < pixels.Length; j++)
				{
					if (pixels[j] != pixels2[j])
					{
						Console.WriteLine($"Different colors: #{i} frame, pos {j}");
					}
				}

				++i;
			}
		}
	}
}

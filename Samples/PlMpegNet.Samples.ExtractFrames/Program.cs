using StbImageWriteSharp;
using System;
using System.IO;
using static PlMpegSharp.PlMpeg;

namespace PlMpegSharp.Samples.ExtractFrames
{
	internal unsafe class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: ExtractFrames <file.mpg>");
				return;
			}

			if (!Directory.Exists("output"))
			{
				Directory.CreateDirectory("output");
			}

			var file = Path.GetFileNameWithoutExtension(Path.GetFileName(args[0]));
			var inputStream = File.OpenRead(args[0]);
			var plm = plm_create_with_stream(inputStream);

			plm_set_audio_enabled(plm, 0);

			var width = plm_get_width(plm);
			var height = plm_get_height(plm);
			var pixels = new byte[width * height * 3];

			plm_frame_t frame;

			var writer = new ImageWriter();
			var i = 1;
			while ((frame = plm_decode_video(plm)) != null)
			{
				fixed (byte* ptr = pixels)
				{
					plm_frame_to_rgb(frame, ptr, width * 3);
				}

				var path = $"output/{file}_{i}.png";
				using (Stream stream = File.OpenWrite(path))
				{
					writer.WritePng(pixels, width, height, ColorComponents.RedGreenBlue, stream);
				}

				Console.WriteLine($"Wrote {path}");

				++i;
			}
		}
	}
}

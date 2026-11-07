using System;
using System.IO;
#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

namespace PlMpegNet
{
	public static unsafe partial class PlMpeg
	{
		private class ReadContext
		{
			public Stream _inputStream;
#if NETSTANDARD2_0
			private byte[] _readBuffer;
#endif

			public ReadContext(Stream stream)
			{
				_inputStream = stream ?? throw new NullReferenceException(nameof(stream));
			}

			public void LoadCallback(plm_buffer_t buffer)
			{
				if (buffer.discard_read_bytes == 1)
				{
					plm_buffer_discard_read_bytes(buffer);
				}

				var bytesAvailable = (int)(buffer.capacity - buffer.length);
				var bytesRead = Read(buffer.bytes + buffer.length, bytesAvailable);
				if (bytesRead > 0)
				{
					buffer.length += (ulong)bytesRead;
				}
				else
				{
					buffer.has_ended = 1;
				}
			}

			private int Read(byte* output, int count)
			{
#if NETSTANDARD2_0
				// Stream.Read(Span<byte>) is not available, so read into a managed buffer and copy it over
				if (_readBuffer == null || _readBuffer.Length < count)
				{
					_readBuffer = new byte[count];
				}

				var bytesRead = _inputStream.Read(_readBuffer, 0, count);
				if (bytesRead > 0)
				{
					Marshal.Copy(_readBuffer, 0, (IntPtr)output, bytesRead);
				}

				return bytesRead;
#else
				return _inputStream.Read(new Span<byte>(output, count));
#endif
			}

			public void SeekCallback(plm_buffer_t buffer, ulong offset)
			{
				_inputStream.Seek((long)offset, SeekOrigin.Begin);
			}

			public ulong TellCallback(plm_buffer_t buffer)
			{
				return (ulong)_inputStream.Position;
			}
		}

		private static void LoadCallback(plm_buffer_t buffer, object user)
		{
			var context = (ReadContext)user;
			context.LoadCallback(buffer);
		}

		private static void SeekCallback(plm_buffer_t buffer, ulong offset, object user)
		{
			var context = (ReadContext)user;
			context.SeekCallback(buffer, offset);
		}

		private static ulong TellCallback(plm_buffer_t buffer, object user)
		{
			var context = (ReadContext)user;
			return context.TellCallback(buffer);
		}

		public static plm_t plm_create_with_stream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			var size = stream.Seek(0, SeekOrigin.End);
			var length = stream.Position;
			stream.Seek(0, SeekOrigin.Begin);

			var context = new ReadContext(stream);

			var buffer = plm_buffer_create_with_callbacks(LoadCallback, SeekCallback, TellCallback,
				(ulong)length, context);

			return plm_create_with_buffer(buffer, 1);
		}

		public static plm_t plm_create_with_bytes(byte[] data)
		{
			unsafe
			{
				fixed (byte* ptr = data)
				{
					return plm_create_with_memory(ptr, (ulong)data.Length, 0);
				}
			}
		}

		public static short plm_buffer_read_vlc(plm_buffer_t self, plm_vlc_t[] table)
		{
			fixed (plm_vlc_t* ptr = table)
			{
				return plm_buffer_read_vlc(self, ptr);
			}
		}

		public static ushort plm_buffer_read_vlc_uint(plm_buffer_t self, plm_vlc_uint_t[] table)
		{
			fixed (plm_vlc_uint_t* ptr = table)
			{
				return (ushort)plm_buffer_read_vlc(self, (plm_vlc_t*)ptr);
			}
		}

		public static ulong plm_buffer_tell(plm_buffer_t self)
		{
			if (self.tell_callback != null)
			{
				return self.tell_callback(self, self.load_callback_user_data) + (ulong)(self.bit_index >> 3) - (ulong)self.length;
			}

			return self.bit_index >> 3;
		}

		public static void plm_frame_to_rgb(plm_frame_t frame, byte[] dest, int stride)
		{
			fixed (byte* ptr = dest)
			{
				plm_frame_to_rgb(frame, ptr, stride);
			}
		}

		public static void plm_frame_to_bgr(plm_frame_t frame, byte[] dest, int stride)
		{
			fixed (byte* ptr = dest)
			{
				plm_frame_to_bgr(frame, ptr, stride);
			}
		}

		public static void plm_frame_to_rgba(plm_frame_t frame, byte[] dest, int stride)
		{
			fixed (byte* ptr = dest)
			{
				plm_frame_to_rgba(frame, ptr, stride);
			}
		}

		public static void plm_frame_to_bgra(plm_frame_t frame, byte[] dest, int stride)
		{
			fixed (byte* ptr = dest)
			{
				plm_frame_to_bgra(frame, ptr, stride);
			}
		}

		public static void plm_frame_to_argb(plm_frame_t frame, byte[] dest, int stride)
		{
			fixed (byte* ptr = dest)
			{
				plm_frame_to_argb(frame, ptr, stride);
			}
		}

		public static void plm_frame_to_abgr(plm_frame_t frame, byte[] dest, int stride)
		{
			fixed (byte* ptr = dest)
			{
				plm_frame_to_abgr(frame, ptr, stride);
			}
		}
	}
}

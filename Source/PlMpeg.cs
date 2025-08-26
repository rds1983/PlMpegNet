namespace PlMpegSharp
{
	public static unsafe partial class PlMpeg
	{
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
	}
}

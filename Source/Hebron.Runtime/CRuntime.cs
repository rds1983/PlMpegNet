using System;
using System.Runtime.InteropServices;

namespace Hebron.Runtime
{
	public static unsafe class CRuntime
	{
		private static readonly string numbers = "0123456789";

		public static void* malloc(ulong size)
		{
			return malloc((long)size);
		}

		public static void* malloc(long size)
		{
			var ptr = Marshal.AllocHGlobal((int)size);

			MemoryStats.Allocated();

			return ptr.ToPointer();
		}

		public static void free(void* a)
		{
			if (a == null)
				return;

			var ptr = new IntPtr(a);
			Marshal.FreeHGlobal(ptr);
			MemoryStats.Freed();
		}

		public static void memcpy(void* a, void* b, long size)
		{
			var ap = (byte*)a;
			var bp = (byte*)b;
			for (long i = 0; i < size; ++i)
				*ap++ = *bp++;
		}

		public static void memcpy(void* a, void* b, ulong size)
		{
			memcpy(a, b, (long)size);
		}

		public static void memmove(void* a, void* b, long size)
		{
			void* temp = null;

			try
			{
				temp = malloc(size);
				memcpy(temp, b, size);
				memcpy(a, temp, size);
			}

			finally
			{
				if (temp != null)
					free(temp);
			}
		}

		public static void memmove(void* a, void* b, ulong size)
		{
			memmove(a, b, (long)size);
		}

		public static void* realloc(void* a, long newSize)
		{
			if (a == null)
				return malloc(newSize);

			var ptr = new IntPtr(a);
			var result = Marshal.ReAllocHGlobal(ptr, new IntPtr(newSize));

			return result.ToPointer();
		}

		public static void* realloc(void* a, ulong newSize)
		{
			return realloc(a, (long)newSize);
		}

		public static int abs(int v)
		{
			return Math.Abs(v);
		}

		public static void ArraySet(this int[] data, int value)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}

			for(var i = 0; i < data.Length; ++i)
			{
				data[i] = value;
			}
		}

		public static void ArraySet(this float[] data, float value)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}

			for (var i = 0; i < data.Length; ++i)
			{
				data[i] = value;
			}
		}
	}
}
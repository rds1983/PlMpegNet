namespace PlMpegNet
{
	internal static class Utility
	{
		public static bool ToBool(this int intResult) => intResult == 1 ? true : false;
		public static int ToInt(this bool par) => par ? 1 : 0;
	}
}

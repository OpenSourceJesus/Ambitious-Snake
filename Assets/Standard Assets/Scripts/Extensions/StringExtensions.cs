using System.Collections;

namespace Extensions
{
	public static class StringExtensions
	{
		public static string Strikethrough (this string str)
		{
			string output = "";
			foreach (char c in str)
				output = output + c + '\u0336';
			return output;
		}
		
		public static string SubstringStartEnd (this string str, int startIndex, int endIndex)
		{
			return str.Substring(startIndex, endIndex - startIndex);
		}
	}
}
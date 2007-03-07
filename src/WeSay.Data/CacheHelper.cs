using System;
using System.IO;
using System.Text;

namespace WeSay.Data
{
	public class CacheHelper
	{
		public static string EscapeFileNameString(string fileName)
		{
			char[] invalid = Path.GetInvalidFileNameChars();
			StringBuilder result = new StringBuilder(fileName.Length + 10);
			foreach (char c in fileName)
			{
				if (c != '%' && Array.IndexOf<char>(invalid, c) == -1)
				{
					result.Append(c);
				}
				else
				{
					result.Append('%');
					result.Append(Convert.ToByte(c).ToString("x"));
				}

			}
			return result.ToString();
		}
	}

}

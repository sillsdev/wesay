using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WeSay.Foundation
{
	public static class XmlEscapingUtilities
	{
		private static XmlNode _xmlNodeUsedForEscaping;
		public static string GetXmlSafeFromIllegalUnicodeCharacters(this string text)
		{
			//we actually want to preserve html markup, just escape the disallowed unicode characters
			text = text.Replace("<", "_lt;");
			text = text.Replace(">", "_gt;");
			text = text.Replace("&", "_amp;");
			text = text.Replace("\"", "_quot;");
			text = text.Replace("'", "_apos;");

			text = GetTextSafeForWritingToXmlAsTextOnly(text);
			//put it back, now
			text = text.Replace("_lt;", "<");
			text = text.Replace("_gt;", ">");
			text = text.Replace("_amp;", "&");
			text = text.Replace("_quot;", "\"");
			text = text.Replace("_apos;", "'");
			return text;
		}
		public static string GetTextSafeForWritingToXmlAsTextOnly(string text)
		{
			if (_xmlNodeUsedForEscaping == null)//notice, this is only done once per run
			{
				XmlDocument doc = new XmlDocument();
				_xmlNodeUsedForEscaping = doc.CreateElement("text", "x", "");
			}

			_xmlNodeUsedForEscaping.InnerText = text;
			text = _xmlNodeUsedForEscaping.InnerXml;
			return text;
		}

	}
}

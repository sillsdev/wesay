using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel
{
   public  class WeSayLexicalImporter
	{
		public static LexicalEntry LoadFromWeSayXml(System.Xml.XmlNode node)
		{
			LexicalEntry entry = new LexicalEntry();
			entry.LexicalForm = GetFieldFromWeSayXml(node, "lexicalForm", "unknown!");
			entry.Gloss = GetFieldFromWeSayXml(node, "sense/gloss", null);
			entry.Example = GetFieldFromWeSayXml(node, "sense/exampleGroup/example", null);
			return entry;
		}

		private static string GetFieldFromWeSayXml(System.Xml.XmlNode node, string elementName, string defaultValue)
		{
			System.Xml.XmlNode n = node.SelectSingleNode(elementName);
			if (n == null)
				return defaultValue;
			return n.InnerText;
		}
	}
}

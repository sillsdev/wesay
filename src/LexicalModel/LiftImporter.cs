using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.Language;

namespace WeSay.LexicalModel
{
   public  class LiftImporter
	{
	   private IList<LexEntry> _entries;

	   /// <summary>
	   ///
	   /// </summary>
	   /// <param name="entries">An existing list to fill</param>
	   public LiftImporter(IList<LexEntry> entries)
	   {
		   this._entries = entries;
	   }

	   public IList<LexEntry> ReadFile(string path)
	   {
		   XmlDocument doc =new XmlDocument();
		   doc.Load(path);
		   XmlNodeList entryNodes = doc.SelectNodes("./lift/entry");
		   foreach (XmlNode node in entryNodes)
		   {
			   this._entries.Add(this.ReadEntry(node));
		   }
		   return this._entries;
	   }

	  public static void ReadMultiTextOrNull(XmlNode node, string query, MultiText text)
	   {
		   XmlNode element = node.SelectSingleNode(query);
		   if (element != null)
		   {
			   ReadMultiText(element,text);
		   }
	   }

	   /// <summary>
	   /// this takes a text, rather than returning one just because the
	   /// lexical model classes currently always create their MultiText fields during the constructor.
	   /// </summary>
		public static void ReadMultiText(XmlNode node, MultiText text)
	   {
		   foreach(XmlNode form in node.SelectNodes("form"))
		   {
			   text.SetAlternative(GetStringAttribute(form, "lang"), form.InnerText);
		   }
	   }

	   private static string GetStringAttribute(XmlNode form, string attr)
	   {
		   return form.Attributes[attr].Value;
	   }

	   public static LexExampleSentence ReadExample(XmlNode xmlNode)
	   {
		   LexExampleSentence example = new LexExampleSentence();
		   ReadMultiTextOrNull(xmlNode, "source", example.Sentence);
		   //NB: will only read in one translation
		   ReadMultiTextOrNull(xmlNode, "trans", example.Translation);
		   return example;
	   }

	   public LexSense ReadSense(XmlNode xmlNode)
	   {
		   LexSense sense = new LexSense();
		   ReadMultiTextOrNull(xmlNode, "gloss", sense.Gloss);
		   foreach (XmlNode n in xmlNode.SelectNodes("example"))
		   {
			   sense.ExampleSentences.Add(ReadExample(n));
		   }
		   return sense;
	   }

	   private static string GetOptionalAttributeString(XmlNode xmlNode, string name)
	   {
		   XmlAttribute attr= xmlNode.Attributes[name];
		   if (attr == null)
			   return null;
		   return attr.Value;
	   }

	   public LexEntry ReadEntry(XmlNode xmlNode)
	   {
		   LexEntry entry;
		   string guid = GetOptionalAttributeString(xmlNode, "id");
		   if (guid != null)
		   {
			   entry = new LexEntry(new Guid(guid));
		   }
		   else
		   {
			   entry = new LexEntry();
		   }
		   ReadMultiText(xmlNode, entry.LexicalForm);

		  foreach (XmlNode n in xmlNode.SelectNodes("sense"))
		   {
			   entry.Senses.Add(ReadSense(n));
		   }

		   return entry;
	   }
   }
}

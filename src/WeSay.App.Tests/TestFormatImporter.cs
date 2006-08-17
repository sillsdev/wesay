using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WeSay.LexicalModel
{
   public class TestFormatImporter
	{
	   private  string _vernacularWS="th";
	   private  string _analysisWS="en";

	   public  void Load(System.Xml.XmlDocument doc, WeSay.Data.Db4oBindingList<LexEntry> db)
		{
			foreach (XmlNode node in doc.SelectNodes("test/entry"))
			{
				LexEntry entry = AddEntry(node);
				db.Add(entry);

			}
		}

		public  LexEntry AddEntry(System.Xml.XmlNode node)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm[_vernacularWS] = GetFieldFromWeSayXml(node, "lexeme", "unknown!");
			AddSenses(entry, node);
			AssignGuid(entry, node);
			return entry;
		}

	   private  void AddSenses(LexEntry entry, XmlNode node)
	   {
		   XmlNode n = node;//test format from eric is flat
		   //foreach (XmlNode n in node.SelectNodes("sense"))
		   //{
			   LexSense sense = entry.Senses.AddNew();
				sense.Gloss[_analysisWS] = GetFieldFromWeSayXml(n, "gloss", null);
				AddExamples(sense, n);
			//}
	   }

	   private  void AddExamples(LexSense sense, XmlNode node)
	   {
		   XmlNode n = node;//test format from eric is flat
		   //foreach (XmlNode n in node.SelectNodes("exampleGroup"))
		   //{
			   string e = GetFieldFromWeSayXml(node, "example", null);
			   if (e != null)
			   {
					LexExampleSentence sentence = sense.ExampleSentences.AddNew();
				  sentence.Sentence[_vernacularWS] = e;
			   }
		  //     }
		   //}
	   }

	   private  string GetFieldFromWeSayXml(System.Xml.XmlNode node, string elementName, string defaultValue)
		{
			System.Xml.XmlNode n = node.SelectSingleNode(elementName);
			if (n == null)
				return defaultValue;
			return n.InnerText;
		}

	   private static void AssignGuid(LexEntry entry, System.Xml.XmlNode node)
	   {
		   System.Xml.XmlAttribute id = node.Attributes["id"];
		   if (id != null)
			  entry.Guid = new Guid(id.Value);
		   else
			   entry.Guid = new Guid();
	   }
	}
}

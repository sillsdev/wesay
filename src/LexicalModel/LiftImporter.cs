using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.Language;

namespace WeSay.LexicalModel
{
   public  class LiftImporter
	{
	   public LiftImporter()
	   {
	   }

	  public void ReadMultiTextOrNull(XmlNode node, string query, MultiText text)
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
		public void ReadMultiText(XmlNode node, MultiText text)
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

	   public LexExampleSentence ReadExample(XmlNode xmlNode)
	   {
		   LexExampleSentence example = new LexExampleSentence();
		   ReadMultiTextOrNull(xmlNode, "example/source", example.Sentence);
		   //NB: will only read in one translation
		   ReadMultiTextOrNull(xmlNode, "example/trans", example.Translation);
		   return example;
	   }
   }
}

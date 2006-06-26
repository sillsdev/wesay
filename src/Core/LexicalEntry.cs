using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Core
{
	public class LexicalEntry
	{
		private string _lexicalForm;
		private string _gloss;
		private string _example;

		public LexicalEntry()
		{

		}

		public void LoadFromTestXml(System.Xml.XmlNode node)
		{
			_lexicalForm = GetFieldFromTestXml(node, "lexeme","unknown!");
			_gloss = GetFieldFromTestXml(node,"gloss",null);
			_example = GetFieldFromTestXml(node, "example",null);
		}
		private static string GetFieldFromTestXml(System.Xml.XmlNode node, string elementName, string defaultValue)
		{
			System.Xml.XmlNode n= node.SelectSingleNode(elementName);
			if(n==null)
				return defaultValue;
			return n.InnerText;
		}

	  public string LexicalForm {
		get {
		  return _lexicalForm;
		}
		set {
		  _lexicalForm = value;
		}
	  }

	  public string Gloss {
		get {
		  return _gloss;
		}
		set {
		  _gloss = value;
		}
	  }

	  public string Example {
		get {
		  return _example;
		}
		set {
		  _example = value;
		}
	  }
	}
}

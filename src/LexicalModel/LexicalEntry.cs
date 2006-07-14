using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel
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

	  private string StringOrEmpty(string s) {
		if (s == null) {
		  return string.Empty;
		}
		else {
		  return s;
		}
	  }

	  private string StringOrNull(string value) {
		if (value == null || value.Length == 0) {
		  return null;
		}
		else {
		  return value;
		}
	 }

	  public string LexicalForm {
		get {
		  return StringOrEmpty(_lexicalForm);
		}
		set {
		  _lexicalForm = StringOrNull(value);
		}
	  }


	  public string Gloss {
		get {
		  return StringOrEmpty(_gloss);
		}
		set {
		  _gloss = StringOrNull(value);
		}
	  }

	  public string Example {
		get {
		  return StringOrEmpty(_example);
		}
		set {
		  _example = StringOrNull(value);
		}
	  }

	  public void Initialize(LexicalEntry entry) {
		_lexicalForm = entry._lexicalForm;
		_gloss = entry._gloss;
		_example = entry._example;
	  }
	}
}

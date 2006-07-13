using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.LexicalModel;


namespace WeSay.Core
{
	public class XmlExporter
	{
		protected LexiconModel _model;
		public XmlExporter(LexiconModel model)
		{
			this._model = model;
		}

		public void ExportToZip(string path)
		{
		   string xmlPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(path) + ".xml");
		   Export(xmlPath);

		   ICSharpCode.SharpZipLib.Zip.FastZip f = new ICSharpCode.SharpZipLib.Zip.FastZip();
		   f.CreateZip(path, System.IO.Path.GetDirectoryName(xmlPath),false,System.IO.Path.GetFileName(xmlPath));

		   System.IO.File.Delete(xmlPath);
		}

		 public void Export(string path)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			XmlWriter writer = XmlTextWriter.Create(path, settings);
			writer.WriteStartDocument();
			writer.WriteStartElement("lexicon");

			foreach (LexicalEntry entry in this._model)
			{
				writer.WriteStartElement("entry");

				WriteMultilingualString(writer, "lexicalForm", entry.LexicalForm);
				WriteMultilingualString(writer, "gloss", entry.Gloss);
				WriteMultilingualString(writer, "example", entry.Example);
				writer.WriteEndElement();

			}
			 writer.WriteEndElement();
			 writer.WriteEndDocument();
			 writer.Close();
	   }

		private static void WriteMultilingualString(XmlWriter writer, string elementName, string s)
		{
			writer.WriteStartElement(elementName);
			writer.WriteAttributeString("ws", "??");
			writer.WriteString(s);
			writer.WriteEndElement();
		}
	}
}

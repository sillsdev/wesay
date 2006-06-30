using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
				writer.WriteElementString("lexicalForm",entry.LexicalForm);
				writer.WriteElementString("gloss",entry.Gloss);
				writer.WriteElementString("example", entry.Example);
				writer.WriteEndElement();

			}
			 writer.WriteEndElement();
			 writer.WriteEndDocument();
			 writer.Close();
	   }
	}
}

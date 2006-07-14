using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.LexicalModel;


namespace WeSay.Core
{
	public class WeSayExporter
	{
		protected LexiconModel _model;
		public WeSayExporter(LexiconModel model)
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
				WeSayLexicalExporter.Write(writer, entry);

			}
			 writer.WriteEndElement();
			 writer.WriteEndDocument();
			 writer.Close();
	   }
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace WeSay.App.Services
{
	/// <summary>
	/// Warning: This is a temporary thing (hopefully), which will be replaced by a multi-backend architecture some day
	/// </summary>
	public class HtmlArticleMaker
	{
		XslCompiledTransform _transformer;
		private XsltArgumentList _tranformArguments;
		public HtmlArticleMaker(string pathToWritingSystemPrefs, string pathToPartsOfSpeech)
		{
			_transformer = new XslCompiledTransform();

			using (Stream stream = GetXsltStream())
			{
				using (XmlReader xsltReader = XmlReader.Create(stream))
				{
					XsltSettings settings = new XsltSettings(true, true);
					_transformer.Load(xsltReader, settings, new XmlUrlResolver());
					xsltReader.Close();
				}
				stream.Close();
			}

			_tranformArguments = new XsltArgumentList();
			_tranformArguments.AddParam("writing-system-info-file", string.Empty, pathToWritingSystemPrefs);
			_tranformArguments.AddParam("grammatical-info-optionslist-file", string.Empty, pathToPartsOfSpeech);

		}

		private static Stream GetXsltStream()
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("WeSay.App.Services.lift2html.xsl");
		}

		public string GetHtmlFragment(string entryXml)
		{
			using (StringReader entryXmlReader = new StringReader(entryXml))
			{
				using (XmlReader reader = XmlReader.Create(entryXmlReader))
				{
					StringBuilder builder = new StringBuilder();
					using (XmlWriter writer = XmlWriter.Create(builder))
					{
						_transformer.Transform(reader, _tranformArguments, writer);
						return builder.ToString();
					}
				}
			}
		}
	}
}
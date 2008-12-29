using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using WeSay.LexicalModel;
using WeSay.Project;

namespace Addin.Transform
{
	public class FLExCompatibleXhtmlWriter
	{
		private readonly LexEntryRepository _repo;
		private readonly ViewTemplate _template;

		public FLExCompatibleXhtmlWriter(WeSay.LexicalModel.LexEntryRepository repo, ViewTemplate template)
		{
			_repo = repo;
			_template = template;
		}

		public void Write(TextWriter textWriter)
		{
			using (var writer = XmlWriter.Create(textWriter))
			{
				foreach (var entry in _repo.GetAllEntriesSortedByHeadword(_template.HeadwordWritingSystems[0]))
				{
					writer.WriteStartElement("div");
					writer.WriteAttributeString("class", "entry");

					writer.WriteEndElement();
				}
			}
		}
	}
}

using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftImportTestsWeSayVer1 : LiftImportTestsBase
	{
		protected override LiftImporter CreateImporter()
		{
			return new LiftImporterWeSay();
		}

		[Test]
		public void ReadsRandomEntityAsMultiText()
		{
			_doc.LoadXml("<sense><SemanticDomainsx><form lang='en'>body</form></SemanticDomainsx></sense>");
			MultiText text = new MultiText();
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.IsNotNull(sense.GetProperty<MultiText>("SemanticDomainsx"));
		}
	}
}
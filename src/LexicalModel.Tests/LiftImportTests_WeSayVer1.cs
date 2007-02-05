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

	}
}
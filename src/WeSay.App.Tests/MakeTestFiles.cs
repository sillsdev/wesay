using System.Xml;
using NUnit.Framework;
using WeSay.LexicalModel;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class MakeTestFiles
	{
		[Test]
		public void ImportWeSayXML()
		{
			string s = @"..\..\SampleProjects\Thai\thai.yap";
			if(System.IO.File.Exists(s))
			{
				System.IO.File.Delete(s);
			}
			using (WeSay.Data.Db4oDataSource src = new Db4oDataSource(s))
			{
			  using (WeSay.Data.Db4oBindingList<LexEntry> list = new Db4oBindingList<LexEntry>(src))
			  {
				//((WeSay.Data.ITransactionControl) list).AutoCommit = true;
				XmlDocument document = new XmlDocument();
				document.Load(@"..\..\SampleProjects\Thai\thai500.xml");

				TestFormatImporter importer = new TestFormatImporter();
				importer.Load(document, list);

				//  ((WeSay.Data.ITransactionControl) list).Commit();
			  }
			}
		}
	}
}

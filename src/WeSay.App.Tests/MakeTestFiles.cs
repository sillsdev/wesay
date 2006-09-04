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
			string s = System.IO.Path.Combine(
			System.IO.Path.Combine(System.IO.Path.Combine("..", ".."),
				System.IO.Path.Combine("SampleProjects", "Thai")),
				"lexicon.yap");
			//string s = System.IO.Path.GetFullPath(@"..\..\SampleProjects\Thai\thai.yap");
			if (System.IO.File.Exists(s))
			{
				System.IO.File.Delete(s);
			}
			using (WeSay.Data.Db4oDataSource src = new Db4oDataSource(s))
			{
				using (WeSay.Data.Db4oBindingList<LexEntry> list = new Db4oBindingList<LexEntry>(src))
				{
					//((WeSay.Data.ITransactionControl) list).AutoCommit = true;
					XmlDocument document = new XmlDocument();
					document.Load(System.IO.Path.Combine(
	System.IO.Path.Combine(System.IO.Path.Combine("..", ".."),
		System.IO.Path.Combine("SampleProjects", "Thai")),
		"thai5000.xml"));

					//                document.Load(@"..\..\SampleProjects\Thai\thai500.xml");

					TestFormatImporter importer = new TestFormatImporter();
					importer.Load(document, list);

					//  ((WeSay.Data.ITransactionControl) list).Commit();
				}
			}
		}
	}
}

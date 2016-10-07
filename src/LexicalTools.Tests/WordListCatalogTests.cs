using NUnit.Framework;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class WordListCatalogTests
	{
		/// <summary>
		/// Regression test, WS-1194
		/// </summary>
		[Test]
		public void CatalogGetOrAddWordList_NotContainedYet_JustAddsToCatalog()
		{
			var catalog = new WordListCatalog();
			Assert.IsNotNull(catalog.GetOrAddWordList("neverHeardOfMe.txt"));
		}
	}
}
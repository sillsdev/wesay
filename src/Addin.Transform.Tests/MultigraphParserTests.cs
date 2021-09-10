using Addin.Transform.PdfDictionary;
using NUnit.Framework;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public sealed class MultigraphParserTests
	{
		[Test]
		public void GetFirstMultigraph_EmptyMultigraphList_GivesFirstCharacterUppercase()
		{
			var p = new MultigraphParser(new string[] { });
			Assert.AreEqual("L", p.GetFirstMultigraph("listen"));
		}

		[Test]
		public void GetFirstMultigraph_WordStartsWithHyphen_GivesFirstLetterCharacterUppercase()
		{
			var p = new MultigraphParser(new string[] { });
			Assert.AreEqual("L", p.GetFirstMultigraph("-listen"));
		}

		[Test]
		public void GetFirstMultigraph_GivenSingleLettersFirst_IdentifiesWholeDigraph()
		{
			var p = new MultigraphParser(new string[] { "t", "th" });
			Assert.AreEqual("TH", p.GetFirstMultigraph("think"));
		}
		[Test]
		public void GetFirstMultigraph_GivenDigraphLettersFirst_IdentifiesWholeDigraph()
		{
			var p = new MultigraphParser(new string[] { "th", "t" });
			Assert.AreEqual("TH", p.GetFirstMultigraph("think"));
		}
		[Test]
		public void GetFirstMultigraph_DigraphDoesNotMatch_GivesSingleLetter()
		{
			var p = new MultigraphParser(new string[] { "th", "t" });
			Assert.AreEqual("T", p.GetFirstMultigraph("talk"));
		}

		[Test]
		public void GetFirstMultigraph_GivenSomeDigraphs_StillIdentifiesSingleCharacters()
		{
			var p = new MultigraphParser(new string[] { "ng", "th" });
			Assert.AreEqual("T", p.GetFirstMultigraph("talk"));
		}
		[Test]
		public void GetFirstMultigraph_GivenMultiLetterGraphs_IdentifiesTrigraphs()
		{
			var p = new MultigraphParser(new string[] { "oug", "ough", "ou" });
			Assert.AreEqual("OUGH", p.GetFirstMultigraph("ought"));
		}
	}
}

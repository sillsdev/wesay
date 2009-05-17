using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WeSay.LexicalModel.Foundation.Options;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class LiftRangeReaderTests
	{
		[Test]
		public void Load_RangeNotFound_GivesEmptyOptionList()
		{
			using(var f = new Palaso.TestUtilities.TempFile("<?xml version='1.0' encoding='UTF-8'?><lift-ranges/>"))
			{
				var r = new LiftRangeReader("foo");
				var list = r.LoadFromFile(f.Path);
				Assert.AreEqual(0, list.Options.Count);
			}
		}
		[Test]
		public void Load_RangeFoundButEmpty_GivesEmptyOptionList()
		{
			using (var f = new Palaso.TestUtilities.TempFile("<?xml version='1.0' encoding='UTF-8'?><lift-ranges> <range id='grammatical-info'></range></lift-ranges>"))
			{
				var r = new LiftRangeReader("foo");
				var list = r.LoadFromFile(f.Path);
				Assert.AreEqual(0, list.Options.Count);
			}
		}
		[Test]
		public void Load_RangeNormal_GivesGoodOptionList()
		{
			using (var f = new Palaso.TestUtilities.TempFile(@"<?xml version='1.0' encoding='UTF-8'?>
				<lift-ranges>
					<range id='foo'>
						<range-element guid='c6e7f0b9-2de7-4172-86bf-f865ddfe4a66' id='testId'>
						  <label>
							<form lang='en'>
							  <text>english label</text>
							</form>
							<form lang='fr'>
							  <text>french label</text>
							</form>
						  </label>
						  <abbrev>
							<form lang='en'>
							  <text>english abrev</text>
							</form>
							<form lang='fr'>
							  <text>french abrev</text>
							</form>
						  </abbrev>
						  <description>
							<form lang='en'>
							  <text>english description</text>
							</form>
						  </description>
					</range-element>
				</range>
				</lift-ranges>"))
			{
				var r = new LiftRangeReader("foo");
				var list = r.LoadFromFile(f.Path);

				Assert.AreEqual(1, list.Options.Count);
				Assert.AreEqual("testId", list.Options[0].Key);
				Assert.AreEqual("english abrev", list.Options[0].Abbreviation["en"]);
				Assert.AreEqual("french label", list.Options[0].Name["fr"]);
				Assert.AreEqual("english description", list.Options[0].Description["en"]);
			}
		}
	}
}

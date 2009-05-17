using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Palaso.TestUtilities;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel.Foundation.Options;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class LiftRangeWriterTests
	{
		[Test]
		public void Save_FileNotFound_CreatesFile()
		{
			using (var dir = new TemporaryFolder("LiftRangeWriterTests"))
			{
				var w = new LiftRangeWriter("foo");
				OptionsList optionList = new OptionsList();
				var path = dir.GetPathForNewTempFile(false);
				w.Save(path, optionList);
				File.Exists(path);
				Palaso.TestUtilities.AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("lift-ranges/range");
			}
		}
		[Test]
		public void Save_FileCorrupt_NotifiesUserAndReCreatesFile()
		{
			using (var dir = new TemporaryFolder("LiftRangeWriterTests"))
			{
				var w = new LiftRangeWriter("foo");
				OptionsList optionList = new OptionsList();
				var path = dir.GetPathForNewTempFile(false);
				File.WriteAllText(path, "<foobar/>");
				using (new Palaso.Reporting.ErrorReport.NonFatalErrorReportExpected())
				{
					w.Save(path, optionList);
				}
				File.Exists(path);
				Palaso.TestUtilities.AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("lift-ranges/range");
			}
		}

		[Test]
		public void Save_RangeDoesNotExist_CreatesIt()
		{
			using (var dir = new TemporaryFolder("LiftRangeWriterTests"))
			{
				var w = new LiftRangeWriter("numbers");
				OptionsList optionList = CreateSimpleList();
				var path = dir.GetPathForNewTempFile(false);
				File.WriteAllText(path, "<?xml version='1.0' encoding='UTF-8'?><lift-ranges/>");
				w.Save(path, optionList);
				CheckResultsFromSimpleList(path);
			}
		}

		private void CheckResultsFromSimpleList(string path)
		{
			AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("lift-ranges/range[@id='numbers']/range-element[@id='oneId']");
			AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("//label/form[@lang='en']/text[text()='one']");
			AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("//label/form[@lang='es']/text[text()='uno']");
		}

		private OptionsList CreateSimpleList()
		{
			OptionsList optionList = new OptionsList();
			MultiText text = new MultiText();
			text.SetAlternative("en", "one");
			text.SetAlternative("es", "uno");
			optionList.Options.Add(new Option("oneId", text));
			return optionList;
		}

		[Test]
		public void Save_RangeExists_ReplacesIt()
		{
			using (var dir = new TemporaryFolder("LiftRangeWriterTests"))
			{
				var w = new LiftRangeWriter("numbers");
				OptionsList optionList = CreateSimpleList();
				var path = dir.GetPathForNewTempFile(false);
				File.WriteAllText(path, "<?xml version='1.0' encoding='UTF-8'?><lift-ranges><range id='numbers'><foobar/></range></lift-ranges>");
				w.Save(path, optionList);
				AssertThatXmlIn.File(path).HasNoMatchForXpath("//foobar");
				CheckResultsFromSimpleList(path);
			}
		}
		[Test]
		public void Save_OptionListEmpty_OK()
		{
			using (var dir = new TemporaryFolder("LiftRangeWriterTests"))
			{
				var w = new LiftRangeWriter("numbers");
				var optionList = new OptionsList();
				var path = dir.GetPathForNewTempFile(false);
				File.WriteAllText(path, "<?xml version='1.0' encoding='UTF-8'?><lift-ranges></lift-ranges>");
				w.Save(path, optionList);
				AssertThatXmlIn.File(path).HasAtLeastOneMatchForXpath("lift-ranges/range[@id='numbers']");
			}
		}

		[Test, Ignore("not yet")]
		public void Save_OptionHasXMlDangerousContents_IsProperlyEscaped()
		{
		}

		[Test, Ignore("not yet")]
		public void Save_OptionListHasFLExHierarchyAttrs_HierachyAttrsSaved()
		{
		}
	}
}

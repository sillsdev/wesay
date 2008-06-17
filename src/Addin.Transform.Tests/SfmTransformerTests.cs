using System;
using System.IO;
using LiftIO.Validation;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.AddinLib;
using WeSay.Project;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class SfmTransformerTests
	{
		public SfmTransformer _addin;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_addin = new SfmTransformer();
			_addin.LaunchAfterTransform = false;
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_addin.PathToOutput))
			{
				File.Delete(_addin.PathToOutput);
			}
		}

		[Test]
		public void LaunchWithDefaultSettings()
		{
			_addin.Settings = new SfmTransformSettings();
			LaunchAddin();
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
		}

		[Test]
		public void LaunchWithNullGrepString()
		{
			LaunchWithConversionString(null);
		}

		[Test]
		public void LaunchWithEmptyGrepString()
		{
			LaunchWithConversionString("");
		}

		[Test]
		[ExpectedException(typeof (UnauthorizedAccessException))]
		public void ThrowsMeaningfulExceptionIfOutputFileIsLocked()
		{
			try
			{
				LaunchWithConversionString("g_en ge");
				File.SetAttributes(_addin.PathToOutput, FileAttributes.ReadOnly);
				LaunchWithConversionString("g_en ge");
			}
			finally
			{
				File.SetAttributes(_addin.PathToOutput, default(FileAttributes));
			}
		}

		[Test]
		public void ConvertsGlossMarker()
		{
			string result = LaunchWithConversionString("");
			Assert.IsTrue(result.Contains("\\ge"));
			Assert.IsFalse(result.Contains("g_en"));
		}

		/// <summary>
		/// this is a regression test.  It was not tagging pronunciation correctly
		/// </summary>
		[Test]
		public void TagsPronunciationWithPh()
		{
			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = "";
			settings.VernacularLanguageWritingSystemId = "bth";
			settings.EnglishLanguageWritingSystemId = "en";
			_addin.Settings = settings;

			string contents =
					string.Format(
							@"<?xml version='1.0' encoding='utf-8'?>
			<lift  version='{0}'>
			  <entry id='abo-abo_ID0ENG' >
				<lexical-unit>
				  <form lang='bth'>
					<text>bthform</text>
				  </form>
				</lexical-unit>
				<pronunciation>
				  <form lang='bth-fonipa'>
					<text>thePronunciation</text>
				  </form>
				</pronunciation>
			  </entry>
			</lift>",
							Validator.LiftVersion);

			string result = GetResultFromAddin(contents);
			Assert.IsTrue(result.Contains("\\lx bthform"));
			Assert.IsTrue(result.Contains("\\ph thePronunciation"));
		}

		[Test]
		public void UntypedRelationGetsCfTag()
		{
			/* um, relation without a type might not actually be valid lift */

			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = "";
			settings.VernacularLanguageWritingSystemId = "bth";
			settings.EnglishLanguageWritingSystemId = "en";
			_addin.Settings = settings;

			string contents =
					string.Format(
							@"<?xml version='1.0' encoding='utf-8'?>
			<lift  version='{0}'>
			  <entry id='abo-abo_ID0ENG' >
					  <relation ref='ebo' />
			  </entry>
			</lift>",
							Validator.LiftVersion);

			string result = GetResultFromAddin(contents);
			Assert.IsTrue(result.Contains("\\lf unknown = ebo"));
		}

		[Test]
		public void RelationTaggedWthType_OutputsTypeForTagAndLexemeFormOfTarget()
		{
			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = "";
			settings.VernacularLanguageWritingSystemId = "bth";
			settings.EnglishLanguageWritingSystemId = "en";
			_addin.Settings = settings;

			string contents =
					string.Format(
							@"<?xml version='1.0' encoding='utf-8'?>
			<lift  version='{0}'>
			  <entry id='abo-abo_ID0ENG' >
					  <relation type='composition' ref='one' />
			  </entry>
			  <entry id='one' >
				<lexical-unit>
				  <form lang='bth'>
					<text>lexemeOfOne</text>
				  </form>
				</lexical-unit>
				</entry>
			</lift>",
							Validator.LiftVersion);
			string result = GetResultFromAddin(contents);
			Console.WriteLine(result);
			Assert.IsTrue(result.Contains("\\lf composition = lexemeOfOne"));
		}

		[Test]
		public void Relation_TargetNotFoundOutputsId()
		{
			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = "";
			settings.VernacularLanguageWritingSystemId = "bth";
			settings.EnglishLanguageWritingSystemId = "en";
			_addin.Settings = settings;

			string contents =
					string.Format(
							@"<?xml version='1.0' encoding='utf-8'?>
			<lift  version='{0}'>
			  <entry id='one' >
					  <relation type='composition' ref='id-of-missing' />
			  </entry>
			</lift>",
							Validator.LiftVersion);
			string result = GetResultFromAddin(contents);
			Assert.IsTrue(result.Contains("\\lf composition = id-of-missing"));
		}

		[Test]
		public void ConvertsDatesToToolboxFormat()
		{
			string result = LaunchWithConversionString("");
			Assert.IsTrue(result.Contains("\\dt 11/Feb/2008"));
		}

		[Test]
		public void SemDomConvertedTo_sd()
		{
			string result = LaunchWithConversionString("");
			Assert.IsTrue(result.Contains("\\sd"));
		}

		[Test]
		public void EmptyBaseFormNotOutput()
		{
			string result = LaunchWithConversionString("");
			Assert.IsTrue(result.Contains("\\lf BaseForm"));
			//should only have one, since the input has one empty, one non-empty
			Assert.IsTrue(result.LastIndexOf("\\lf BaseForm") == result.IndexOf("\\lf BaseForm"));
		}

		[Test]
		public void LaunchWithEmptyMissingConversionPiece()
		{
			LaunchWithConversionString("g_en");
		}

		[Test]
		public void LaunchWithExtraConversionPiece()
		{
			LaunchWithConversionString("g_en x y");
		}

		[Test]
		public void BogusExpressionDoesntCrash()
		{
			ErrorReport.JustRecordNonFatalMessagesForTesting = true;
			Assert.IsNull(ErrorReport.PreviousNonFatalMessage);
			LaunchWithConversionString("{foo " //missing "to"
									   + Environment.NewLine + "{foo $3" //bogus group refference
									   + Environment.NewLine + "[ foo");
			// this is the one that is failing
			Assert.IsNotNull(ErrorReport.PreviousNonFatalMessage);
		}

		[Test]
		public void LaunchWithRecursiveConversionPiece()
		{
			LaunchWithConversionString("hello hellothere");
		}

		[Test]
		public void CanGetXsltFromResource()
		{
			ProjectInfo info = WeSayWordsProject.Project.GetProjectInfoForAddin(null);
			string path = info.LocateFile("lift2sfm.xsl");
			if (!string.IsNullOrEmpty(path))
			{
				File.Delete(path);
			}
			Stream stream = LiftTransformer.GetXsltStream(info, "lift2sfm.xsl");
			Assert.IsNotNull(stream);
		}

		private string LaunchWithConversionString(string conversions)
		{
			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = conversions;
			_addin.Settings = settings;
			return LaunchAddin();
		}

		private string LaunchAddin()
		{
			string contents =
					string.Format(
							@"<?xml version='1.0' encoding='utf-8'?>
			<lift  version='{0}'>
				<entry id='one'
					dateCreated='2008-02-06T09:46:31Z'
					dateModified='2008-02-11T04:13:06Z'>
					<relation type='BaseForm' ref=''/>
					<relation type='BaseForm' ref='two'/>
				   <sense>
						<gloss lang='en'><text>hello</text></gloss>
						<trait name='SemanticDomainDdp4' value='1.1' />
					 </sense>

				</entry>
				<entry id='two'>
				</entry>
			</lift>",
							Validator.LiftVersion);
			return GetResultFromAddin(contents);
		}

		private string GetResultFromAddin(string contents)
		{
			if (WeSayWordsProject.Project.LiftIsLocked)
			{
				WeSayWordsProject.Project.ReleaseLockOnLift();
			}
			File.WriteAllText(WeSayWordsProject.Project.PathToLiftFile, contents);
			_addin.Launch(null, WeSayWordsProject.Project.GetProjectInfoForAddin(null));
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
			string result = File.ReadAllText(_addin.PathToOutput);
			Assert.Greater(result.Trim().Length, 0);

			return result;
		}
	}
}
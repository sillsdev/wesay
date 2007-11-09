using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.AddinLib;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class SfmTransformerTests
	{
		public Transform.SfmTransformer _addin;

		[SetUp]
		public void Setup()
		{
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_addin = new Transform.SfmTransformer();
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

		[Test, ExpectedException(typeof(UnauthorizedAccessException))]
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
			Assert.IsTrue(result.Contains("\\base"));
			//should only have one, since the input has one empty, one non-empty
			Assert.IsTrue(result.LastIndexOf("\\base") == result.IndexOf("\\base"));
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
			Palaso.Reporting.ErrorReport.JustRecordNonFatalMessagesForTesting = true;
			Assert.IsNull(ErrorReport.PreviousNonFatalMessage);
			LaunchWithConversionString("{foo "  //missing "to"
				+System.Environment.NewLine+"{foo $3" //bogus group refference
				+System.Environment.NewLine+"[ foo"); // this is the one that is failing
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
			ProjectInfo info = WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin();
			string path = info.LocateFile("lift2sfm.xsl");
			if (!string.IsNullOrEmpty(path))
			{
				File.Delete(path);
			}
			Stream stream = LiftTransformer.GetXsltStream(info,
										  "lift2sfm.xsl");
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
			string contents = @"<?xml version='1.0' encoding='utf-8'?>
			<lift  version='0.10'>
				<entry id='one'>
					<relation name='BaseForm' ref=''/>
					<relation name='BaseForm' ref='two'/>
				   <sense>
						<gloss lang='en'><text>hello</text></gloss>
						<trait name='SemanticDomainDdp4' value='1.1' />
					 </sense>
				</entry>
				<entry id='two'>
				</entry>
			</lift>";
			if (WeSay.Project.WeSayWordsProject.Project.LiftIsLocked)
			{
				WeSay.Project.WeSayWordsProject.Project.ReleaseLockOnLift();
			}
			File.WriteAllText(WeSay.Project.WeSayWordsProject.Project.PathToLiftFile, contents);
			_addin.Launch(null, WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin());
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
			string result =File.ReadAllText(_addin.PathToOutput);
			Assert.Greater(result.Trim().Length, 0);

			return result;
		}

	}

}
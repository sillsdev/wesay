using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
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
			string result = LaunchWithConversionString("g_en ge");
			Assert.IsTrue(result.Contains("\\ge"));
			Assert.IsFalse(result.Contains("g_en"));
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
		public void LaunchWithRecursiveConversionPiece()
		{
			LaunchWithConversionString("g_en g_en");
		}
		[Test]
		public void CanGetXsltFromResource()
		{
			Stream stream = LiftTransformer.GetXsltStream(WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin(),
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
<lift  version='0.10'><entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry><entry id='two'/></lift>";
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
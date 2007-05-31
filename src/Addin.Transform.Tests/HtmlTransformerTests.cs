using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WeSay.AddinLib;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class HtmlTransformerTests
	{
		public Transform.HtmlTransformer _addin;

		[SetUp]
		public void Setup()
		{
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_addin = new Transform.HtmlTransformer();
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
			LaunchAddin();
			Assert.IsTrue(File.Exists(_addin.PathToOutput));
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
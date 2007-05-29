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
			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = null;
			_addin.Settings = settings;
			LaunchAddin();
		}

		[Test]
		public void LaunchWithEmptyGrepString()
		{
			SfmTransformSettings settings = new SfmTransformSettings();
			settings.SfmTagConversions = "";
			_addin.Settings = settings;
			LaunchAddin();
		}

		private void LaunchAddin()
		{
			_addin.Launch(null, WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin());
			Assert.IsTrue(File.Exists(_addin.PathToOutput));

		}

	}

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using WeSay.App.Services;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests.Services
{
	[TestFixture]
	public class HtmlArticleMakerTests
	{
		private ProjectDirectorySetupForTesting _projectDirectory;
	 //   public WeSayWordsProject _project;

		/// <summary>
		/// Db4oProjectSetupForTesting is extremely time consuming to setup, so we reuse it.
		/// </summary>
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			_projectDirectory = new ProjectDirectorySetupForTesting("does not matter");
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_projectDirectory.Dispose();
		}



		[SetUp]
		public void Setup()
		{

		}

		[TearDown]
		public void TearDown()
		{

		}


		[Test]
		public void SmokeTest()
		{
			string contents = @"<entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry>";
			WeSay.App.Services.HtmlArticleMaker maker = new HtmlArticleMaker(_projectDirectory.PathToWritingSystemFile, _projectDirectory.PathToFactoryDefaultsPartsOfSpeech);
			string s = maker.GetHtmlFragment(contents);
			Assert.IsTrue(s.Contains("<html>"));
			Assert.IsTrue(s.Contains("hello"));
		}



	}
}
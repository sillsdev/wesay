using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WeSay.AddinLib;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class HtmlTransformerTests
	{
		public Transform.HtmlTransformer _addin;
		private Db4oRecordListManager _recordListManager;
		private string _dbFile;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_dbFile = Path.GetTempFileName();
			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _dbFile);
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);

			Lexicon.Init(_recordListManager);

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
			_recordListManager.Dispose();
			File.Delete(_dbFile);
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

		[Test]
		public void CanGetXsltFromResource()
		{
			ProjectInfo info = WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin();
			string path = info.LocateFile("plift2html.xsl");
			if (!string.IsNullOrEmpty(path))
			{
				File.Delete(path);
			}
			Stream stream = LiftTransformer.GetXsltStream(info,
										  "plift2html.xsl");
			Assert.IsNotNull(stream);
		}
	}

}
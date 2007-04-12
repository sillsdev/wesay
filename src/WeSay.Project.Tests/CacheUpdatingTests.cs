using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class CacheUpdatingTests
	{
		private string _experimentDir;

		private string _projectDir;
		private string _weSayDir;
		private WeSayWordsProject _project;
		private string _liftPath;

		[SetUp]
		public void Setup()
		{
			_experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			_projectDir = MakeDir(_experimentDir, "TestProj");
			 _weSayDir = MakeDir(_projectDir, "WeSay");
			_liftPath = Path.Combine(_weSayDir, "test.lift");
			File.CreateText(_liftPath).Close();
			_project = new WeSayWordsProject();
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(_experimentDir, true);
			_project.Dispose();
		}
		private static string MakeDir(string existingParent, string newChild)
		{
			string dir = Path.Combine(existingParent, newChild);
			Directory.CreateDirectory(dir);
			return dir;
		}
		[Test]
		public void MissingCacheDirTriggersUpdate()
		{
			OutOfDate();
		}

		[Test]
		public void EmptyCacheDirTriggersUpdate()
		{
			MakeDir(_weSayDir, "Cache");
			OutOfDate();
		}

		[Test]
		public void OlderWordsFileTriggersUpdate()
		{
			string cacheDir = MakeDir(_weSayDir, "Cache");
			string s = Path.Combine(cacheDir, "test.words");
			File.Create(s).Dispose();
			File.SetLastWriteTimeUtc(s, TimeOfUpdate.Subtract(new TimeSpan(1))); //just one tick older
			OutOfDate();
		}

		private DateTime TimeOfUpdate
		{
			get
			{
			   return File.GetLastWriteTimeUtc(_liftPath);
			}
		}

		private void OutOfDate()
		{
			Assert.IsTrue(_project.GetCacheIsOutOfDate(_liftPath));
		}
	}

}
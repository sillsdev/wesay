using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Foundation.Tests.TestHelpers;
using WeSay.LexicalModel;
using WeSay.LexicalTools.AddPictures;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class AddPicturesTasksAbnormalSetupTests
	{
		[Test, ExpectedException(typeof(Palaso.Reporting.ConfigurationException))]
		public void Activate_IndexNotFound_GivesUserMessage()
		{
		   using(new Palaso.Reporting.ErrorReport.NonFatalErrorReportExpected())
			{
				using (var repoFile = new WeSay.Foundation.Tests.TestHelpers.TempLiftFile(""))
				{
					using(var repo = new LexEntryRepository(repoFile.Path))
					{
						AddPicturesConfig config = MakeConfig("Bogus.txt");
						var task = new AddPicturesTask(config, repo,
							new TaskMemoryRepository(),
							new FileLocator(new string[0])  );
						task.Activate();
					}
				}
			}
		}

		private AddPicturesConfig MakeConfig(string indexName)
		{
			return new AddPicturesConfig(string.Format("<task taskName='AddMissingInfo' visible='true'><indexFileName>{0}</indexFileName></task>", indexName));
		}
	}

	[TestFixture]
	public class AddPicturesTasksTests
	{
		private TempLiftFile _repoFile;
		private LexEntryRepository _repo;
		private AddPicturesTask _task;

		[SetUp]
		public void Setup()
		{
			_repoFile = new WeSay.Foundation.Tests.TestHelpers.TempLiftFile("");
			_repo = new LexEntryRepository(_repoFile.Path);
			AddPicturesConfig config = new AddPicturesConfig(string.Format("<task taskName='AddMissingInfo' visible='true'><indexFileName>{0}</indexFileName></task>", "ArtOfReadingIndexV3_en.txt"));
			_task = new AddPicturesTask(config, _repo,
										new TaskMemoryRepository(),
										new FileLocator(new string[]{WeSay.Project.BasilProject.ApplicationCommonDirectory}));
			_task.Activate();
		}
		[TearDown]
		public void TearDown()
		{
			_task.Deactivate();
			_repo.Dispose();
			_repoFile.Dispose();
		}


		[Test]
		public void GetMatchingPictures_OnKeyWordHasManyMatches_GetManyMatches()
		{
			var matches = _task.GetMatchingPictures("duck");
			Assert.Less(30, matches.Count);
		}

		[Test]
		public void GetMatchingPictures_OnKeyWordHasTwoMatches_GetTwoMatches()
		{
			var matches = _task.GetMatchingPictures("xyz");
			Assert.AreEqual(0, matches.Count);
		}

		[Test]
		public void GetMatchingPictures_TwoKeyWords_GetMatchesOnBoth()
		{
			var duckMatches = _task.GetMatchingPictures("duck");
			var bothMatches = _task.GetMatchingPictures("duck sheep");
			Assert.Greater(bothMatches.Count, duckMatches.Count);
		}

		[Test]
		public void GetMatchingPictures_KeyWordsMatchSamePicture_PictureOnlyListedOnce()
		{
			var batMatches = _task.GetMatchingPictures("bat");
			var bothMatches = _task.GetMatchingPictures("bat bat");
			Assert.AreEqual(bothMatches.Count, batMatches.Count);

			bothMatches = _task.GetMatchingPictures("bat animal");
			List<string> found = new List<string>();
			foreach (var s in bothMatches)
			{
				Assert.IsFalse(found.Contains(s));
				found.Add(s);
			}
		}
	}
}

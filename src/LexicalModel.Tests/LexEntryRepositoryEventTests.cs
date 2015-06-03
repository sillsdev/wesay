using NUnit.Framework;
using SIL.DictionaryServices.Model;
using Palaso.TestUtilities;


namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryEventTests
	{
		private bool _gotEventCall;

		[SetUp]
		public void Setup()
		{
			_gotEventCall = false;
		}


		[Test]
		public void NewEntry_ByEntry_TriggersModifiedEntryAdded()
		{
			using (var f = new TemporaryFolder("eventTests"))
			{
				using (var r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					r.AfterEntryModified += OnEvent;
					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);
					Assert.IsTrue(_gotEventCall);
				}
			}
		}

		[Test]
		public void ModifiedEntry_ByEntry_TriggersModifiedEntryAdded()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);
					r.AfterEntryModified += OnEvent;
					entry.Senses.Add(new LexSense());
					r.SaveItem(entry);
					Assert.IsTrue(_gotEventCall);
				}
			}
		}

		[Test]
		public void DeleteEntry_ById_TriggersAfterEntryDeleted()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					r.AfterEntryDeleted +=OnEvent;

					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);

					r.DeleteItem(r.GetId(entry));
					Assert.IsTrue(_gotEventCall);
				}
			}
		}

		[Test]
		public void DeleteEntry_ByEntry_TriggersAfterEntryDeleted()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					r.AfterEntryDeleted += OnEvent;

					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);

					r.DeleteItem(entry);
					Assert.IsTrue(_gotEventCall);
				}
			}
		}


		void OnEvent(object sender, LexEntryRepository.EntryEventArgs e)
		{
			_gotEventCall = true;
		}
	}
}
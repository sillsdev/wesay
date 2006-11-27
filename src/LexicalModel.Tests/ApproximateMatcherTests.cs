using System.Collections.Generic;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class ApproximateMatcherTests
	{
		private IRecordList<LexEntry> _records;
		private IRecordListManager _recordListManager;
		//private EntryDetailControl _entryDetailControl;
		string _FilePath;


		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			_FilePath = System.IO.Path.GetTempFileName();

			//REVIEW EA (JH): is this ok?  these test are no longer reliant on the tool itself
//            BasilProject.InitializeForTests();
//
//            string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId };
//            FieldInventory fieldInventory = new FieldInventory();
//            fieldInventory.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));
//
			this._recordListManager = new Db4oRecordListManager(_FilePath);
			this._records = _recordListManager.GetListOfType<LexEntry>();
		   // this.Db4oLexQueryHelper = new EntryDetailControl(this._recordListManager, fieldInventory);
		}
		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			_records.Dispose();
		   // _entryDetailControl.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		private LexEntry AddEntry(string lexicalForm)
		{
			LexEntry entry = new LexEntry();
		  //  entry.LexicalForm[BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId] = lexicalForm;
			entry.LexicalForm["xyz"] = lexicalForm;
			this._records.Add(entry);
			return entry;
		}

		private static bool ContainsEntryWithLexicalForm(IList<LexEntry> entries, string lexicalForm)
		{
			foreach (LexEntry entry in entries)
			{
				if (lexicalForm == entry.ToString())
				{
					return true;
				}
			}
			return false;
		}

		[Test]
		public void Equal()
		{
			AddEntry("distance");
			AddEntry("distances");
			AddEntry("distane");
			AddEntry("destance");
			AddEntry("distence");
			IList<LexEntry> closest = ApproximateMatcher.FindEntriesWithClosestLexemeForms("distance",_recordListManager,_records );
		   Assert.AreEqual(1, closest.Count);
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "distance"));
		}


		[Test]
		public void Closest_EditDistance1()
		{
			AddEntry("a1234567890"); // insertion at beginning
			AddEntry("1a234567890"); // insertion in middle
			AddEntry("1234567890a"); // insertion at end
			AddEntry("234567890"); // deletion at beginning
			AddEntry("123457890"); // deletion in middle
			AddEntry("123456789"); // deletion at end
			AddEntry("a234567890"); //substitution at beginning
			AddEntry("1234a67890"); //substitution in middle
			AddEntry("123456789a"); //substitution at end
			AddEntry("2134567890"); //transposition at beginning
			AddEntry("1234657890"); //transposition in middle
			AddEntry("1234567809"); //transposition at end

			AddEntry("aa1234567890"); // noise
			AddEntry("1a23456789a0");
			AddEntry("1a2a34567890");
			AddEntry("1a23a4567890");
			AddEntry("1a234a567890");
			AddEntry("1a2345a67890");
			AddEntry("ab34567890");
			AddEntry("1234ab7890");
			AddEntry("12345678ab");
			AddEntry("2134567809");
			AddEntry("1235467980");


			IList<LexEntry> closest = ApproximateMatcher.FindEntriesWithClosestLexemeForms("1234567890",_recordListManager,_records);
			Assert.AreEqual(12, closest.Count);
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "a1234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1a234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234567890a"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "123457890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "123456789"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "a234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234a67890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "123456789a"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "2134567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234657890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234567809"));
		}

		[Test]
		public void ClosestAndNextClosest_EditDistance0and1()
		{
			AddEntry("a1234567890"); // insertion at beginning
			AddEntry("1a234567890"); // insertion in middle
			AddEntry("1234567890a"); // insertion at end
			AddEntry("234567890"); // deletion at beginning
			AddEntry("123457890"); // deletion in middle
			AddEntry("123456789"); // deletion at end
			AddEntry("a234567890"); //substitution at beginning
			AddEntry("1234a67890"); //substitution in middle
			AddEntry("123456789a"); //substitution at end
			AddEntry("2134567890"); //transposition at beginning
			AddEntry("1234657890"); //transposition in middle
			AddEntry("1234567809"); //transposition at end
			AddEntry("1234567890"); // identity

			AddEntry("aa1234567890"); // noise
			AddEntry("1a23456789a0");
			AddEntry("1a2a34567890");
			AddEntry("1a23a4567890");
			AddEntry("1a234a567890");
			AddEntry("1a2345a67890");
			AddEntry("ab34567890");
			AddEntry("1234ab7890");
			AddEntry("12345678ab");
			AddEntry("2134567809");
			AddEntry("1235467980");


			IList<LexEntry> closest = ApproximateMatcher.FindClosestAndNextClosest("1234567890",_recordListManager,_records );
			Assert.AreEqual(13, closest.Count);
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "a1234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1a234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234567890a"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "123457890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "123456789"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "a234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234a67890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "123456789a"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "2134567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234657890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "1234567809"));
		}

		[Test]
		public void ClosestAndNextClosest_EditDistance1and3()
		{
			AddEntry("a1234567890");
			AddEntry("aaa1234567890");

			AddEntry("aaa1234567890a"); // noise

			IList<LexEntry> closest = ApproximateMatcher.FindClosestAndNextClosest("1234567890", _recordListManager, _records);
			Assert.AreEqual(2, closest.Count);
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "a1234567890"));
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "aaa1234567890"));
		}


		/// <summary>
		/// This test was created after we found that LexEntries did not
		/// cascade on their delete and so lexical forms could be found even when
		/// their entry was deleted, causing a crash.
		/// </summary>
		[Test]
		public void Find_AfterDeleted_NotFound()
		{
			LexEntry test = AddEntry("test");
			AddEntry("test1");
			this._records.Remove(test);

			IList<LexEntry> closest = ApproximateMatcher.FindEntriesWithClosestLexemeForms("test", _recordListManager, _records);
			Assert.AreEqual(1, closest.Count);
			Assert.IsTrue(ContainsEntryWithLexicalForm(closest, "test1"));
		}


		//[Test]
		//public void Time()
		//{
		//    Stopwatch stopwatch = new Stopwatch();
		//    stopwatch.Start();
		//    Random random = new Random();
		//    for (int i = 0; i < 5000; i++)
		//    {
		//        string LexicalForm = string.Empty;
		//        for (int j = 0; j < 10; j++) //average word length of 10 characters
		//        {
		//            LexicalForm += Convert.ToChar(random.Next(Convert.ToInt16('a'), Convert.ToInt16('z')));
		//        }
		//        AddEntry(LexicalForm);
		//    }
		//
		//    stopwatch.Stop();
		//    Console.WriteLine("Time to initialize " + stopwatch.Elapsed.ToString());
		//
		//    stopwatch.Reset();
		//    stopwatch.Start();
		//    Db4oLexQueryHelper.FindClosest("something");
		//    stopwatch.Stop();
		//    Console.WriteLine("Time to find " + stopwatch.Elapsed.ToString());
		//}

	}
}
using System.Collections;
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
		private IList<string> _forms;

		[SetUp]
		public void Setup()
		{
			_forms = new List<string>();
		}

		private void AddEntry(string form)
		{
			_forms.Add(form);
		}

		[Test]
		public void Equal()
		{
			AddEntry("distance");
			AddEntry("distances");
			AddEntry("distane");
			AddEntry("destance");
			AddEntry("distence");
			IList closest = (IList) ApproximateMatcher.FindClosestForms("distance", _forms);
		   Assert.AreEqual(1, closest.Count);
			Assert.Contains("distance", closest);
		}

		[Test]
		public void Prefixes()
		{
			AddEntry("distance");
			AddEntry("distances");
			AddEntry("distane");
			AddEntry("destance");
			AddEntry("distence");
			IList closest = (IList) ApproximateMatcher.FindClosestAndPrefixedForms("dist", _forms);
			Assert.AreEqual(4, closest.Count);
			Assert.Contains("distance", closest);
			Assert.Contains("distances", closest);
			Assert.Contains("distane", closest);
			Assert.Contains("distence", closest);
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


			IList closest = (IList)ApproximateMatcher.FindClosestForms("1234567890", _forms);
			Assert.AreEqual(12, closest.Count);
			Assert.Contains("a1234567890", closest);
			Assert.Contains("1a234567890", closest);
			Assert.Contains("1234567890a", closest);
			Assert.Contains("234567890", closest);
			Assert.Contains("123457890", closest);
			Assert.Contains("123456789", closest);
			Assert.Contains("a234567890", closest);
			Assert.Contains("1234a67890", closest);
			Assert.Contains("123456789a", closest);
			Assert.Contains("2134567890", closest);
			Assert.Contains("1234657890", closest);
			Assert.Contains("1234567809", closest);
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


			IList closest = (IList) ApproximateMatcher.FindClosestAndNextClosestForms("1234567890", _forms);
			Assert.AreEqual(13, closest.Count);
			Assert.Contains("1234567890", closest);
			Assert.Contains("a1234567890", closest);
			Assert.Contains("1a234567890", closest);
			Assert.Contains("1234567890a", closest);
			Assert.Contains("234567890", closest);
			Assert.Contains("123457890", closest);
			Assert.Contains("123456789", closest);
			Assert.Contains("a234567890", closest);
			Assert.Contains("1234a67890", closest);
			Assert.Contains("123456789a", closest);
			Assert.Contains("2134567890", closest);
			Assert.Contains("1234657890", closest);
			Assert.Contains("1234567809", closest);
		}

		[Test]
		public void ClosestAndNextClosest_EditDistance1and3()
		{
			AddEntry("a1234567890");
			AddEntry("aaa1234567890");

			AddEntry("aaa1234567890a"); // noise

			IList closest = (IList) ApproximateMatcher.FindClosestAndNextClosestForms("1234567890", _forms);
			Assert.AreEqual(2, closest.Count);
			Assert.Contains("a1234567890", closest);
			Assert.Contains("aaa1234567890", closest);
		}


		/// <summary>
		/// This test was created after we found that LexEntries did not
		/// cascade on their delete and so lexical forms could be found even when
		/// their entry was deleted, causing a crash.
		/// </summary>
		[Test]
		public void Find_AfterDeleted_NotFound() // move to sorted Cache Tests
		{
			//LexEntry test = AddEntry("test");
			//AddEntry("test1");
			//this._records.Remove(test);

			//IList<LexEntry> closest = ApproximateMatcher.FindEntriesWithClosestLexemeForms("test", _forms);
			//Assert.AreEqual(1, closest.Count);
			//Assert.Contains("test1", closest);
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
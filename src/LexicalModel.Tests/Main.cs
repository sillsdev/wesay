using System;
using NUnit.Framework;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using WeSay.LexicalModel;

namespace LexicalModel.Tests
{
	[TestFixture]
	public class Test1
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void CreationDate()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(entry.CreationDate.Date, DateTime.Today);
		}

		[Test]
		public void ModifiedDateOnCreation()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(entry.ModifiedDate, entry.CreationDate);
		}

		[Test]
		public void ModifiedDateAfterMultiTextChange()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(entry.ModifiedDate, entry.CreationDate);
			long start = entry.ModifiedDate.Ticks;
			System.Threading.Thread.Sleep(10);//else modtime doesn't change
			entry.LexicalForm["foo"] = "hello";
			Assert.Greater((decimal)entry.ModifiedDate.Ticks, start);
		}
	}
}

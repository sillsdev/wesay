using System;
using NUnit.Framework;
using WeSay.Data;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class EntryDetailTaskTests : TaskBaseTests
	{
		IRecordListManager _recordListManager;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			_recordListManager = new InMemoryRecordListManager();
			_task = new EntryDetailTask(_recordListManager);
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
		}

		[Test]
		public void Create()
		{
			Assert.IsNotNull(_task);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullRecordListManager_Throws()
		{
			new EntryDetailTask(null);
		}
	}

}
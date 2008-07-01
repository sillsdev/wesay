using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class ResultSetTests
	{
		[Test]
		public void FindFirstIndex_RepositoryIdEqualToOneInList_Index()
		{
			MemoryRepository<TestItem> repository = new MemoryRepository<TestItem>();
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(repository, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(repository, new TestRepositoryId(12)));
			results.Add(new RecordToken<TestItem>(repository, new TestRepositoryId(1)));
			results.Add(new RecordToken<TestItem>(repository, new TestRepositoryId(3)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(repository, results);

			int index = resultSet.FindFirstIndex(new TestRepositoryId(1));
			Assert.AreEqual(2, index);
		}

	}

}
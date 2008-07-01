using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class ResultSetTests
	{
		class MyRepositoryId:RepositoryId
		{
			private readonly int id;

			public MyRepositoryId(int id)
			{
				this.id = id;
			}

			public override int CompareTo(RepositoryId other)
			{
				MyRepositoryId otherAsMy = other as MyRepositoryId;
				if(otherAsMy == null)
				{
					return 1;
				}
				return Comparer<int>.Default.Compare(id, otherAsMy.id);
			}

			public override bool Equals(RepositoryId other)
			{
				MyRepositoryId otherAsMy = other as MyRepositoryId;
				if (otherAsMy == null)
				{
					return false;
				}
				return id == otherAsMy.id;
			}
		}

		private class MyType
		{
		}

		[Test]
		public void FindFirstIndex_RepositoryIdEqualToOneInList_Index()
		{
			MemoryRepository<MyType> repository = new MemoryRepository<MyType>();
			List<RecordToken<MyType>> results = new List<RecordToken<MyType>>();

			results.Add(new RecordToken<MyType>(repository, new MyRepositoryId(8)));
			results.Add(new RecordToken<MyType>(repository, new MyRepositoryId(12)));
			results.Add(new RecordToken<MyType>(repository, new MyRepositoryId(1)));
			results.Add(new RecordToken<MyType>(repository, new MyRepositoryId(3)));

			ResultSet<MyType> resultSet = new ResultSet<MyType>(repository, results);

			int index = resultSet.FindFirstIndex(new MyRepositoryId(1));
			Assert.AreEqual(2, index);
		}

	}

}
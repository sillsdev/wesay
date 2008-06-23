using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class MemoryRepositoryStateUnitializedTests : IRepositoryStateUnitializedTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestItem>();
		}

	}

	[TestFixture]
	public class MemoryRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<TestItem>
	{
		[SetUp]
		protected void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestItem>();
		}

	}

	[TestFixture]
	public class MemoryRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestItem>();
		}

	}

	[TestFixture]
	public class MemoryRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestItem>();
		}

	}
}
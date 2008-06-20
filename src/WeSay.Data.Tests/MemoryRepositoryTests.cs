using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	public class TestClass
	{
		private string String;
		private int Int;

	}

	[TestFixture]
	public class MemoryRepositoryStateUnitializedTests : IRepositoryStateUnitializedTests<TestClass>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestClass>();
		}

	}

	[TestFixture]
	public class MemoryRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<TestClass>
	{
		[SetUp]
		protected void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestClass>();
		}

	}

	[TestFixture]
	public class MemoryRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<TestClass>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestClass>();
		}

	}

	[TestFixture]
	public class MemoryRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<TestClass>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new MemoryRepository<TestClass>();
		}

	}
}
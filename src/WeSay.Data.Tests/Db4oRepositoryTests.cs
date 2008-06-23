using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oRepositoryStateUnitializedTests : IRepositoryStateUnitializedTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>();
		}

	}

	[TestFixture]
	public class Db4oRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<TestItem>
	{
		[SetUp]
		protected void Setup()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>();
		}

	}

	[TestFixture]
	public class Db4oRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>();
		}

	}

	[TestFixture]
	public class Db4oRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>();
		}

	}
}
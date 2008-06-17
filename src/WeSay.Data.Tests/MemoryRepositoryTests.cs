using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Repository.RespositoryTests
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
		public void Setup()
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

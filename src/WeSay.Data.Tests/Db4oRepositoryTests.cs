using System.IO;
using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oRepositoryStateUnitializedTests : IRepositoryStateUnitializedTests<TestItem>
	{
		private string _name;

		[SetUp]
		public void Setup()
		{
			this._name = Path.GetTempFileName();
			RepositoryUnderTest = new Db4oRepository<TestItem>(_name);
		}
		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

	}

	[TestFixture]
	public class Db4oRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public void Setup()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository() {
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}

	[TestFixture]
	public class Db4oRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<TestItem>
	{
		private string _name;
		[SetUp]
		public void Setup()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}

	[TestFixture]
	public class Db4oRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public void Setup()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}

	[TestFixture]
	public class Db4oRepositoryDeleteAllItemsTransitionTests : IRepositoryDeleteAllItemsTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public void Setup()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void RepopulateRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}
}
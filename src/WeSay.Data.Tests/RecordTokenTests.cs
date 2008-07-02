using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class RecordTokenTests
	{
		private MemoryRepository<TestItem> _repository;

		[SetUp]
		public void Setup()
		{
			_repository = new MemoryRepository<TestItem>();
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}

		[Test]
		public void Construct()
		{
			Assert.IsNotNull(new RecordToken<TestItem>(_repository, new TestRepositoryId(8)));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Construct_NullRepository_Throws()
		{
			Assert.IsNotNull(new RecordToken<TestItem>(null, new TestRepositoryId(8)));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Construct_NullRepositoryId_Throws()
		{
			Assert.IsNotNull(new RecordToken<TestItem>(_repository, null));
		}

		[Test]
		public void ConstructWithResults()
		{
			Assert.IsNotNull(
					new RecordToken<TestItem>(_repository,
											  new Dictionary<string, object>(),
											  new TestRepositoryId(8)));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithResults_NullRepository_Throws()
		{
			Assert.IsNotNull(
					new RecordToken<TestItem>(null,
											  new Dictionary<string, object>(),
											  new TestRepositoryId(8)));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithResults_NullResults_Throws()
		{
			Assert.IsNotNull(new RecordToken<TestItem>(_repository, null, new TestRepositoryId(8)));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithResults_NullRepositoryId_Throws()
		{
			Assert.IsNotNull(
					new RecordToken<TestItem>(_repository, new Dictionary<string, object>(), null));
		}
	}

	public class RecordTokenTestsBase
	{
		private RecordToken<TestItem> _token;

		public RecordToken<TestItem> Token
		{
			get
			{
				if(_token == null)
				{
					throw new InvalidOperationException("Token must be initialized before tests can be run");
				}
				return _token;
			}
			set { _token = value; }
		}

		[Test]
		public void GetIndexer_NullFieldName_Throws()
		{
			Token[null] = null;
		}

		[Test]
		public void GetIndexer_AnyFieldName_Null()
		{
			Assert.IsNull(Token["anything"]);
		}

		[Test]
		public void GetIndexer_EmptyName_Null()
		{
			Assert.IsNull(Token[""]);
		}

		[Test]
		public void SetIndexer_AnthingFieldName_Null_KeepsNull()
		{
			Token["anything"] = null;
			Assert.IsNull(Token["anything"]);
		}

		[Test]
		public void SetIndexer_AnthingFieldName_Value_KeepsValue()
		{
			object o = new object();
			Token["anything"] = o;
			Assert.AreSame(o, Token["anything"]);
		}

		[Test]
		public void SetIndexer_EmptyFieldName_Value_KeepsValue()
		{
			object o = new object();
			Token[""] = o;
			Assert.AreSame(o, Token[""]);
		}

		[Test]
		public void Id_AsConstructed()
		{
			Assert.AreEqual(Token.Id, new TestRepositoryId(8));
		}
	}

	[TestFixture]
	public class RecordTokenConstructedWithoutResultsTests: RecordTokenTestsBase
	{
		private MemoryRepository<TestItem> _repository;

		[SetUp]
		public void Setup()
		{
			_repository = new MemoryRepository<TestItem>();
			Token = new RecordToken<TestItem>(_repository, new TestRepositoryId(8));
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}
	}

	[TestFixture]
	public class RecordTokenConstructedWithResultsTests : RecordTokenTestsBase
	{
		private MemoryRepository<TestItem> _repository;

		[SetUp]
		public void Setup()
		{
			_repository = new MemoryRepository<TestItem>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			results["string"] = "result";
			results["int"] = 12;
			Token = new RecordToken<TestItem>(_repository, results, new TestRepositoryId(8));
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}

		[Test]
		public void GetIndexer_ExistingFieldName_ConstructedValue()
		{
			Assert.AreEqual("result", Token["string"]);
			Assert.AreEqual(12, Token["int"]);
		}

		[Test]
		public void SetIndexer_ExistingFieldName_OverwritesValue()
		{
			Token["string"] = "new result";
			Assert.AreEqual("new result", Token["string"]);
		}

	}

	[TestFixture]
	public class RecordTokenUsesRepositoryTests
	{
		private class TestRepository: IRepository<TestItem>
		{
			private readonly TestItem _itemToReturn;

			public TestRepository(TestItem itemToReturn) {
				_itemToReturn = itemToReturn;
			}

			public DateTime LastModified
			{
				get { throw new NotImplementedException(); }
			}

			public TestItem CreateItem()
			{
				throw new NotImplementedException();
			}

			public int CountAllItems()
			{
				throw new NotImplementedException();
			}

			public RepositoryId GetId(TestItem item)
			{
				throw new NotImplementedException();
			}

			public TestItem GetItem(RepositoryId id)
			{
				Assert.AreEqual(id, new TestRepositoryId(8));
				return _itemToReturn;
			}

			public void DeleteItem(TestItem item)
			{
				throw new NotImplementedException();
			}

			public void DeleteItem(RepositoryId id)
			{
				throw new NotImplementedException();
			}

			public RepositoryId[] GetAllItems()
			{
				throw new NotImplementedException();
			}

			public void SaveItem(TestItem item)
			{
				throw new NotImplementedException();
			}

			public bool CanQuery
			{
				get
				{
					return false;
				}
			}

			public bool CanPersist
			{
				get
				{
					return false;
				}
			}

			public void SaveItems(IEnumerable<TestItem> items)
			{
				throw new NotImplementedException();
			}

			public ResultSet<TestItem> GetItemsMatching(Query query)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
			}
		}

		private TestRepository _repository;
		private RecordToken<TestItem> _token;
		private TestItem _testItem;

		[SetUp]
		public void Setup()
		{
			this._testItem = new TestItem();
			_repository = new TestRepository(this._testItem);
			this._token = new RecordToken<TestItem>(this._repository, new TestRepositoryId(8));
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}

		[Test]
		public void RealObject_DelegatesToRepository()
		{
			Assert.AreSame(_testItem, _token.RealObject);
		}
	}
}
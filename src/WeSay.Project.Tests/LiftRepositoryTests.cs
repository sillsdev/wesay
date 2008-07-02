using System.IO;
using NUnit.Framework;
using WeSay.Data.Tests;
using WeSay.LexicalModel;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class LiftRepositoryStateUnitializedTests:IRepositoryStateUnitializedTests<LexEntry>
	{
		private string _persistedFilePath;
		[SetUp]
		public void Setup()
		{
			this._persistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(this._persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			File.Delete(this._persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		[SetUp]
		public void Setup()
		{
			this._persistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(this._persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			File.Delete(this._persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		[SetUp]
		public void Setup()
		{
			this._persistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(this._persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			File.Delete(this._persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		[SetUp]
		public void Setup()
		{
			this._persistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(this._persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			File.Delete(this._persistedFilePath);
		}
	}
}
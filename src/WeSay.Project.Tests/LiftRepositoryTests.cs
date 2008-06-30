using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using WeSay.Data.Tests;
using WeSay.LexicalModel;

namespace WeSay.Project.Tests.Resources
{
	[TestFixture]
	public class LiftRepositoryStateUnitializedTests:IRepositoryStateUnitializedTests<LexEntry>
	{
		[SetUp]
		public void Setup()
		{
			PersistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(PersistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<LexEntry>
	{
		[SetUp]
		public void Setup()
		{
			PersistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(PersistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<LexEntry>
	{
		[SetUp]
		public void Setup()
		{
			PersistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(PersistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<LexEntry>
	{
		[SetUp]
		public void Setup()
		{
			PersistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new LiftRepository(PersistedFilePath);
		}
	}
}

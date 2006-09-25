using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	class InMemoryRecordListManagerTests : RecordListManagerBaseTests
	{
		[SetUp]
		public override void Setup()
		{
			_recordListManager = new InMemoryRecordListManager();
			base.Setup();
		}
	}
}

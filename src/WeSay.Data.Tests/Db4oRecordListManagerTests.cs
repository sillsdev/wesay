using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	class Db4oRecordListManagerTests : RecordListManagerBaseTests
	{
		[SetUp]
		public override void Setup()
		{
			_recordListManager = new Db4oRecordListManager();
			base.Setup();
		}
	}
}

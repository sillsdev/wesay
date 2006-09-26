using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oRecordListManagerTests : RecordListManagerBaseTests
	{
		private string _filePath;

		[SetUp]
		public override void Setup()
		{
			_filePath = System.IO.Path.GetTempFileName();
			base.Setup();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			System.IO.File.Delete(_filePath);
		}

		protected override IRecordListManager CreateRecordListManager()
		{
			return new Db4oRecordListManager(_filePath);
		}

	}
}

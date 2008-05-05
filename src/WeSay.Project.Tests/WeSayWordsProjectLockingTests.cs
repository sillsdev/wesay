using System.IO;
using NUnit.Framework;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class WeSayWordsProjectLockingTests
	{
		private WeSayWordsProject _p;

		[SetUp]
		public void Setup()
		{
			_p = new WeSayWordsProject();
			this._p.PathToLiftFile = Path.GetTempFileName();
		}

		[TearDown]
		public void Teardown()
		{
			if (_p.LiftIsLocked)
			{
				_p.ReleaseLockOnLift();
			}
			File.Delete(this._p.PathToLiftFile);
		}

		[Test]
		public void LiftIsLocked()
		{
			Assert.IsFalse(_p.LiftIsLocked);
			_p.LockLift();
			Assert.IsTrue(_p.LiftIsLocked);
			_p.ReleaseLockOnLift();
			Assert.IsFalse(_p.LiftIsLocked);
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void LockLift()
		{
			_p.LockLift();
			File.Open(_p.PathToLiftFile, FileMode.Open);
		}

		[Test]
		public void ReleaseLockOnLift()
		{
			_p.LockLift();
			_p.ReleaseLockOnLift();
			using (File.Open(_p.PathToLiftFile, FileMode.Open))
			{
			}
		}
	}
}
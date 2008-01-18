using System;
using NUnit.Framework;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	public abstract class TaskBaseTests
	{
		protected ITask _task;

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deactivate_CalledBeforeActivate_Throws()
		{
			_task.Deactivate();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Deactivate_CalledTwice_Throws()
		{
			try
			{
				_task.Activate();
				_task.Deactivate();

			}
			finally
			{
				_task.Deactivate();
			}
			//    _task = null;
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
		}

		[Test]
		public void IsActive_AfterActivate_True()
		{
			_task.Activate();
			Assert.IsTrue(_task.IsActive);
			_task.Deactivate();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Activate_CalledTwice_Throws()
		{

			_task.Activate();
			try
			{
				_task.Activate();
			}
			finally
			{
				_task.Deactivate();
			}
		}



		[Test]
		public void IsActive_BeforeActivate_False()
		{
			Assert.IsFalse(_task.IsActive);
		}

		[Test]
		public void IsActive_AfterDeactivate_False()
		{
			_task.Activate();
			_task.Deactivate();
			Assert.IsFalse(_task.IsActive);
		}
	}
}
using System;
using NUnit.Framework;
using WeSay.UI;

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
			_task.Activate();
			_task.Deactivate();
			_task.Deactivate();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Activate_CalledTwice_Throws()
		{
			_task.Activate();
			_task.Activate();
		}

		[Test]
		public void IsActive_AfterActivate_True()
		{
			_task.Activate();
			Assert.IsTrue(_task.IsActive);
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
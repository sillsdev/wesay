using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Project;

namespace WeSay.App.Tests
{
	public class MockTask : ITask
	{
		private bool _isPinned;
		private string _description;
		private string _label;
		private Control _control;

		private bool _isActive;

		public MockTask(string label, string description, bool isPinned)
		{
			this._label = label;
			this._description = description;
			this._isPinned = isPinned;
			this._control = new Control();
		}

		public Control Control
		{
			get
			{
				return _control;
			}
		}

		public bool IsActive
		{
			get { return this._isActive; }
		}

		public void Activate()
		{
			_isActive = true;
		}

		public void Deactivate()
		{
			_isActive = false;
		}

		public string Label
		{
			get { return this._label; }
		}

		public int ReferenceCount
		{
			get
			{
				return -1;
			}
		}

		public string Description
		{
			get { return this._description; }
		}

		public bool IsPinned
		{
			get { return this._isPinned; }
		}

		public string Status
		{
			get
			{
				return "12";
			}
		}
		public string ExactStatus
		{
			get
			{
				return Status;
			}
		}
		public bool WantsToPreCache
		{
			get { return true; }
		}

	}

	[TestFixture]
	public class TabbedFormTests
	{
		WeSayWordsProject _project;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_project = new WeSayWordsProject();
			_project.StringCatalogSelector = "en";
			_project.LoadFromProjectDirectoryPath(WeSayWordsProject.GetPretendProjectDirectory());

			_project.Tasks = new List<ITask>();
		}

		[SetUp]
		public void Setup()
		{
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));
			_project.Tasks.Add(new MockTask("Add Meanings", "Add meanings to entries that are lacking them.", false));
			_project.Tasks.Add(new MockTask("Semantic Domains", "Add new words using semantic domains.", false));
		}

		[TearDown]
		public void TearDown()
		{
			_project.Tasks.Clear();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			_project.Dispose();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializeTasks_NullTaskList_Throws()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(null);
		}

		[Test]
		public void GetCurrentWorkTask_TasksNotInitialized_IsNull()
		{
			TabbedForm tabbedForm = new TabbedForm();
			Assert.IsNull(tabbedForm.CurrentWorkTask);
		}


		[Test]
		public void GetCurrentWorkTask_NoTasks_IsNull()
		{
			_project.Tasks.Clear();
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsNull(tabbedForm.CurrentWorkTask);
		}

		[Test]
		public void GetCurrentWorkTask_OnlyPinnedTasks_IsNull()
		{
			_project.Tasks.Clear();
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));

			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsNull(tabbedForm.CurrentWorkTask);
		}

		[Test]
		public void ActiveTask_OnlyPinnedTasks_IsFirstTask()
		{
			_project.Tasks.Clear();
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));

			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsTrue(((MockTask)_project.Tasks[0]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[1]).IsActive);
		}

		[Test]
		public void GetCurrentWorkTask_RegularSetOfTasks_IsFirstNonPinned()
		{
			ClearCurrentWorkTask();
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			Assert.IsNotNull(tabbedForm.CurrentWorkTask);
			Assert.AreEqual("Add Meanings", tabbedForm.CurrentWorkTask.Label);
		}

		private static void ClearCurrentWorkTask()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.LastCurrentWorkTaskLabel = string.Empty;
		}


		[Test]
		public void GetCurrentWorkTask_RemembersLastCurrentWorkTask()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			tabbedForm.ActiveTask = _project.Tasks[3];

			tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.AreEqual("Semantic Domains", tabbedForm.CurrentWorkTask.Label);
		}

		[Test]
		public void ActiveTask_RegularSetOfTasks_IsFirstTask()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			Assert.IsTrue(((MockTask)_project.Tasks[0]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[1]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[2]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[3]).IsActive);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetActiveTask_Null_Throws()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			tabbedForm.ActiveTask = null;
		}

		[Test]
		public void SetActiveTask_TaskIsPinned_CurrentWorkTaskNoChange()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			ITask initialWorkTask = tabbedForm.CurrentWorkTask;
			tabbedForm.ActiveTask = _project.Tasks[0];
			Assert.AreSame(initialWorkTask, tabbedForm.CurrentWorkTask);
		}

		[Test]
		public void SetActiveTask_ToPinnedTask_AnotherTaskActivated_DeactivateOtherAndActivateNew()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsTrue(((MockTask)_project.Tasks[0]).IsActive);

			tabbedForm.ActiveTask = _project.Tasks[1];
			Assert.IsFalse(((MockTask)_project.Tasks[0]).IsActive);
			Assert.IsTrue(((MockTask)_project.Tasks[1]).IsActive);
		}

		[Test]
		public void SetActiveTask_ToWorkTask_AnotherTaskActivated_DeactivateOtherAndActivateNew_ChangeLabel()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsTrue(((MockTask)_project.Tasks[0]).IsActive);

			tabbedForm.ActiveTask = _project.Tasks[3];
			Assert.IsFalse(((MockTask)_project.Tasks[0]).IsActive);
			Assert.IsTrue(((MockTask)_project.Tasks[3]).IsActive);
			Assert.AreEqual(_project.Tasks[3].Label, tabbedForm.TabLabels[2]);
		}

		[Test]
		public void SetActiveTask_ToWorkTask_AnotherWorkTaskActivated_DeactivateOtherAndActivateNew()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			tabbedForm.ActiveTask = _project.Tasks[2];
			tabbedForm.ActiveTask = _project.Tasks[3];
			Assert.IsTrue(((MockTask)_project.Tasks[3]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[2]).IsActive);

			Assert.AreEqual(_project.Tasks[3].Label, tabbedForm.TabLabels[2]);
		}

	}
}

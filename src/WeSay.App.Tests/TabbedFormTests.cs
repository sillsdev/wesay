using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.UI;

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
		public void ActiveTask_OnlyPinnedTasks_LastPinnedTask()
		{
			_project.Tasks.Clear();
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));

			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsTrue(((MockTask)_project.Tasks[1]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[0]).IsActive);
		}

		[Test]
		public void GetCurrentWorkTask_RegularSetOfTasks_IsFirstNonPinned()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			Assert.IsNotNull(tabbedForm.CurrentWorkTask);
			Assert.AreEqual("Add Meanings", tabbedForm.CurrentWorkTask.Label);
			Assert.IsTrue(((MockTask) _project.Tasks[2]).IsActive);
		}

		[Test]
		public void ActiveTask_RegularSetOfTasks_IsFirstNonPinned()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			Assert.IsTrue(((MockTask)_project.Tasks[2]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[0]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[1]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[3]).IsActive);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetCurrentWorkTask_Null_Throws()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			tabbedForm.CurrentWorkTask = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SetCurrentWorkTask_TaskIsPinned_Throws()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			tabbedForm.CurrentWorkTask = _project.Tasks[0];
		}

		[Test]
		public void SetCurrentWorkTask_AnotherTaskActivated_DeactivateOtherAndActivateNew()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			tabbedForm.ActiveTask = _project.Tasks[0];
			tabbedForm.CurrentWorkTask = _project.Tasks[3];
			Assert.IsTrue(((MockTask)_project.Tasks[3]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[2]).IsActive);
			Assert.AreEqual(_project.Tasks[3].Label, tabbedForm.TabLabels[2]);
		}

		[Test]
		public void SetCurrentWorkTask_AnotherWorkTaskActivated_DeactivateOtherAndActivateNew()
		{
			TabbedForm tabbedForm = new TabbedForm();
			tabbedForm.InitializeTasks(_project.Tasks);

			tabbedForm.CurrentWorkTask = _project.Tasks[3];
			Assert.IsTrue(((MockTask)_project.Tasks[3]).IsActive);
			Assert.IsFalse(((MockTask)_project.Tasks[2]).IsActive);

			Assert.AreEqual(_project.Tasks[3].Label, tabbedForm.TabLabels[2]);
		}

	}
}

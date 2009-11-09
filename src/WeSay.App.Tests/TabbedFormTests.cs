using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using Palaso.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.App.Tests
{
	public class MockDictionaryTask : MockTask, ITaskForExternalNavigateToEntry
	{
		 public MockDictionaryTask(string label, string description, bool isPinned)
			 :base(label,description,isPinned)
		 {

		 }

		public void NavigateToEntry(string url)
		{

		}

		public string CurrentUrl
		{
			get { return string.Empty; }
		}
	}

	public class MockTask: ITask
	{
		private readonly bool _isPinned;
		private readonly string _description;
		private readonly string _label;
		private readonly Control _control;

		private bool _isActive;
		public string _urlThatItWasToldToGoTo;

		public MockTask(string label, string description, bool isPinned)
		{
			_label = label;
			_description = description;
			_isPinned = isPinned;
			_control = new Control();
		}

		public bool Available
		{
			get { return true; }
		}

		public Control Control
		{
			get { return _control; }
		}

		public bool IsActive
		{
			get { return _isActive; }
		}

		public void Activate()
		{
			_isActive = true;
		}

		public void Deactivate()
		{
			_isActive = false;
		}

		#region ITask Members

		public void GoToUrl(string url)
		{
			_urlThatItWasToldToGoTo = url;
		}

		#endregion

		public string Label
		{
			get { return _label; }
		}

		public int GetReferenceCount()
		{
			return -1;
		}

		public string Description
		{
			get { return _description; }
		}

		public bool IsPinned
		{
			get { return _isPinned; }
		}

		public int GetRemainingCount()
		{
			return 12;
		}

		public bool AreCountsRelevant()
		{
			return false;
		}

		public string GetRemainingCountText()
		{
			return "Remaining: " + GetRemainingCount();
		}

		public string GetReferenceCountText()
		{
			return "Reference: " + GetReferenceCount();
		}

		public int ExactCount
		{
			get { return GetRemainingCount(); }
		}


		#region IThingOnDashboard Members

		public string GroupName
		{
			get { throw new NotImplementedException(); }
		}

		public DashboardGroup Group
		{
			get { throw new NotImplementedException(); }
		}

		public string LocalizedLabel
		{
			get { throw new NotImplementedException(); }
		}

		public string LocalizedLongLabel
		{
			get { throw new NotImplementedException(); }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { throw new NotImplementedException(); }
		}

		public Image DashboardButtonImage
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	[TestFixture]
	public class TabbedFormTests
	{
		private WeSayWordsProject _project;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_project = new WeSayWordsProject();
			_project.UiOptions.Language = "en";
			_project.LoadFromProjectDirectoryPath(BasilProject.GetPretendProjectDirectory());

			_project.Tasks = new List<ITask>();
		}

		[SetUp]
		public void Setup()
		{
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));
			_project.Tasks.Add(new MockTask("Add Meanings",
											"Add meanings to entries that are lacking them.",
											false));
			_project.Tasks.Add(new MockTask("Semantic Domains",
											"Add new words using semantic domains.",
											false));
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
		[ExpectedException(typeof (ArgumentNullException))]
		public void InitializeTasks_NullTaskList_Throws()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(null);
			}
		}

		[Test]
		public void GetCurrentWorkTask_TasksNotInitialized_IsNull()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				Assert.IsNull(tabbedForm.CurrentWorkTask);
			}
		}

		[Test]
		public void GetCurrentWorkTask_NoTasks_IsNull()
		{
			_project.Tasks.Clear();
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);
				Assert.IsNull(tabbedForm.CurrentWorkTask);
			}
		}

		[Test]
		public void GetCurrentWorkTask_OnlyPinnedTasks_IsNull()
		{
			_project.Tasks.Clear();
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));

			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);
				Assert.IsNull(tabbedForm.CurrentWorkTask);
			}
		}

		[Test]
		public void ActiveTask_OnlyPinnedTasks_IsFirstTask()
		{
			_project.Tasks.Clear();
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));
			_project.Tasks.Add(new MockTask("Words", "The whole lexicon.", true));

			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);
				Assert.IsTrue(((MockTask) _project.Tasks[0]).IsActive);
				Assert.IsFalse(((MockTask) _project.Tasks[1]).IsActive);
			}
		}

		[Test]
		public void GetCurrentWorkTask_RegularSetOfTasks_IsFirstNonPinned()
		{
			ClearCurrentWorkTask();
			using (TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);

				Assert.IsNotNull(tabbedForm.CurrentWorkTask);
				Assert.AreEqual("Add Meanings", tabbedForm.CurrentWorkTask.Label);
			}
		}

		private static void ClearCurrentWorkTask()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.LastCurrentWorkTaskLabel = string.Empty;
			}
		}

		[Test]
		public void GetCurrentWorkTask_RemembersLastCurrentWorkTask()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);
				tabbedForm.ActiveTask = _project.Tasks[3];
			}
			using(var x = new TabbedForm(new NullStatusBarController()))
			{
				x.InitializeTasks(_project.Tasks);
				Assert.AreEqual("Semantic Domains", x.CurrentWorkTask.Label);
			}
		}

		[Test]
		public void ActiveTask_RegularSetOfTasks_IsFirstTask()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);

				Assert.IsTrue(((MockTask) _project.Tasks[0]).IsActive);
				Assert.IsFalse(((MockTask) _project.Tasks[1]).IsActive);
				Assert.IsFalse(((MockTask) _project.Tasks[2]).IsActive);
				Assert.IsFalse(((MockTask) _project.Tasks[3]).IsActive);
			}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetActiveTask_Null_Throws()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);

				tabbedForm.ActiveTask = null;
			}
		}

		[Test]
		public void SetActiveTask_TaskIsPinned_CurrentWorkTaskNoChange()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);
				ITask initialWorkTask = tabbedForm.CurrentWorkTask;
				tabbedForm.ActiveTask = _project.Tasks[0];
				Assert.AreSame(initialWorkTask, tabbedForm.CurrentWorkTask);
			}
		}

		private void EnsureCreated(Form form)
		{
			form.SuspendLayout();
			form.ShowInTaskbar = false;
			form.ShowIcon = false;
			form.Show();
			form.Hide();
			form.ResumeLayout(false);
		}

		[Test]
		public void SetActiveTask_ToPinnedTask_AnotherTaskActivated_DeactivateOtherAndActivateNew()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				EnsureCreated(tabbedForm);
				tabbedForm.InitializeTasks(_project.Tasks);
				Assert.IsTrue(((MockTask) _project.Tasks[0]).IsActive, "1");

				tabbedForm.ActiveTask = _project.Tasks[1];
				Assert.IsFalse(((MockTask) _project.Tasks[0]).IsActive, "2");
				Assert.IsTrue(((MockTask) _project.Tasks[1]).IsActive, "3");
			}
		}

		[Test]
		public void
				SetActiveTask_ToWorkTask_AnotherTaskActivated_DeactivateOtherAndActivateNew_ChangeLabel
				()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
			EnsureCreated(tabbedForm);
			tabbedForm.InitializeTasks(_project.Tasks);
			Assert.IsTrue(((MockTask)_project.Tasks[0]).IsActive, "1");

			tabbedForm.ActiveTask = _project.Tasks[3];
			Assert.IsFalse(((MockTask) _project.Tasks[0]).IsActive, "2");
			Assert.IsTrue(((MockTask) _project.Tasks[3]).IsActive, "3");
			Assert.AreEqual(_project.Tasks[3].Label, tabbedForm.TabLabels[2], "4");

			}
		}

		[Test]
		public void SetActiveTask_ToWorkTask_AnotherWorkTaskActivated_DeactivateOtherAndActivateNew()
		{
			using(TabbedForm tabbedForm = new TabbedForm(new NullStatusBarController()))
			{
				tabbedForm.InitializeTasks(_project.Tasks);

				tabbedForm.ActiveTask = _project.Tasks[2];
				tabbedForm.ActiveTask = _project.Tasks[3];
				Assert.IsTrue(((MockTask) _project.Tasks[3]).IsActive);
				Assert.IsFalse(((MockTask) _project.Tasks[2]).IsActive);

				Assert.AreEqual(_project.Tasks[3].Label, tabbedForm.TabLabels[2]);
			}
		}
	}
}
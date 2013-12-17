using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.App.Properties;
using WeSay.Project;
using Timer=System.Windows.Forms.Timer;

namespace WeSay.App
{
	public partial class TabbedForm: Form, ICurrentWorkTask
	{
		private ITask _activeTask;
		private TabPage _currentWorkTab;
		private string _currentUrl;
		public SynchronizationContext synchronizationContext;
		//        private ProgressDialogHandler _progressHandler;

		public TabbedForm()
		{
			InitializeComponent();
			tabControl1.TabPages.Clear();
			tabControl1.Selected += OnTabSelected;

			synchronizationContext = SynchronizationContext.Current;
			Debug.Assert(synchronizationContext != null);
		}

		public void InitializeTasks(IList<ITask> taskList)
		{
			if (taskList == null)
			{
				throw new ArgumentNullException("taskList");
			}
			foreach (ITask t in taskList)
			{
				if (t.IsPinned)
				{
					CreateTabPageForTask(t);
				}
			}

			InitializeCurrentWorkTaskFromLastSession(taskList);
			if (CurrentWorkTask == null)
			{
				InitializeCurrentWorkTaskToFirstNonPinnedTask(taskList);
			}

			if (taskList.Count > 0)
			{
				ActiveTask = taskList[0];
			}
		}

		private void InitializeCurrentWorkTaskToFirstNonPinnedTask(IEnumerable<ITask> taskList)
		{
			// use first non-pinned task.
			foreach (ITask task in taskList)
			{
				if (!task.IsPinned)
				{
					SetCurrentWorkTask(task);
					break;
				}
			}
		}

		private void InitializeCurrentWorkTaskFromLastSession(IEnumerable<ITask> taskList)
		{
			foreach (ITask task in taskList)
			{
				if (LastCurrentWorkTaskLabel == task.Label)
				{
					SetCurrentWorkTask(task);
					break;
				}
			}
		}

		private void CreateTabPageForTask(ITask t)
		{
			//t.Container = container;
			TabPage page = new TabPage(t.Label);
			page.Tag = t;

			//this is trying to get around screwing up spacing when the ui font
			//is a huge one
			page.Font = new Font(FontFamily.GenericSansSerif, 9);

			tabControl1.TabPages.Add(page);
		}

		private delegate void TakesStringArg(string arg);

		public void GoToUrl(string url)
		{
			if (InvokeRequired)
			{
				Invoke(new TakesStringArg(GoToUrl), url);
				return;
			}
			//todo: find the task in the url, pick the right task,
			//handle the case where we don't have that task, etc.

			foreach (TabPage page in tabControl1.TabPages)
			{
				//todo: temporary hack
				if (((ITask) page.Tag).Label.Contains("Dictionary"))
				{
					//this approach is for user clicking, chokes without an event loop: ActiveTask = (ITask) page.Tag;
					tabControl1.SelectedTab = page;
					ActivateTab(page, false);
					ActiveTask.GoToUrl(url);
					CurrentUrl = url;
					return;
				}
			}
			ErrorReport.ReportNonFatalMessage(
					"Sorry, that URL requires a task which is not currently enabled for this user. ({0})",
					url);
			throw new NavigationException("Couldn't locate ");
		}

		public void MakeFrontMostWindow()
		{
			if (WindowState == FormWindowState.Minimized)
			{
				WindowState = FormWindowState.Normal;
			}

			BringToFront();
			Activate(); // may only flash the icon on the taskbar
		}

		public ITask ActiveTask
		{
			get { return _activeTask; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				TabPage tabPageToActivate = null;
				if (value.IsPinned)
				{
					foreach (TabPage page in tabControl1.TabPages)
					{
						if (page.Tag == value)
						{
							tabPageToActivate = page;
							break;
						}
					}
				}
				else
				{
					SetCurrentWorkTask(value);
					tabPageToActivate = _currentWorkTab;
				}

				if (tabPageToActivate != null)
				{
					if (tabControl1.SelectedTab != tabPageToActivate)
					{
						tabControl1.SelectedTab = tabPageToActivate;
					}
					else
					{
						ActivateTab(tabPageToActivate, true);
					}
				}
			}
		}

		private void SetCurrentWorkTask(ITask value)
		{
			if (_currentWorkTab == null)
			{
				_currentWorkTab = new TabPage();
				tabControl1.TabPages.Add(_currentWorkTab);
			}
			_currentWorkTab.Tag = value;
			_currentWorkTab.Text = value.Label;
			LastCurrentWorkTaskLabel = value.Label;
		}

		public ITask CurrentWorkTask
		{
			get
			{
				if (_currentWorkTab == null)
				{
					return null;
				}
				return (ITask) _currentWorkTab.Tag;
			}
		}

		public string LastCurrentWorkTaskLabel
		{
			get { return Settings.Default.CurrentWorkTask; }
			set { Settings.Default.CurrentWorkTask = value; }
		}

		public IList<string> TabLabels
		{
			get
			{
				IList<string> labels = new List<string>();
				foreach (TabPage page in tabControl1.TabPages)
				{
					labels.Add(page.Text);
				}
				return labels;
			}
		}

		public string CurrentUrl
		{
			get { return _currentUrl; }
			set { _currentUrl = value; }
		}

		private void OnTabSelected(object sender, TabControlEventArgs e)
		{
			if (e.Action == TabControlAction.Selected)
			{
				ActivateTab(e.TabPage, true);
			}
		}

		private void ActivateTab(Control page, bool okTouseTimer)
		{
			ITask task = (ITask) page.Tag;
			if (ActiveTask == task)
			{
				return; //debounce
			}

			if (ActiveTask != null)
			{
				ActiveTask.Deactivate();
				_activeTask = null;
			}
			if (task != null)
			{
				page.Cursor = Cursors.WaitCursor;
				page.Controls.Clear();
				if (Visible && okTouseTimer)
				{
					ActivateAfterScreenDraw(page, task);
				}
				else
				{
					ActivateTask(page, task);
				}
			}
		}

		private void ActivateAfterScreenDraw(Control page, ITask task)
		{
			page.Text += " " +
						 StringCatalog.Get("~Loading...",
										   "Appended to the name of a task, in its tab, while the user is waiting for the task to come up.");
			Timer t = new Timer();
			t.Tick += delegate
					  {
						  t.Stop();
						  ActivateTask(page, task);
						  page.Text = task.Label;
						  t.Dispose();
					  };
			t.Interval = 1;
			t.Start();
		}

		private void ActivateTask(Control page, ITask task)
		{
			Logger.WriteEvent("Activating " + page.Text); //enhance: get in English always
			if (ActiveTask == task)
			{
				return;
			}
			try
			{
				task.Activate();
			}
			catch (ConfigurationException e) //let others go through the normal bug reporting system
			{
				ErrorReport.ReportNonFatalMessage(e.Message);
				Logger.WriteEvent("Failed Activating");
				return;
			}

			// RunCommand(new ActivateTaskCommand(page, task));
			task.Control.Dock = DockStyle.Fill;
			page.Controls.Add(task.Control);
			task.Control.SelectNextControl(task.Control, true, true, true, true);
			task.Control.PerformLayout();
			task.Control.Invalidate(true);
			page.Cursor = Cursors.Default;
			_activeTask = task;
			Logger.WriteEvent("Done Activating");
		}

		public event EventHandler IntializationComplete;

		public void ContinueLaunchingAfterInitialDisplay()
		{
			Timer t = new Timer();
			t.Tick += delegate
					  {
						  t.Stop();
						  InitializeTasks(WeSayWordsProject.Project.Tasks);
						  if (IntializationComplete != null)
						  {
							  IntializationComplete.Invoke(this, null);
						  }
						  t.Dispose();
					  };
			t.Interval = 1;
			t.Start();
		}
	}
}
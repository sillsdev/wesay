using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Chorus.UI.Review;
using Palaso.Code;
using Palaso.i18n;
using Palaso.Reporting;
using WeSay.App.Properties;
using WeSay.Project;
using WeSay.UI;

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

		[CLSCompliant(false)]
		public TabbedForm(
			StatusBarController statusBarController,
			NavigateToRecordEvent navigateToRecordEventToSubscribeTo)
		{
			InitializeComponent();
			_helpProvider.RegisterPrimaryHelpFileMapping("wesay.helpmap");
			_helpProvider.RegisterSecondaryHelpMapping("chorus.helpmap");

			tabControl1.TabPages.Clear();
			tabControl1.Selected += OnTabSelected;

			synchronizationContext = SynchronizationContext.Current;
			Debug.Assert(synchronizationContext != null);

			_statusStrip.Font = (System.Drawing.Font)StringCatalog.LabelFont.Clone();
			statusBarController.StatusStrip = _statusStrip;
			if (navigateToRecordEventToSubscribeTo != null)
			{
				navigateToRecordEventToSubscribeTo.Subscribe(OnNavigateToUrl);
			}
		}

		//for tests
		public TabbedForm(StatusBarController statusBarController)
			:this(statusBarController, null)
		{

		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
				return cp;
			}
		}

		public StatusStrip StatusStrip
		{
			get { return _statusStrip; }
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
				SetActiveTask(taskList[0]);
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

		private void OnNavigateToUrl(string url)
		{
			GoToUrl(url);
		}


		private delegate void TakesStringArg(string arg);

		public void GoToUrl(string url)
		{
			if (InvokeRequired)
			{
				Invoke(new TakesStringArg(GoToUrl), url);
				return;
			}

			//NB: notice, we only handly URLs to a single task at this point (DictionaryBrowseAndEdit)

			foreach (TabPage page in tabControl1.TabPages)
			{
				if (page.Tag is ITaskForExternalNavigateToEntry)
				{
					tabControl1.SelectedTab = page;
#if __MonoCS__    //For some reason .net fires this event if TabPages.Clear has been used. Mono does not.
					if(!tabControl1.IsHandleCreated)
					{
						OnTabSelected(tabControl1, new TabControlEventArgs (tabControl1.SelectedTab, tabControl1.SelectedIndex, TabControlAction.Selected));
					}
#endif

					((ITaskForExternalNavigateToEntry)page.Tag).GoToUrl(url);
					return;
				}
			}
			ErrorReport.NotifyUserOfProblem(
					"Sorry, that URL requires a task which is not currently enabled for this user. ({0})",
					url);
			throw new NavigationException("Couldn't locate ");
		}


		private void CreateTabPageForTask(ITask t)
		{
			//t.Container = container;
			var page = new TabPage(t.Label);
			page.Tag = t;

			//this is trying to get around screwing up spacing when the ui font
			//is a huge one...
			//JH sep09: doesn't seem to have any effect, at least on windows
			page.Font = StringCatalog.LabelFont;

			//jh experiment
			tabControl1.Font = page.Font;


			tabControl1.TabPages.Add(page);
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
		}
		public void SetActiveTask(ITask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException();
			}
			TabPage tabPageToActivate = null;
			if (task.IsPinned)
			{
				foreach (TabPage page in tabControl1.TabPages)
				{
					if (page.Tag == task)
					{
						tabPageToActivate = page;
						break;
					}
				}
			}
			else
			{
				SetCurrentWorkTask(task);
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
			get
			{
				if(_activeTask==null)
					return string.Empty;
				var t = _activeTask as ITaskForExternalNavigateToEntry;
				if(t==null)
					return string.Empty;
				return t.CurrentUrl;
			}
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
			var task = (ITask) page.Tag;
			if (ActiveTask == task)
			{
				return; //debounce
			}

			if (ActiveTask != null)
			{
				ActiveTask.Deactivate();
				_activeTask =null;
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
				UsageReporter.SendNavigationNotice(task.Label);
			}
		}

		private void ActivateAfterScreenDraw(Control page, ITask task)
		{
			page.Text += " " +
						 StringCatalog.Get("~Loading...",
										   "Appended to the name of a task, in its tab, while the user is waiting for the task to come up.");
			var t = new Timer();
			t.Tick += delegate
					  {
						  t.Stop();
						  page.Text = task.Label;
						  ActivateTask(page, task);
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
			catch (ConfigurationException e) //let others go through the normal reporting system
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
				Logger.WriteEvent("Failed Activating");
				return;
			}

			task.Control.Dock = DockStyle.Fill;

			if(task.Control.GetType() == typeof(Chorus.UI.Notes.Browser.NotesBrowserPage))
			{
				page.Controls.Add(task.Control);
			}
			else //I (JH) don't know what problem this code was intended to solve, but it prevents the notes browser from docking properly
			{
				// Prevent partial scrollbars and the like from displaying before the page's Control actually lays itself out below.
				// Suspending layout of the topmost Control works fine on Windows, but not for Linux/Mono.  But suspending/resuming
				// all the way down should be okay on Windows even if it's overkill.
				PreventLayout(task.Control);
				page.Controls.Add(task.Control);
				AllowLayout(task.Control);
			}
			task.Control.SelectNextControl(task.Control, true, true, true, true);

			// The .FocusDesiredControl() gives the task specific control over which child field gets focus when the task is activated.  If every task implemented this method, then the above task.Control.SelectNextControl would be unnecessary
			task.FocusDesiredControl();
			task.Control.PerformLayout();
			task.Control.Invalidate(true);

			page.Cursor = Cursors.Default;
			_activeTask = task;
			Logger.WriteEvent("Done Activating");
		}

		/// <summary>
		/// Suspend layout for the given control, plus its children, plus their children, plus ...
		/// </summary>
		private void PreventLayout(Control control)
		{
			foreach (Control c in control.Controls)
				PreventLayout(c);
			control.SuspendLayout();
		}

		/// <summary>
		/// Resume layout (but don't force it) for the given control, plus its children, plus their children, plus ...
		/// </summary>
		private void AllowLayout(Control control)
		{
			foreach (Control c in control.Controls)
				AllowLayout(c);
			control.ResumeLayout(false);
		}

		public event EventHandler IntializationComplete;

		public void ContinueLaunchingAfterInitialDisplay()
		{
			var t = new Timer();
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

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			//if (e.KeyCode == Keys.F1) //help is now handled by the HelpProvider

		}
	}


}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using WeSay.Foundation;
using WeSay.Project;
using System.Windows.Forms;
using Timer=System.Windows.Forms.Timer;

namespace WeSay.App
{
	public partial class TabbedForm : Form, ICurrentWorkTask
	{
		private ITask _activeTask;
		private TabPage _currentWorkTab;
		private string _currentUrl;
		public SynchronizationContext synchronizationContext;
//        private ProgressDialogHandler _progressHandler;

		public TabbedForm()
		{
			InitializeComponent();
			this.tabControl1.TabPages.Clear();
			this.tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);
			this.tabControl1.Font = StringCatalog.ModifyFontForLocalization(tabControl1.Font);

			synchronizationContext =  WindowsFormsSynchronizationContext.Current;
			Debug.Assert(synchronizationContext != null);
		}

		public void InitializeTasks(IList<ITask> taskList)
		{
			if(taskList == null)
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

			if(taskList.Count > 0){
				ActiveTask = taskList[0];
			}
		}

		private void InitializeCurrentWorkTaskToFirstNonPinnedTask(IList<ITask> taskList) {
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

		private void InitializeCurrentWorkTaskFromLastSession(IList<ITask> taskList) {
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
			page.Font = new Font(FontFamily.GenericSansSerif,9);



			this.tabControl1.TabPages.Add(page);
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


			foreach (TabPage page in this.tabControl1.TabPages)
			{
				//todo: temporary hack
				if (((ITask)page.Tag).Label.Contains("Dictionary"))
				{
					//this approach is for user clicking, chokes without an event loop: ActiveTask = (ITask) page.Tag;
					this.tabControl1.SelectedTab = page;
					ActivateTab(page, false);
					ActiveTask.GoToUrl(url);
					CurrentUrl = url;
					return;
				}
			}
			Palaso.Reporting.ErrorReport.ReportNonFatalMessage("Sorry, that URL requires a task which is not currently enabled for this user. ({0})",url);
			throw new NavigationException("Couldn't locate ");
		}

		public void MakeFrontMostWindow()
		{
			if(this.WindowState == FormWindowState.Minimized)
			{
				this.WindowState = FormWindowState.Normal;
			}

			BringToFront();
			Activate(); // may only flash the icon on the taskbar
		}
		public ITask ActiveTask
		{
			get
			{
				return _activeTask;
			}
			set
			{
				if(value == null)
				{
					throw new ArgumentNullException();
				}
				TabPage tabPageToActivate = null;
				if (value.IsPinned)
				{
					foreach (TabPage page in this.tabControl1.TabPages)
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
					tabPageToActivate = this._currentWorkTab;
				}

				if (tabPageToActivate != null)
				{
					if (this.tabControl1.SelectedTab != tabPageToActivate)
					{
						this.tabControl1.SelectedTab = tabPageToActivate;
					}
					else
					{
						ActivateTab(tabPageToActivate,true);
					}
				}

			}
		}

		private void SetCurrentWorkTask(ITask value) {
			if (this._currentWorkTab == null)
			{
				this._currentWorkTab = new TabPage();
				this.tabControl1.TabPages.Add(this._currentWorkTab);
			}
			this._currentWorkTab.Tag = value;
			this._currentWorkTab.Text = value.Label;
			LastCurrentWorkTaskLabel = value.Label;
		}

		public ITask CurrentWorkTask
		{
			get
			{
				if (this._currentWorkTab == null)
				{
					return null;
				}
				return (ITask)this._currentWorkTab.Tag;
			}
		}

		public string LastCurrentWorkTaskLabel
		{
			get
			{
				return WeSay.App.Properties.Settings.Default.CurrentWorkTask;
			}
			set
			{
				WeSay.App.Properties.Settings.Default.CurrentWorkTask = value;
			}
		}

		public IList<string> TabLabels
		{
			get
			{
				IList<string> labels = new List<string>();
				foreach (TabPage page in this.tabControl1.TabPages)
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

		void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			TabPage page = ((TabControl)sender).SelectedTab;
			ActivateTab(page, true);
		}


		private void ActivateTab(TabPage page, bool okTouseTimer)
		{
			ITask task = (ITask)page.Tag;
			if (ActiveTask == task)
				return; //debounce

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

		private void ActivateAfterScreenDraw(TabPage page, ITask task)
		{
			page.Text += " " +StringCatalog.Get("~Loading...", "Appended to the name of a task, in its tab, while the user is waiting for the task to come up.");
			Timer t = new Timer();
			t.Tick+=new EventHandler(delegate
									 {
											   t.Stop();
											   ActivateTask(page, task);
											   page.Text = task.Label;
											   t.Dispose();
									 });
			t.Interval = 1;
			t.Start();
		}

		private void ActivateTask(TabPage page, ITask task)
		{
			Palaso.Reporting.Logger.WriteEvent("Activating " + page.Text);//enhance: get in English always
			if (ActiveTask == task)
			{
				return;
			}
			try
			{
				task.Activate();
			}
			catch (Palaso.Reporting.ConfigurationException e) //let others go through the normal bug reporting system
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(e.Message);
				Palaso.Reporting.Logger.WriteEvent("Failed Activating");
				return;
			}

			// RunCommand(new ActivateTaskCommand(page, task));
			task.Control.Dock = DockStyle.Fill;
			page.Controls.Add(task.Control);
			task.Control.Focus();
			page.Cursor = Cursors.Default;
			_activeTask = task;
			Palaso.Reporting.Logger.WriteEvent("Done Activating");
		}

//        private void RunCommand(BasicCommand command)
//        {
//            _progressHandler = new ProgressDialogHandler(this, command);
//            _progressHandler.Finished += new EventHandler(_progressHandler_Finished);
//            ProgressState progress = new ProgressState(_progressHandler);
//            // UpdateEnabledStates();
//            command.BeginInvoke(progress);
//        }
//
//        void _progressHandler_Finished(object sender, EventArgs e)
//        {
//            if (InvokeRequired)
//            {
//                BeginInvoke(new MethodInvoker(FinishButOnCorrectThread));
//            }
//            else
//            {
//                FinishButOnCorrectThread();
//            }
//        }
//
//        private void FinishButOnCorrectThread()
//        {
//            _progressHandler = null;
//            _activeTask.Control.Dock = DockStyle.Fill;
//            this.tabControl1.SelectedTab.Controls.Add(_activeTask.Control);
//            this.tabControl1.SelectedTab.Cursor = Cursors.Default;
//        }

		public event System.EventHandler  IntializationComplete;

		public void ContinueLaunchingAfterInitialDisplay()
		{
			Timer t = new Timer();
			t.Tick += new EventHandler(delegate
									   {
										   t.Stop();
										   InitializeTasks(WeSayWordsProject.Project.Tasks);
										   if (IntializationComplete != null)
										   {
											   IntializationComplete.Invoke(this, null);
										   }
										   t.Dispose();
									   });
			t.Interval = 1;
			t.Start();
		}

		private void TabbedForm_Load(object sender, EventArgs e)
		{

		}
	}

//    public class ActivateTaskCommand : BasicCommand
//    {
//        private readonly TabPage _page;
//        private readonly ITask _task;
//        protected WeSay.Foundation.Progress.ProgressState _progress;
//
//        public ActivateTaskCommand(TabPage page, ITask task)
//        {
//            _page = page;
//            _task = task;
//        }
//
//        protected override void DoWork2(ProgressState progress)
//        {
//            _progress = progress;
//            _task.Activate();
//
//        }
//
//        protected override void DoWork(InitializeProgressCallback initializeCallback, ProgressCallback progressCallback,
//                                       StatusCallback primaryStatusTextCallback,
//                                       StatusCallback secondaryStatusTextCallback)
//        {
//            throw new NotImplementedException();
//        }
//    }
}

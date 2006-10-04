using System;
using System.Collections.Generic;
using WeSay.UI;
using System.Windows.Forms;

namespace WeSay.App
{
	public partial class TabbedForm : Form, ICurrentWorkTask
	{
		private ITask _activeTask;
		private TabPage _currentWorkTab;

		public TabbedForm()
		{
			InitializeComponent();
			this.tabControl1.TabPages.Clear();
			this.tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);
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
			this.tabControl1.TabPages.Add(page);
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
					ActivateTab(tabPageToActivate);
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

		void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			TabPage page = ((TabControl)sender).SelectedTab;
			ActivateTab(page);
		}

		private void ActivateTab(TabPage page)
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
				if (Visible)
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
			page.Text += " Loading...";
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
			if (ActiveTask == task)
			{
				return;
			}
			task.Activate();
			task.Control.Dock = DockStyle.Fill;
			page.Controls.Add(task.Control);
			page.Cursor = Cursors.Default;
			_activeTask = task;
		}


		private void ContinueLaunchingAfterInitialDisplay()
		{
			Timer t = new Timer();
			t.Tick += new EventHandler(delegate
									   {
										   t.Stop();
										   InitializeTasks(WeSayWordsProject.Project.Tasks);
										   t.Dispose();
									   });
			t.Interval = 1;
			t.Start();
		}

		private void TabbedForm_Load(object sender, EventArgs e)
		{
			ContinueLaunchingAfterInitialDisplay();
		}
	}
}

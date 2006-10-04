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

			this.tabControl1.SelectedIndexChanged += new System.EventHandler(tabControl1_SelectedIndexChanged);
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
				this.tabControl1.SelectedIndex = 0;
				ActiveTask = taskList[0];
			}
		}

		private void InitializeCurrentWorkTaskToFirstNonPinnedTask(IList<ITask> taskList) {
			// use first non-pinned task.
			foreach (ITask t in taskList)
			{
				if (!t.IsPinned)
				{
					ActiveTask = t;
					break;
				}
			}
		}

		private void InitializeCurrentWorkTaskFromLastSession(IList<ITask> taskList) {
			foreach (ITask task in taskList)
			{
				if (LastCurrentWorkTaskLabel == task.Label)
				{
					ActiveTask = task;
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
					if (_currentWorkTab == null)
					{
						_currentWorkTab = new TabPage();
						this.tabControl1.TabPages.Add(_currentWorkTab);
					}
					_currentWorkTab.Tag = value;
					_currentWorkTab.Text = value.Label;
					tabPageToActivate = _currentWorkTab;
					LastCurrentWorkTaskLabel = value.Label;
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

		void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TabPage page = ((TabControl)sender).SelectedTab;
			ActivateTab(page);
		}

		private void ActivateTab(TabPage page)
		{
			ITask task = (ITask)page.Tag;
			if (_activeTask == task)
				return; //debounce

			if (_activeTask != null)
				_activeTask.Deactivate();
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
			string previousTabText = page.Text;
			page.Text += " Loading...";
			System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
			t.Tick+=new EventHandler(delegate
										   {
											   ActivateTask(page, task);
												 t.Enabled = false;
												page.Text = previousTabText;
										 });
			t.Interval = 1;
			t.Enabled = true;

		}

		private void ActivateTask(TabPage page, ITask task)
		{
			task.Activate();
			task.Control.Dock = DockStyle.Fill;
			page.Controls.Add(task.Control);
			page.Cursor = Cursors.Default;
			_activeTask = task;
		}


		private void ContinueLaunchingAfterInitialDisplay()
		{
			System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
			t.Tick += new EventHandler(delegate
										   {
											   InitializeTasks(WeSayWordsProject.Project.Tasks);
											   t.Enabled = false;
										   });
			t.Interval = 1;
			t.Enabled = true;
		}

		private void TabbedForm_Load(object sender, EventArgs e)
		{
			ContinueLaunchingAfterInitialDisplay();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.CommonTools
{
	public partial class DashboardControl : UserControl, ITask
	{
		IRecordListManager _recordListManager;
		ICurrentWorkTask _currentWorkTaskProvider;
		IList<TaskIndicator> _taskIndicators;
		private bool _isActive;

		public DashboardControl(IRecordListManager recordListManager, ICurrentWorkTask currentWorkTaskProvider)
		{
			if (recordListManager == null)
			{
				throw new ArgumentNullException("recordListManager");
			}
			if (currentWorkTaskProvider == null)
			{
				throw new ArgumentNullException("currentWorkTaskProvider");
			}
			_taskIndicators = new List<TaskIndicator>();
			_recordListManager = recordListManager;
			_currentWorkTaskProvider = currentWorkTaskProvider;
			InitializeComponent();
	   }

		private TaskIndicator TaskIndicatorFromTask(ITask task)
		{
			TaskIndicator taskIndicator = new TaskIndicator(task);
			taskIndicator.selected += new EventHandler(OnTaskIndicatorSelected);
			_taskIndicators.Add(taskIndicator);
			return taskIndicator;
		}

		void OnTaskIndicatorSelected(object sender, EventArgs e)
		{
			TaskIndicator taskIndicator = (TaskIndicator) sender;
			_currentWorkTaskProvider.ActiveTask = taskIndicator.Task;
		}

		private void AddIndicator(TaskIndicator indicator)
		{
			Panel indentPanel = new Panel();
			indicator.Left = 70;
			indicator.Top = 0;
			indentPanel.Size = new System.Drawing.Size(indicator.Right,indicator.Height);
			indentPanel.AutoSize = true;
			indentPanel.Controls.Add(indicator);
			this._vbox.AddControlToBottom(indentPanel);
		}

		#region ITask
		public void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException("Activate should not be called when object is active.");
			}
			this._projectNameLabel.Text = BasilProject.Project.Name;
			DictionaryStatusControl status = new DictionaryStatusControl(_recordListManager.Get<LexEntry>());
			this._vbox.AddControlToBottom(status);
			ITask currentWorkTask = _currentWorkTaskProvider.CurrentWorkTask;
			if (currentWorkTask != null)
			{
				_vbox.AddControlToBottom(new CurrentTaskIndicatorControl(TaskIndicatorFromTask(currentWorkTask)));
			}

			IList<ITask> taskList = ((WeSayWordsProject)BasilProject.Project).Tasks;
			foreach (ITask task in taskList)
			{
				if (task != this && task.IsPinned)
				{
					AddIndicator(TaskIndicatorFromTask(task));
				}
			}

			foreach (ITask task in taskList)
			{
				if (task != this && !task.IsPinned)
				{
					AddIndicator(TaskIndicatorFromTask(task));
				}
			}
			_isActive = true;
		}

		public void Deactivate()
		{
			if(!IsActive)
			{
				throw new InvalidOperationException("Deactivate should only be called once after Activate.");
			}
			foreach (TaskIndicator taskIndicator in _taskIndicators)
			{
				taskIndicator.selected -= OnTaskIndicatorSelected;
			}
			this._vbox.Clear();
			_isActive = false;
		}

		public bool IsActive
		{
			get { return this._isActive; }
		}

		public string Label
		{
			get { return StringCatalog.Get("Dashboard");}
		}

		public Control Control
		{
			get { return this; }
		}

		public bool IsPinned
		{
			get
			{
				return true;
			}
		}

		public string Status
		{
			get
			{
				return string.Empty;
			}
		}

		public string Description
		{
			get
			{
				return StringCatalog.Get("Switch tasks and see current status of tasks");
			}
		}

		#endregion
	}
}

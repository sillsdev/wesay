using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
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
		  //  indicator.BorderStyle = BorderStyle.Fixed3D;
			indicator.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		   // indicator.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			indentPanel.Size = new System.Drawing.Size(indicator.Right,indicator.Height);
			indentPanel.SizeChanged += new EventHandler(indentPanel_SizeChanged);
		  //  indentPanel.AutoSize = true;
			indentPanel.Controls.Add(indicator);
		//    indentPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			//indentPanel.ForeColor = System.Drawing.Color.Aqua;
		   // indentPanel.BorderStyle = BorderStyle.FixedSingle;
			this._vbox.AddControlToBottom(indentPanel);
		}

		//I don't know why this was needed, but it works
		void indentPanel_SizeChanged(object sender, EventArgs e)
		{
			((TaskIndicator)(((Panel)sender).Controls[0])).RecalcSize(this,null);
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

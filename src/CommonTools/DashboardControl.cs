using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class DashboardControl: UserControl, ITask, IFinishCacheSetup
	{
		private readonly LexEntryRepository _lexEntryRepository;
		private readonly ICurrentWorkTask _currentWorkTaskProvider;
		private readonly IList<TaskIndicator> _taskIndicators;
		private bool _isActive;
		private CurrentTaskIndicatorControl _currentTaskIndicator;

		public DashboardControl()
		{
			if (!DesignMode)
			{
				throw new NotSupportedException("Please use other constructor");
			}
			InitializeComponent();
		}

		public DashboardControl(LexEntryRepository lexEntryRepository,
								ICurrentWorkTask currentWorkTaskProvider)
		{
			if (lexEntryRepository == null)
			{
				throw new ArgumentNullException("lexEntryRepository");
			}
			if (currentWorkTaskProvider == null)
			{
				throw new ArgumentNullException("currentWorkTaskProvider");
			}
			_taskIndicators = new List<TaskIndicator>();
			_lexEntryRepository = lexEntryRepository;
			_currentWorkTaskProvider = currentWorkTaskProvider;

			//InitializeComponent();
			InitializeContextMenu();
		}

		private void InitializeContextMenu()
		{
			ContextMenu = new ContextMenu();
			ContextMenu.MenuItems.Add("Configure this project...", OnRunConfigureTool);
			ContextMenu.MenuItems.Add("Use projector-friendly colors", OnToggleColorScheme);
			ContextMenu.MenuItems[1].Checked = DisplaySettings.Default.UsingProjectorScheme;
		}

		private void OnToggleColorScheme(object sender, EventArgs e)
		{
			DisplaySettings.Default.ToggleColorScheme();
			ContextMenu.MenuItems[1].Checked = DisplaySettings.Default.UsingProjectorScheme;
			if (_currentTaskIndicator != null)
			{
				_currentTaskIndicator.UpdateColors();
			}
		}

		private static void OnRunConfigureTool(object sender, EventArgs e)
		{
			string dir = Directory.GetParent(Application.ExecutablePath).FullName;
			ProcessStartInfo startInfo =
					new ProcessStartInfo(Path.Combine(dir, "WeSay Configuration Tool.exe"),
										 string.Format("\"{0}\"",
													   WeSayWordsProject.Project.PathToConfigFile));
			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				ErrorReport.ReportNonFatalMessage("Could not start " + startInfo.FileName);
				return;
			}

			Application.Exit();
		}

		private TaskIndicator TaskIndicatorFromTask(ITask task)
		{
			TaskIndicator taskIndicator = new TaskIndicator(task);
			taskIndicator.Selected += OnTaskIndicatorSelected;
			_taskIndicators.Add(taskIndicator);
			return taskIndicator;
		}

		private void OnTaskIndicatorSelected(object sender, EventArgs e)
		{
			TaskIndicator taskIndicator = (TaskIndicator) sender;
			_currentWorkTaskProvider.ActiveTask = taskIndicator.Task;
		}

		private void AddIndicator(Control indicator)
		{
			indicator.Dock = DockStyle.Fill;
			indicator.Margin = new Padding(70, 0, 20, 5);
			_panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			_panel.RowCount = _panel.Controls.Count;
			_panel.Controls.Add(indicator);
		}

		private void InitializeProjectNameLabel()
		{
			_projectNameLabel = new Label();
			_projectNameLabel.AutoSize = true;
			_projectNameLabel.Font = new Font("Microsoft Sans Serif",
											  20.25F,
											  FontStyle.Bold,
											  GraphicsUnit.Point,
											  0);
			_projectNameLabel.Location = new Point(14, 13);
			_projectNameLabel.Name = "_projectNameLabel";
			_projectNameLabel.Size = new Size(194, 31);
			_projectNameLabel.TabIndex = 0;
			_projectNameLabel.Text = BasilProject.Project.Name;
			_panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			_panel.Controls.Add(_projectNameLabel);
		}

		#region ITask

		public void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException(
						"Activate should not be called when object is active.");
			}
			InitializeComponent();
			SuspendLayout();
			_panel.SuspendLayout();
			_panel.Controls.Clear();
			InitializeProjectNameLabel();
			int entriesCount = _lexEntryRepository.CountAllItems();
			ItemsToDoIndicator.MakeAllInstancesSameWidth(entriesCount);
			DictionaryStatusControl status = new DictionaryStatusControl(entriesCount);
			_panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			_panel.Controls.Add(status);

			ITask currentWorkTask = _currentWorkTaskProvider.CurrentWorkTask;
			if (currentWorkTask != null)
			{
				TaskIndicator currentTaskIndicator = TaskIndicatorFromTask(currentWorkTask);
				_currentTaskIndicator = new CurrentTaskIndicatorControl(currentTaskIndicator);
				_currentTaskIndicator.Anchor = AnchorStyles.Left | AnchorStyles.Right |
											   AnchorStyles.Top;
				_currentTaskIndicator.Margin = new Padding(0, 0, 15, 5);
				_panel.Controls.Add(_currentTaskIndicator);
			}

			IList<ITask> taskList = ((WeSayWordsProject) BasilProject.Project).Tasks;

#if WantToShowIndicatorsForPinnedTasks
			foreach (ITask task in taskList)
			{
				if (task != this && task.IsPinned)
				{
					AddIndicator(TaskIndicatorFromTask(task));
				}
			}
#endif
			int count = 0;
			foreach (ITask task in taskList)
			{
				if (task != this && !task.IsPinned) // && (task != currentWorkTask))
				{
					count++;
				}
			}

			if (count > 1 || currentWorkTask == null)
			{
#if WantToShowIndicatorsForPinnedTasks
				GroupHeader header = new GroupHeader();
				header.Name = StringCatalog.Get("~Tasks");
				AddGroupHeader(header);
#endif

				foreach (ITask task in taskList)
				{
					if (task != this && !task.IsPinned && (task != currentWorkTask))
					{
						AddIndicator(TaskIndicatorFromTask(task));
					}
				}
			}

			_isActive = true;
			_panel.ResumeLayout(false);
			ResumeLayout(true);
		}

		public void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException(
						"Deactivate should only be called once after Activate.");
			}
			foreach (TaskIndicator taskIndicator in _taskIndicators)
			{
				taskIndicator.Selected -= OnTaskIndicatorSelected;
			}
			Controls.Clear();
			_isActive = false;
		}

		#region ITask Members

		public void GoToUrl(string url) {}

		#endregion

		public bool IsActive
		{
			get { return _isActive; }
		}

		public string Label
		{
			get
			{
				return StringCatalog.Get("~Home",
										 "The label for the 'dashboard'; the task which lets you see the status of other tasks and jump to them.");
			}
		}

		public Control Control
		{
			get { return this; }
		}

		private const int CountNotRelevant = -1;

		/// <summary>
		/// Not relevant for this task
		/// </summary>
		public int GetReferenceCount()
		{
			return CountNotRelevant;
		}

		public bool IsPinned
		{
			get { return true; }
		}

		public int GetRemainingCount()
		{
			return CountNotRelevant;
		}

		public bool AreCountsRelevant()
		{
			return false;
		}

		public int ExactCount
		{
			get { return CountNotRelevant; }
		}

		public string Description
		{
			get { return StringCatalog.Get("~Switch tasks and see current status of tasks"); }
		}

		public bool MustBeActivatedDuringPreCache
		{
			get { return false; }
		}

		public string GetRemainingCountText()
		{
			return string.Empty;
		}

		public string GetReferenceCountText()
		{
			return string.Empty;
		}

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.DontShow; }
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

		#endregion

		#region IFinishCacheSetup Members

		public void FinishCacheSetup()
		{
			Activate();
			Deactivate();
		}

		#endregion
	}
}
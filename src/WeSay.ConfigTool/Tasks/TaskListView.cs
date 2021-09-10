using Autofac;
using SIL.i18n;
using SIL.Reporting;
using System;
using System.Windows.Forms;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public partial class TaskListView : ConfigurationControlBase
	{
		private readonly IComponentContext _diContainer;
		public TaskListPresentationModel Model { get; set; }

		public TaskListView(ILogger logger, IComponentContext diContainer)
			: base("set up tasks for the user", logger, "tasks")
		{
			_diContainer = diContainer;

			InitializeComponent();
			splitContainer1.Resize += splitContainer1_Resize;
		}


		private void splitContainer1_Resize(object sender, EventArgs e)
		{
			try
			{
				//this is part of dealing with .net not adjusting stuff well for different dpis
				splitContainer1.Dock = DockStyle.None;
				splitContainer1.Width = Width - 25;
			}
			catch (Exception)
			{
				//swallow
			}
		}

		private void TaskList_Load(object sender, EventArgs e)
		{
			if (DesignMode)
			{
				return;
			}

			_taskList.Font = (System.Drawing.Font)SIL.i18n.StringCatalog.LabelFont.Clone();
			foreach (var task in Model.Tasks)
			{
				if (!Model.DoShowTask(task))
					continue;

				bool showCheckMark = task.IsVisible || (!task.IsOptional);

				_taskList.Items.Add(task, showCheckMark);
			}
			if (_taskList.Items.Count > 0)
			{
				_taskList.SelectedIndex = 0;
			}
		}

		private void _taskList_SelectedIndexChanged(object sender, EventArgs e)
		{
			splitContainer1.Panel2.Controls.Clear();
			var configuration = _taskList.SelectedItem as ITaskConfiguration;
			if (configuration == null)
			{
				return;
			}

			//      autofac's generated factory stuff wasn't working with our version of autofac, so
			//  i abandoned this
			//            Control c = null;
			//            try
			//            {
			//                //look for a factory that makes controls for this kind of task configuration
			//                  _context.Resolve(configuration.TaskName);
			//            }
			//            catch (Exception)
			//            {
			//            }
			//            if(c!=null)
			//            {
			//                splitContainer1.Panel2.Controls.Add(c);
			//            }
			var control = ConfigTaskControlFactory.Create(_diContainer, configuration);
			control.Dock = DockStyle.Fill;
			splitContainer1.Panel2.Controls.Add(control);

		}

		private void _taskList_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			var i = _taskList.SelectedItem as ITaskConfiguration;
			if (i == null)
			{
				return;
			}
			if (!i.IsOptional)
			{
				e.NewValue = CheckState.Checked;
			}
			i.IsVisible = e.NewValue == CheckState.Checked;
			if (e.NewValue == CheckState.Checked)
			{
				_logger.WriteConciseHistoricalEvent(StringCatalog.Get("Enabled {0}", "Checkin Description in WeSay Config Tool used when you enable a task."), _taskList.SelectedItem.ToString());
			}
			else
			{
				_logger.WriteConciseHistoricalEvent(StringCatalog.Get("Disabled {0}", "Checkin Description in WeSay Config Tool used when you disable a task."), _taskList.SelectedItem.ToString());
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public partial class DefaultTaskConfigurationControl : UserControl
	{
		protected readonly ITaskConfiguration _config;

		public DefaultTaskConfigurationControl()
		{
			InitializeComponent();
		}

		public DefaultTaskConfigurationControl(ITaskConfiguration config, bool haveSetupControls)
		{
			_config = config;
			InitializeComponent();
			_description.Text = config.Description;
			_setupLabel.Visible = haveSetupControls;
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.LexicalTools.AddMissingInfo;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public partial class MissingInfoTaskConfigControl : DefaultTaskConfigurationControl
	{

		private MissingInfoTaskConfigControl()
		{
			InitializeComponent();
		}

		public MissingInfoTaskConfigControl(ITaskConfiguration config)
			:base(config)
		{
			InitializeComponent();
			_showExampleField.Visible = Configuration.IncludesField(LexicalModel.LexExampleSentence.WellKnownProperties.ExampleSentence);
			_showExampleField.Checked = Configuration.IncludesField(LexicalModel.LexExampleSentence.WellKnownProperties.Translation);
		}
		private MissingInfoConfiguration Configuration
		{
			get
			{
				return (MissingInfoConfiguration)_config;
			}
		}

		private void OnShowExampleField_CheckedChanged(object sender, EventArgs e)
		{
			Configuration.SetInclusionOfField(LexicalModel.LexExampleSentence.WellKnownProperties.Translation, _showExampleField.Checked);
		}
	}
}

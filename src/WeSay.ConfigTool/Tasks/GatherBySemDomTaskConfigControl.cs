using System;
using WeSay.LexicalTools.GatherBySemanticDomains;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public partial class GatherBySemDomTaskConfigControl : DefaultTaskConfigurationControl
	{

		private GatherBySemDomTaskConfigControl()
		{
			InitializeComponent();
		}

		public GatherBySemDomTaskConfigControl(ITaskConfiguration config)
			:base(config)
		{
			InitializeComponent();
			_showMeaningField.Checked =  Configuration.ShowMeaningField;
		}
		private GatherBySemanticDomainConfig Configuration
		{
			get
			{
				return (GatherBySemanticDomainConfig)_config;
			}
		}

		private void OnShowMeaning_CheckedChanged(object sender, EventArgs e)
		{
			Configuration.ShowMeaningField = _showMeaningField.Checked;
		}
	}
}

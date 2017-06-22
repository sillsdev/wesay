using System;
using WeSay.LexicalTools.DictionaryBrowseAndEdit;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public partial class DictionaryBrowseEditTaskConfigControl : DefaultTaskConfigurationControl
	{
		public DictionaryBrowseEditTaskConfigControl(ITaskConfiguration config)
			: base(config, true)
		{
			InitializeComponent();
			switch (Configuration.MeaningField)
			{
				case "definition":
					_definition.Checked = true;
					break;
				case "gloss":
					_gloss.Checked = true;
					break;
			}
		}

		private DictionaryBrowseAndEditConfiguration Configuration
		{
			get
			{
				return (DictionaryBrowseAndEditConfiguration)_config;
			}
		}

		private void OnGloss_RadioClicked(object sender, EventArgs e)
		{
			Configuration.MeaningField = "gloss";
		}

		private void OnDefinition_RadioClicked(object sender, EventArgs e)
		{
			Configuration.MeaningField = "definition";
		}

	}
}

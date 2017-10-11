using System;
using WeSay.LexicalTools.DictionaryBrowseAndEdit;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public partial class DictionaryBrowseEditTaskConfigControl : DefaultTaskConfigurationControl
	{
		private bool _initialising;
		public DictionaryBrowseEditTaskConfigControl(ITaskConfiguration config)
			: base(config, true)
		{
			_initialising = true;
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
			_initialising = false;
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
			if (!_initialising)
			{
				Configuration.MeaningField = "gloss";
				WeSayWordsProject.Project.MakeMeaningFieldChange("definition", "gloss");
			}
		}

		private void OnDefinition_RadioClicked(object sender, EventArgs e)
		{
			if (!_initialising)
			{
				Configuration.MeaningField = "definition";
				WeSayWordsProject.Project.MakeMeaningFieldChange("gloss", "definition");
			}
		}
	}
}

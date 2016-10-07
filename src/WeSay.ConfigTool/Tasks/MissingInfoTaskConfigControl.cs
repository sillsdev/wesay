using System;
using System.Collections.Generic;
using Palaso.DictionaryServices.Model;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalTools.AddMissingInfo;
using WeSay.Project;
using System.Linq;

namespace WeSay.ConfigTool.Tasks
{
	public partial class MissingInfoTaskConfigControl : DefaultTaskConfigurationControl
	{
		private readonly ViewTemplate _viewTemplate;
		private List<IWritingSystemDefinition> _relevantWritingSystems;
		private Field _field;

		public MissingInfoTaskConfigControl(MissingInfoConfiguration config, ViewTemplate viewTemplate)
			:base(config, true)
		{
			_viewTemplate = viewTemplate;
			InitializeComponent();

			_showExampleFieldBox.Visible = Configuration.IncludesField(LexExampleSentence.WellKnownProperties.ExampleSentence);
			//these are separated to get the long label to show in mono
			 _showExampleLabel.Visible = _showExampleFieldBox.Visible;
			_showExampleFieldBox.Checked = Configuration.IncludesField(LexExampleSentence.WellKnownProperties.Translation);
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
			Configuration.SetInclusionOfField(LexExampleSentence.WellKnownProperties.Translation, _showExampleFieldBox.Checked);
		}

		private void MissingInfoTaskConfigControl_Load(object sender, EventArgs e)
		{
			_field = _viewTemplate.GetField(Configuration.MissingInfoFieldName);
			_matchWhenEmpty.Visible = _field.DataTypeName == Field.BuiltInDataType.MultiText.ToString();
			_matchWhenEmptyLabel.Visible = _matchWhenEmpty.Visible;
			_requiredToBeFilledIn.Visible = _matchWhenEmpty.Visible;
			_requiredToBeFilledInLabel.Visible = _requiredToBeFilledIn.Visible;

			_setupLabel.Visible = _matchWhenEmpty.Visible;

			if (_matchWhenEmpty.Visible)
			{
				_field.WritingSystemsChanged += OnWritingSystemsChanged;
				Configuration.WritingSystemIdChanged += OnWritingSystemsChanged;
				Configuration.WritingSystemIdDeleted += OnWritingSystemsDeleted;
				UpdateWritingSystemFilterControls();
			}
		}

		private void OnWritingSystemsDeleted(object sender, EventArgs e)
		{
			UpdateWritingSystemFilterControls();
		}

		private void UpdateWritingSystemFilterControls()
		{
			_relevantWritingSystems = new List<IWritingSystemDefinition>();
			var relevantWritingSystems = from x in _viewTemplate.WritingSystems.AllWritingSystems
										 where _field.WritingSystemIds.Contains(x.Id)
										 select x;
			_relevantWritingSystems.AddRange(relevantWritingSystems);


			_matchWhenEmpty.Init(_relevantWritingSystems,
								 Configuration.WritingSystemsWeWantToFillIn,
								 "any");
			// _matchWhenEmpty.Changed += new EventHandler(_matchWhenEmpty_Changed);
			_requiredToBeFilledIn.Init(_relevantWritingSystems,
									   Configuration.WritingSystemsWhichAreRequired,
									   "none");
		}

		private void OnWritingSystemsChanged(object sender, EventArgs e)
		{
			if (_matchWhenEmpty.Visible)
			{
				UpdateWritingSystemFilterControls();
			}
		}

		private void MissingInfoTaskConfigControl_BackColorChanged(object sender, EventArgs e)
		{
			_showExampleLabel.BackColor = BackColor;
			_requiredToBeFilledInLabel.BackColor = BackColor;
			_matchWhenEmptyLabel.BackColor = BackColor;
		}

	}
}

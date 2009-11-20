using System;
using System.Collections.Generic;
using Palaso.Lift.Model;
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
		private List<WritingSystem> _relevantWritingSystems;

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
			var field = _viewTemplate.GetField(Configuration.MissingInfoFieldName);
			_matchWhenEmpty.Visible = field.DataTypeName == Field.BuiltInDataType.MultiText.ToString();
			_matchWhenEmptyLabel.Visible = _matchWhenEmpty.Visible;
			_requiredToBeFilledIn.Visible = _matchWhenEmpty.Visible;
			_requiredToBeFilledInLabel.Visible = _requiredToBeFilledIn.Visible;

			_setupLabel.Visible = _matchWhenEmpty.Visible;

			if (_matchWhenEmpty.Visible)
			{
				_relevantWritingSystems = new List<WritingSystem>();
				var relevantWritingSystems = from x in _viewTemplate.WritingSystems
											 where field.WritingSystemIds.Contains(x.Key)
											 select x.Value;
				_relevantWritingSystems.AddRange(relevantWritingSystems);


				_matchWhenEmpty.Init(_relevantWritingSystems,
									 Configuration.WritingSystemsWeWantToFillIn,
									 "any");
				// _matchWhenEmpty.Changed += new EventHandler(_matchWhenEmpty_Changed);
				_requiredToBeFilledIn.Init(_relevantWritingSystems,
										   Configuration.WritingSystemsWhichAreRequired,
										   "none");
			}
		}

		private void MissingInfoTaskConfigControl_BackColorChanged(object sender, EventArgs e)
		{
			_showExampleLabel.BackColor = this.BackColor;
			_requiredToBeFilledInLabel.BackColor = this.BackColor;
			_matchWhenEmptyLabel.BackColor = this.BackColor;
		}

	}
}

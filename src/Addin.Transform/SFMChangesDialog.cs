using System;
using System.Windows.Forms;
using SIL.WritingSystems;
using WeSay.AddinLib;
using WeSay.LexicalModel.Foundation;

namespace Addin.Transform
{
	public partial class SFMChangesDialog: Form
	{
		private readonly SfmTransformSettings _settings;
		private readonly ProjectInfo _projectInfo;

		public SFMChangesDialog(SfmTransformSettings settings, ProjectInfo projectInfo)
		{
			_settings = settings;
			_projectInfo = projectInfo;
			InitializeComponent();
			//the xml serialization process seems to convert the \r\n we need (on windows) to \n
			_pairsText.Text = settings.SfmTagConversions.Replace("\n", Environment.NewLine);

			_settings.FillEmptySettingsWithGuesses(projectInfo);
			FillLanguageCombos(_vernacularLanguage, settings.VernacularLanguageWritingSystemId);
			FillLanguageCombos(_englishLanguage, settings.EnglishLanguageWritingSystemId);
			FillLanguageCombos(_nationalLanguage, settings.NationalLanguageWritingSystemId);
			FillLanguageCombos(_regionalLanguage, settings.RegionalLanguageWritingSystemId);
		}

		private void FillLanguageCombos(ComboBox languageCombo, string currentWSId)
		{
			if (_projectInfo != null)
			{
				languageCombo.Items.Clear();
				foreach (WritingSystemDefinition ws in _projectInfo.WritingSystems.AllWritingSystems)
				{
					languageCombo.Items.Add(ws.Id);
				}
				if (currentWSId != null && languageCombo.Items.Contains(currentWSId))
				{
					languageCombo.SelectedItem = currentWSId;
				}
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_settings.SfmTagConversions = _pairsText.Text;
			_settings.VernacularLanguageWritingSystemId = _vernacularLanguage.SelectedItem as string;
			_settings.EnglishLanguageWritingSystemId = _englishLanguage.SelectedItem as string;
			_settings.NationalLanguageWritingSystemId = _nationalLanguage.SelectedItem as string;
			_settings.RegionalLanguageWritingSystemId = _regionalLanguage.SelectedItem as string;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void _pairsText_TextChanged(object sender, EventArgs e) {}

		private void textBox1_TextChanged(object sender, EventArgs e) {}

		private void SFMChangesDialog_Load(object sender, EventArgs e) {}
	}
}
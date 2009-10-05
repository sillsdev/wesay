using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class InterfaceLanguageControl: ConfigurationControlBase
	{
		public InterfaceLanguageControl(ILogger logger)
			: base("settings for the user interface", logger)
		{
			InitializeComponent();
		}

		private void OnLoad(object sender, EventArgs e)
		{
			LoadPoFilesIntoCombo(
					WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject);
			LoadPoFilesIntoCombo(BasilProject.ApplicationCommonDirectory);

			UpdateFontDisplay();
			_languageCombo.SelectedIndexChanged += _languageCombo_SelectedIndexChanged;
		}

		private void _languageCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_languageCombo.SelectedItem != null)
			{
				var lang = ((PoProxy) _languageCombo.SelectedItem).fileNameWithoutExtension;
				if (UILanguage != lang)
				{
					UILanguage = lang;
					if(lang==string.Empty)
					{
						lang = "default";
					}
					_logger.WriteConciseHistoricalEvent(StringCatalog.Get("Changed UI Language to {0}", "Checkin Description in WeSay Config Tool used when you change the User Interface language."),lang);
				}
			}
		}

		private void LoadPoFilesIntoCombo(string directory)
		{
			_languageCombo.Items.Clear();
			foreach (string file in Directory.GetFiles(directory, "*.po"))
			{
				PoProxy selector = new PoProxy(file);

				if (selector.fileNameWithoutExtension == "wesay-en")
				{
					selector.LanguageName = "English (Default)";
				}

				_languageCombo.Items.Add(selector);

				bool currentFileCorrespondsToConfiguredLanguage = (Options.Language == selector.fileNameWithoutExtension);


				if (currentFileCorrespondsToConfiguredLanguage)
				{
					_languageCombo.SelectedItem = selector;
				}
			}
		}

		private class PoProxy
		{
			public PoProxy() {}

			public PoProxy(string path)
			{
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
				_languageName = fileNameWithoutExtension;
				try
				{
					string contents = File.ReadAllText(path);
					Match m = Regex.Match(contents, @"# (.*) translation");
					if (m.Success)
					{
						_languageName = m.Groups[1].Value.Trim();
					}
				}
				catch (Exception) //couldn't extract a better name
				{}
			}

			public string fileNameWithoutExtension;
			private string _languageName;

			public string LanguageName
			{
				get { return _languageName; }
				set { _languageName = value; }
			}

			public override string ToString()
			{
				return _languageName;
			}
		}

		private class EnglishPoProxy: PoProxy
		{
			public EnglishPoProxy()
			{
				LanguageName = "English (Default)";
				fileNameWithoutExtension = string.Empty;
			}
		}

		private string UILanguage
		{
			get
			{
				return Options.Language;
				//                if (_languageCombo.SelectedItem == null)
				//                {
				//                    return String.Empty;
				//                }
				//                return ((PoProxy) _languageCombo.SelectedItem).fileNameWithoutExtension;
			}
			set
			{
				if (_languageCombo.SelectedItem != null)
				{
					Options.Language = value;
				}
			}
		}


		private UiConfigurationOptions Options
		{
			get { return WeSayWordsProject.Project.UiOptions; }
		}

		private void OnChooseFont(object sender, EventArgs e)
		{
			FontDialog dialog = new FontDialog();
			dialog.Font = Options.GetLabelFont();
			dialog.ShowColor = false;
			dialog.ShowEffects = false;

			try //strange, but twice we've found situations where ShowDialog crashes on windows
			{
				if (DialogResult.OK != dialog.ShowDialog())
				{
					return;
				}
			}
			catch (Exception)
			{
				ErrorReport.NotifyUserOfProblem(
						"There was some problem with choosing that font.  If you just installed it, you might try restarting the program or even your computer.");
				return;
			}
			Options.SetLabelFont(dialog.Font);
			UpdateFontDisplay();
		}

		private void UpdateFontDisplay()
		{
			_fontInfoDisplay.Text = string.Format("{0}, {1} points",
												  Options.LabelFontName,
												  Math.Round(Options.LabelFontSizeInPoints));
		}
	}
}
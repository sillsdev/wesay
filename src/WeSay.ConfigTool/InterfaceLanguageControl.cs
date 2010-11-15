using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Palaso.i18n;
using Palaso.Reporting;
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
			LoadPoFilesIntoCombo(WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject);
			LoadPoFilesIntoCombo(BasilProject.ApplicationCommonDirectory);

			UpdateFontDisplay();
			_languageCombo.SelectedIndexChanged += _languageCombo_SelectedIndexChanged;
		}

		private void _languageCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_languageCombo.SelectedItem != null)
			{
				var lang = ((PoProxy) _languageCombo.SelectedItem).LanguageCode;
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
			var englishPoProxy = new EnglishPoProxy();
			_languageCombo.Items.Add(englishPoProxy);
			_languageCombo.SelectedItem = englishPoProxy;
			foreach (string file in Directory.GetFiles(directory, "*.po"))
			{
				var selector = new PoProxy(file);
				_languageCombo.Items.Add(selector);
				if (Options.Language ==
					selector.LanguageCode)
				{
					_languageCombo.SelectedItem = selector;
				}
			}
		}

		private class PoProxy
		{
			public string LanguageCode { get; protected set; }
			protected string LanguageName { private get; set; }

			protected PoProxy()
			{
			}

			public PoProxy(string poFilePath)
			{
				LanguageCode = PoFilePathToLanguageCode(poFilePath);
				LanguageName = LanguageCode;
				try
				{
					string contents = File.ReadAllText(poFilePath);
					Match m = Regex.Match(contents, @"# (.*) translation");
					if (m.Success)
					{
						LanguageName = m.Groups[1].Value.Trim();
					}
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch
				{}
				// ReSharper restore EmptyGeneralCatchClause
			}

			private static string PoFilePathToLanguageCode(string poFilePath)
			{
				var parts = poFilePath.Split(new[] {'.', '-'});
				return parts[parts.Length - 2];
			}

			public override string ToString()
			{
				return LanguageName;
			}
		}

		private class EnglishPoProxy: PoProxy
		{
			public EnglishPoProxy()
			{
				LanguageName = "English (Default)";
				LanguageCode = string.Empty;
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
			var dialog = new FontDialog
							 {
								 Font = Options.GetLabelFont(),
								 ShowColor = false,
								 ShowEffects = false
							 };

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
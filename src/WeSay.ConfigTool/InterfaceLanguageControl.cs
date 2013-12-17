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
		public InterfaceLanguageControl(): base("settings for the user interface")
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
				UILanguage = ((PoProxy) _languageCombo.SelectedItem).fileNameWithoutExtension;
			}
		}

		public override void PreLoad()
		{
			base.PreLoad();
			WeSayWordsProject.Project.EditorsSaveNow += Project_EditorsSaveNow;
		}

		private void LoadPoFilesIntoCombo(string directory)
		{
			_languageCombo.Items.Clear();
			EnglishPoProxy englishPoProxy = new EnglishPoProxy();
			_languageCombo.Items.Add(englishPoProxy);
			_languageCombo.SelectedItem = englishPoProxy;
			foreach (string file in Directory.GetFiles(directory, "*.po"))
			{
				PoProxy selector = new PoProxy(file);
				_languageCombo.Items.Add(selector);
				if (WeSayWordsProject.Project.StringCatalogSelector ==
					selector.fileNameWithoutExtension)
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
			protected string _languageName;

			public override string ToString()
			{
				return _languageName;
			}
		}

		private class EnglishPoProxy: PoProxy
		{
			public EnglishPoProxy()
			{
				_languageName = "English (Default)";
				fileNameWithoutExtension = string.Empty;
			}
		}

		private void Project_EditorsSaveNow(object owriter, EventArgs e)
		{
			XmlWriter writer = (XmlWriter) owriter;

			writer.WriteStartElement("uiOptions");
			if (!String.IsNullOrEmpty(UILanguage))
			{
				writer.WriteAttributeString("uiLanguage", UILanguage);
			}
			if (!String.IsNullOrEmpty(LabelName))
			{
				writer.WriteAttributeString("uiFont", LabelName);
				writer.WriteAttributeString("uiFontSize", LabelSizeInPoints.ToString());
			}
			writer.WriteEndElement();
		}

		private string UILanguage
		{
			get
			{
				return WeSayWordsProject.Project.StringCatalogSelector;
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
					WeSayWordsProject.Project.StringCatalogSelector = value;
				}
			}
		}

		private static string LabelName
		{
			get { return StringCatalog.LabelFont.Name; }
		}

		private static float LabelSizeInPoints
		{
			get { return StringCatalog.LabelFont.SizeInPoints; }
		}

		private void OnChooseFont(object sender, EventArgs e)
		{
			FontDialog dialog = new FontDialog();
			dialog.Font = StringCatalog.LabelFont;
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
				ErrorReport.ReportNonFatalMessage(
						"There was some problem with choosing that font.  If you just installed it, you might try restarting the program or even your computer.");
				return;
			}
			StringCatalog.LabelFont = dialog.Font;
			UpdateFontDisplay();
		}

		private void UpdateFontDisplay()
		{
			_fontInfoDisplay.Text = string.Format("{0}, {1} points",
												  StringCatalog.LabelFont.Name,
												  (int) StringCatalog.LabelFont.SizeInPoints);
		}
	}
}
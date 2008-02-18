using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class InterfaceLanguageControl : ConfigurationControlBase
	{
		public InterfaceLanguageControl():base("settings for the user interface")
		{
			InitializeComponent();
		}

		private void OnLoad(object sender, EventArgs e)
		{
			LoadPoFilesIntoCombo(Project.WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject);
			LoadPoFilesIntoCombo(Project.WeSayWordsProject.ApplicationCommonDirectory);

			WeSayWordsProject.Project.EditorsSaveNow += Project_HackedEditorsSaveNow;
			UpdateFontDisplay();
	   }

		private void LoadPoFilesIntoCombo(string directory)
		{
			foreach (string file in Directory.GetFiles(directory,"*.po"))
			{
				PoProxy selector = new PoProxy(file);
				_languageCombo.Items.Add(selector);
				if(Project.WeSayWordsProject.Project.StringCatalogSelector == selector.fileNameWithoutExtension)
				{
					_languageCombo.SelectedItem = selector;
				}
			}
		}

		private class PoProxy
		{
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
				catch(Exception) //couldn't extract a better name
				{
				}
			}

			public string fileNameWithoutExtension;
			private string _languageName;
			public override string ToString()
			{
				return _languageName;
			}
		}

		void Project_HackedEditorsSaveNow(object owriter, EventArgs e)
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
				if (_languageCombo.SelectedItem == null)
				{
					return String.Empty;
				}
				return ((PoProxy) _languageCombo.SelectedItem).fileNameWithoutExtension;
			}
		}

		private string LabelName
		{
			get
			{
				return StringCatalog.LabelFont.Name;
			}
		}

		private float LabelSizeInPoints
		{
			get
			{
				return StringCatalog.LabelFont.SizeInPoints;
			}
		}

		private void OnChooseFont(object sender, EventArgs e)
		{
			System.Windows.Forms.FontDialog dialog = new FontDialog();
			dialog.Font = StringCatalog.LabelFont;
			dialog.ShowColor = false;
			dialog.ShowEffects = false;

			try//strange, but twice we've found situations where ShowDialog crashes on windows
			{
				if (DialogResult.OK != dialog.ShowDialog())
				{
					return;
				}
			}
			catch (Exception)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
					"There was some problem with choosing that font.  If you just installed it, you might try restarting the program or even your computer.");
				return;
			}
			StringCatalog.LabelFont = dialog.Font;
			UpdateFontDisplay();
		}
		private void UpdateFontDisplay()
		{
			_fontInfoDisplay.Text = string.Format("{0}, {1} points",StringCatalog.LabelFont.Name, (int)StringCatalog.LabelFont.SizeInPoints);
		}

		private void _fontInfoDisplay_TextChanged(object sender, EventArgs e)
		{

		}

	}
}

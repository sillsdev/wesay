using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
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
			LoadPoFilesIntoCombo(Project.WeSayWordsProject.Project.ApplicationCommonDirectory);

			WeSayWordsProject.Project.EditorsSaveNow += Project_HackedEditorsSaveNow;
			UpdateFontDisplay();
	   }

		private void LoadPoFilesIntoCombo(string directory)
		{
			foreach (string file in Directory.GetFiles(directory,"*.po"))
			{
				string selector = Path.GetFileNameWithoutExtension(file);
				_languageCombo.Items.Add(selector);
				if(Project.WeSayWordsProject.Project.StringCatalogSelector == selector)
				{
					_languageCombo.SelectedItem = selector;
				}
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
				return _languageCombo.SelectedItem.ToString();
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

		   if (DialogResult.OK != dialog.ShowDialog())
			{
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

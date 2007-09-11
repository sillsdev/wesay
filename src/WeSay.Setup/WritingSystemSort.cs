using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.Setup
{
	public partial class WritingSystemSort : UserControl
	{
		private WritingSystem _writingSystem;

		public WritingSystemSort()
		{
			InitializeComponent();
			List<CultureInfo> result = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));

			result.Sort(
					delegate(CultureInfo ci1, CultureInfo ci2) { return StringComparer.InvariantCulture.Compare(ci1.DisplayName, ci2.DisplayName); });

			result.Remove(CultureInfo.InvariantCulture);

			List<KeyValuePair<string,string>> sortChoices = new List<KeyValuePair<string, string>>();
			sortChoices.Add(new KeyValuePair<string, string>(null, "(select a sort method)"));

			sortChoices.Add(new KeyValuePair<string, string>("custom", "<Custom>"));
			foreach (CultureInfo cultureInfo in result)
			{
				sortChoices.Add(new KeyValuePair<string, string>(cultureInfo.IetfLanguageTag, cultureInfo.DisplayName));
			}

			comboBoxCultures.DataSource = sortChoices;
			comboBoxCultures.DisplayMember = "Value";
			comboBoxCultures.ValueMember = "Key";

			comboBoxCultures.SelectedValue = string.Empty;
			comboBoxCultures.SelectedIndexChanged += new EventHandler(comboBoxCultures_SelectedIndexChanged);

			textBoxCustomRules.Validated += new EventHandler(textBoxCustomRules_Validated);
		}

		private void textBoxCustomRules_Validated(object sender, EventArgs e)
		{
			_writingSystem.CustomSortRules = textBoxCustomRules.Text;
		}

		private void comboBoxCultures_SelectedIndexChanged(object sender, EventArgs e)
		{
			_writingSystem.SortUsing = (string) comboBoxCultures.SelectedValue;
			UpdateCustomRules();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;
				Refresh();
			}
		}

		public override void Refresh()
		{
			comboBoxCultures.SelectedValue = _writingSystem.SortUsing;
			UpdateCustomRules();
			base.Refresh();
		}

		private void UpdateCustomRules()
		{
			if (_writingSystem.SortUsing == WritingSystem.SortUsingCustomSortRules)
			{
				textBoxCustomRules.Visible = true;
				textBoxCustomRules.Text = _writingSystem.CustomSortRules;
			}
			else
			{
				textBoxCustomRules.Visible = false;
				textBoxCustomRules.Clear();
			}
		}

		private void buttonSortTest_Click(object sender, EventArgs e)
		{
			string trim = this.textBoxSortTest.Text.Trim('\n');
			string[] stringsToSort = trim.Split('\n');
			Array.Sort(stringsToSort, _writingSystem);
			string s = string.Empty;
			foreach (string s1 in stringsToSort)
			{
				s += s1.Trim('\r') + "\r\n";
			}
			this.textBoxSortTest.Text = s;
		}
	}
}
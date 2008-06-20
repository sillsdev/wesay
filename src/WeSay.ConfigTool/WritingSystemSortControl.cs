using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.WritingSystems.Collation;
using Spart;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemSortControl: UserControl
	{
		private WritingSystem _writingSystem;
		private readonly Color validBackgroundColor;
		private readonly Color invalidBackgroundColor;

		public WritingSystemSortControl()
		{
			InitializeComponent();
			if (DesignMode)
			{
				return;
			}

			List<CultureInfo> result =
					new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));

			result.Sort(
					delegate(CultureInfo ci1, CultureInfo ci2)
					{
						return
								StringComparer.InvariantCulture.Compare(ci1.DisplayName,
																		ci2.DisplayName);
					});

			result.Remove(CultureInfo.InvariantCulture);

			List<KeyValuePair<string, string>> sortChoices =
					new List<KeyValuePair<string, string>>();
			sortChoices.Add(new KeyValuePair<string, string>(null, "(select a sort method)"));

			foreach (Enum customSortRulesType in Enum.GetValues(typeof (CustomSortRulesType)))
			{
				FieldInfo fi =
						customSortRulesType.GetType().GetField(customSortRulesType.ToString());

				DescriptionAttribute[] descriptions =
						(DescriptionAttribute[])
						fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
				string description;
				if (descriptions.Length == 0)
				{
					description = customSortRulesType.ToString();
				}
				else
				{
					description = descriptions[0].Description;
				}
				sortChoices.Add(
						new KeyValuePair<string, string>(customSortRulesType.ToString(), description));
			}
			foreach (CultureInfo cultureInfo in result)
			{
				sortChoices.Add(
						new KeyValuePair<string, string>(cultureInfo.IetfLanguageTag,
														 cultureInfo.DisplayName));
			}

			comboBoxCultures.DataSource = sortChoices;
			comboBoxCultures.DisplayMember = "Value";
			comboBoxCultures.ValueMember = "Key";

			comboBoxCultures.SelectedValue = string.Empty;
			comboBoxCultures.SelectedIndexChanged += comboBoxCultures_SelectedIndexChanged;

			textBoxCustomRules.Validating += textBoxCustomRules_Validating;
			textBoxCustomRules.Validated += textBoxCustomRules_Validated;

			validBackgroundColor = textBoxCustomRules.BackColor;
			invalidBackgroundColor = Color.Tomato;

			WeSayWordsProject.Project.EditorsSaveNow += OnEditorSaveNow;
		}

		private void OnEditorSaveNow(object sender, EventArgs e)
		{
			// has never been initialized
			if (_writingSystem == null)
			{
				return;
			}

			ValidateChildren(); // this will make it save
		}

		private void textBoxCustomRules_Validating(object sender, CancelEventArgs cancelEventArgs)
		{
			ValidateCustomRules();
		}

		private void ValidateCustomRules()
		{
			//prevent WS-708:  WritingSystem Sort trying to validate non-custom rules
			if (!_writingSystem.UsesCustomSortRules)
			{
				return;
			}
			CustomSortRulesType customSortRulesType;
			try
			{
				customSortRulesType =
						(CustomSortRulesType)
						Enum.Parse(typeof (CustomSortRulesType), _writingSystem.SortUsing);
			}
			catch (ArgumentException)
			{
				ErrorReport.ReportNonFatalMessage(
						"WeSay could not understand this type of sorting ('{0}'). It will be reset.",
						_writingSystem.SortUsing);
				customSortRulesType = default(CustomSortRulesType);
			}

			string errorMessage;
			if (AreCustomRulesValid(textBoxCustomRules.Text, customSortRulesType, out errorMessage))
			{
				textBoxCustomRules.BackColor = validBackgroundColor;
				toolTip1.Active = false;
			}
			else
			{
				textBoxCustomRules.BackColor = invalidBackgroundColor;
				toolTip1.SetToolTip(textBoxCustomRules, errorMessage);
				toolTip1.Active = true;
				toolTip1.ShowAlways = true;
			}
		}

		private void textBoxCustomRules_Validated(object sender, EventArgs e)
		{
			_writingSystem.CustomSortRules =
					textBoxCustomRules.Text.Replace(Environment.NewLine, "\n");

			InvalidateCache();
		}

		private static void InvalidateCache()
		{
			WeSayWordsProject.Project.InvalidateCacheSilently();
		}

		private void comboBoxCultures_SelectedIndexChanged(object sender, EventArgs e)
		{
			string oldValue = _writingSystem.SortUsing;
			_writingSystem.SortUsing = (string) comboBoxCultures.SelectedValue;
			UpdateCustomRules();

			if (oldValue != _writingSystem.SortUsing)
			{
				InvalidateCache();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;
				if (_writingSystem != null)
				{
					textBoxSortTest.Font = _writingSystem.Font;
					textBoxCustomRules.Font = _writingSystem.Font;
				}
				Refresh();
			}
		}

		public override void Refresh()
		{
			//handle WS-707 : ws loses custom simple contents if unmodified
			if (_writingSystem.UsesCustomSortRules &&
				string.IsNullOrEmpty(_writingSystem.CustomSortRules))
			{
				_writingSystem.SortUsing = _writingSystem.Id;
			}

			comboBoxCultures.SelectedValue = _writingSystem.SortUsing;

			UpdateCustomRules();
			base.Refresh();
		}

		private void UpdateCustomRules()
		{
			if (_writingSystem.UsesCustomSortRules)
			{
				if (_writingSystem.SortUsing == CustomSortRulesType.CustomSimple.ToString() &&
					textBoxCustomRules.Text.Trim() == string.Empty)
				{
					textBoxCustomRules.Text =
							@"A a
B b
C c
D d
E e
F f
G g
H h
I i
J j
K k
L l
M m
N n
O o
P p
Q q
R r
S s
T t
U u
V v
W w
X x
Y y
Z z";
					textBoxCustomRules.Text =
							textBoxCustomRules.Text.Replace("\r\n", Environment.NewLine);
					textBoxCustomRules.Text =
							textBoxCustomRules.Text.Replace("\n", Environment.NewLine);
				}
				else
				{
					textBoxCustomRules.Text =
							_writingSystem.CustomSortRules.Replace("\n", Environment.NewLine);
				}

				textBoxCustomRules.Visible = true;
				ValidateCustomRules();
			}
			else
			{
				textBoxCustomRules.Visible = false;
				textBoxCustomRules.Clear();
			}
		}

		private static bool AreCustomRulesValid(string customRules,
												CustomSortRulesType type,
												out string errorMessage)
		{
			try
			{
				switch (type)
				{
					case CustomSortRulesType.CustomSimple:
						new SimpleRulesCollator(customRules);
						break;
					case CustomSortRulesType.CustomICU:
						new IcuRulesCollator(customRules);
						break;
					default:
						throw new NotSupportedException("Unexpected CustomSortRulesType");
				}
			}
			catch (DllNotFoundException e)
			{
				errorMessage = e.Message;
				return false;
			}
			catch (ParserErrorException e)
			{
				errorMessage =
						string.Format("{0} at line {1} column {2}",
									  e.ParserError.ErrorText,
									  e.ParserError.Line,
									  e.ParserError.Column);
				return false;
			}
			catch (ApplicationException e)
			{
				errorMessage = e.Message;
				return false;
			}
			errorMessage = string.Empty;
			return true;
		}

		private void buttonSortTest_Click(object sender, EventArgs e)
		{
			string text = textBoxSortTest.Text;
			string[] stringsToSort =
					text.Split(new string[] {Environment.NewLine},
							   StringSplitOptions.RemoveEmptyEntries);
			Array.Sort(stringsToSort, _writingSystem);
			string s = string.Empty;
			foreach (string s1 in stringsToSort)
			{
				s += s1 + Environment.NewLine;
			}
			textBoxSortTest.Text = s.Trim();
		}

		private void WritingSystemSort_Load(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
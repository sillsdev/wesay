using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Palaso.WritingSystems.Collation;
using Spart;
using WeSay.Data;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class WritingSystemSort : UserControl
	{
		private WritingSystem _writingSystem;
		private Color validBackgroundColor;
		private Color invalidBackgroundColor;

		public WritingSystemSort()
		{
			InitializeComponent();
			List<CultureInfo> result = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));

			result.Sort(
					delegate(CultureInfo ci1, CultureInfo ci2) { return StringComparer.InvariantCulture.Compare(ci1.DisplayName, ci2.DisplayName); });

			result.Remove(CultureInfo.InvariantCulture);

			List<KeyValuePair<string,string>> sortChoices = new List<KeyValuePair<string, string>>();
			sortChoices.Add(new KeyValuePair<string, string>(null, "(select a sort method)"));

			foreach (Enum customSortRulesType in Enum.GetValues(typeof(CustomSortRulesType)))
			{
				FieldInfo fi = customSortRulesType.GetType().GetField(customSortRulesType.ToString());

				DescriptionAttribute[] descriptions = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
				string description;
				if(descriptions.Length == 0)
				{
					description = customSortRulesType.ToString();
				}
				else
				{
					description = descriptions[0].Description;
				}
				sortChoices.Add(new KeyValuePair<string, string>(customSortRulesType.ToString(), description));
			}
			foreach (CultureInfo cultureInfo in result)
			{
				sortChoices.Add(new KeyValuePair<string, string>(cultureInfo.IetfLanguageTag, cultureInfo.DisplayName));
			}

			comboBoxCultures.DataSource = sortChoices;
			comboBoxCultures.DisplayMember = "Value";
			comboBoxCultures.ValueMember = "Key";

			comboBoxCultures.SelectedValue = string.Empty;
			comboBoxCultures.SelectedIndexChanged += new EventHandler(comboBoxCultures_SelectedIndexChanged);

			textBoxCustomRules.Validating += new CancelEventHandler(textBoxCustomRules_Validating);
			textBoxCustomRules.Validated += new EventHandler(textBoxCustomRules_Validated);

			validBackgroundColor = textBoxCustomRules.BackColor;
			invalidBackgroundColor = Color.Tomato;

			WeSayWordsProject.Project.EditorsSaveNow += OnEditorSaveNow;

		}

		private void OnEditorSaveNow(object sender, EventArgs e)
		{
			this.Validate(); // this will make it save
		}

		void textBoxCustomRules_Validating(object sender, CancelEventArgs cancelEventArgs)
		{
			ValidateCustomRules();
		}

		private void ValidateCustomRules() {
			CustomSortRulesType customSortRulesType = (CustomSortRulesType)Enum.Parse(typeof(CustomSortRulesType), this._writingSystem.SortUsing);
			string errorMessage;
			if(AreCustomRulesValid(this.textBoxCustomRules.Text, customSortRulesType, out errorMessage))
			{
				this.textBoxCustomRules.BackColor = this.validBackgroundColor;
				this.toolTip1.Active = false;
			}
			else
			{
				this.textBoxCustomRules.BackColor = this.invalidBackgroundColor;
				this.toolTip1.SetToolTip(textBoxCustomRules, errorMessage);
				this.toolTip1.Active = true;
				this.toolTip1.ShowAlways = true;
			}
		}

		private void textBoxCustomRules_Validated(object sender, EventArgs e)
		{
			_writingSystem.CustomSortRules = textBoxCustomRules.Text.Replace(Environment.NewLine, "\n");

			InvalidateCache();
		}

		private static void InvalidateCache() {
			DateTime liftLastWriteTimeUtc = File.GetLastWriteTimeUtc(WeSayWordsProject.Project.PathToLiftFile);
			if (File.Exists(WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
			{
				try // don't crash if we can't update
				{
					using (
							Db4oDataSource ds =
									new Db4oDataSource(
											WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
					{
						//this should be different now so the cache should be updated
						//but it shouldn't be off by enough to make it so we lose
						//records if a crash occured and updates hadn't been written
						//out yet and so are still pending in the db.
						CacheManager.UpdateSyncPointInCache(ds.Data,
															liftLastWriteTimeUtc.AddMilliseconds(10));
					}
				}
				catch {}
			}
		}

		private void comboBoxCultures_SelectedIndexChanged(object sender, EventArgs e)
		{
			_writingSystem.SortUsing = (string) comboBoxCultures.SelectedValue;
			UpdateCustomRules();
			InvalidateCache();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;
				if(_writingSystem!=null)
				{
					textBoxSortTest.Font = _writingSystem.Font;
					textBoxCustomRules.Font = _writingSystem.Font;
				}
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
			if (_writingSystem.UsesCustomSortRules)
			{
				if (_writingSystem.SortUsing == CustomSortRulesType.CustomSimple.ToString()  &&
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
					textBoxCustomRules.Text = textBoxCustomRules.Text.Replace("\r\n", Environment.NewLine);
					textBoxCustomRules.Text = textBoxCustomRules.Text.Replace("\n", Environment.NewLine);
				}
				else
				{
					textBoxCustomRules.Text = _writingSystem.CustomSortRules.Replace("\n", Environment.NewLine);
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

		static private bool AreCustomRulesValid(string customRules, CustomSortRulesType type, out string errorMessage)
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
			catch(DllNotFoundException e)
			{
				errorMessage = e.Message;
				return false;
			}
			catch (ParserErrorException e)
			{
				errorMessage = string.Format("{0} at line {1} column {2}", e.ParserError.ErrorText, e.ParserError.Line, e.ParserError.Column);
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
			string text = this.textBoxSortTest.Text;
			string[] stringsToSort = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			Array.Sort(stringsToSort, _writingSystem);
			string s = string.Empty;
			foreach (string s1 in stringsToSort)
			{
				s += s1 + Environment.NewLine;
			}
			this.textBoxSortTest.Text = s.Trim();
		}

		private void WritingSystemSort_Load(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
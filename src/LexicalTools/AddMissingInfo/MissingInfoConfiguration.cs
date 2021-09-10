using SIL.DictionaryServices.Model;
using SIL.Extensions;
using SIL.i18n;
using System;
using System.Collections.Generic;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public class MissingInfoConfiguration : TaskConfigurationBase, ITaskConfiguration, ICareThatWritingSystemIdChanged, ICareThatMeaningFieldChanged
	{
		private List<string> _fieldsToShow;
		private List<string> _fieldsReadOnly;
		private string _field;
		private List<string> _writingSystemsWeWantToFillIn;
		private List<string> _writingSystemsWhichAreRequired;

		public MissingInfoConfiguration(string configurationXml)//, ViewTemplate viewTemplate)
			: base(configurationXml)
		{
			_field = GetStringFromConfigNode("field");
			_fieldsToShow = GetStringFromConfigNode("showFields").SplitTrimmed(',');
			_fieldsReadOnly = GetStringFromConfigNode("readOnly", "").SplitTrimmed(',');
			_writingSystemsWeWantToFillIn = GetStringFromConfigNode("writingSystemsToMatch", string.Empty).SplitTrimmed(',');
			_writingSystemsWhichAreRequired = GetStringFromConfigNode("writingSystemsWhichAreRequired", string.Empty).SplitTrimmed(',');
		}

		public override string ToString()
		{
			return LongLabel;
		}

		public event EventHandler WritingSystemIdChanged;
		public event EventHandler WritingSystemIdDeleted;
		//public event EventHandler ChangeMeaningField;

		public void OnWritingSystemIdChanged(string from, string to)
		{
			int indexToReplace =
				_writingSystemsWeWantToFillIn.FindIndex(id => id.Equals(from, StringComparison.OrdinalIgnoreCase));
			if (indexToReplace != -1)
			{
				_writingSystemsWeWantToFillIn.RemoveAt(indexToReplace);
				//check whether the new id is already cinfigured for use. This can happen when writing systems are conflated
				if (_writingSystemsWeWantToFillIn.FindIndex((id => id.Equals(to, StringComparison.OrdinalIgnoreCase))) == -1)
				{
					_writingSystemsWeWantToFillIn.Add(to);
				}
			}

			indexToReplace =
				_writingSystemsWhichAreRequired.FindIndex(id => id.Equals(from, StringComparison.OrdinalIgnoreCase));
			if (indexToReplace != -1)
			{
				_writingSystemsWhichAreRequired.RemoveAt(indexToReplace);
				//check whether the new id is already cinfigured for use. This can happen when writing systems are conflated
				if (_writingSystemsWhichAreRequired.FindIndex((id => id.Equals(to, StringComparison.OrdinalIgnoreCase))) == -1)
				{
					_writingSystemsWhichAreRequired.Add(to);
				}
			}
			if (WritingSystemIdChanged != null)
			{
				WritingSystemIdChanged(this, new EventArgs());
			}
		}

		public void OnWritingSystemIdDeleted(string idToDelete)
		{
			int indexToReplace =
				_writingSystemsWeWantToFillIn.FindIndex(id => id.Equals(idToDelete, StringComparison.OrdinalIgnoreCase));
			if (indexToReplace != -1)
			{
				_writingSystemsWeWantToFillIn.RemoveAt(indexToReplace);
			}


			indexToReplace =
				_writingSystemsWhichAreRequired.FindIndex(id => id.Equals(idToDelete, StringComparison.OrdinalIgnoreCase));
			if (indexToReplace != -1)
			{
				_writingSystemsWhichAreRequired.RemoveAt(indexToReplace);
			}
			if (WritingSystemIdDeleted != null)
			{
				WritingSystemIdDeleted(this, new EventArgs());
			}
		}

		public void OnMeaningFieldChanged(string from, string to)
		{
			// if field contains the old meaning field then change it
			if (_field.Equals(from))
				_field = to;

			// if showfields contains the old meaning field then change it
			if (IncludesField(from))
			{
				SetInclusionOfField(from, false);
				SetInclusionOfField(to, true);
			}

			// if read-only contains the old meaning field then change it
			int index = _fieldsReadOnly.IndexOf(from);
			if (index != -1)
			{
				_fieldsReadOnly[index] = to;
			}
		}

		public DashboardGroup Group
		{
			get
			{
				if (IsBaseFormFillingTask)
				{
					return DashboardGroup.Refine;
				}
				return DashboardGroup.Describe;
			}
		}

		private bool IsBaseFormFillingTask
		{
			get { return FieldsToShowCommaSeparated.Contains(LexEntry.WellKnownProperties.BaseForm); }
		}

		protected override IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get
			{
				yield return new KeyValuePair<string, string>("label", Label);
				yield return new KeyValuePair<string, string>("longLabel", LongLabel);
				yield return new KeyValuePair<string, string>("description", Description);
				yield return new KeyValuePair<string, string>("field", MissingInfoFieldName);
				yield return new KeyValuePair<string, string>("showFields", FieldsToShowCommaSeparated);
				yield return new KeyValuePair<string, string>("readOnly", FieldsToShowReadOnlyCommaSeparated);
				yield return new KeyValuePair<string, string>("writingSystemsToMatch", WritingSystemsWeWantToFillInCommaSeparated);
				yield return new KeyValuePair<string, string>("writingSystemsWhichAreRequired", WritingSystemsWhichAreRequiredCommaSeparated);
			}
		}

		protected string WritingSystemsWhichAreRequiredCommaSeparated
		{
			get
			{
				string s = string.Empty;
				_writingSystemsWhichAreRequired.ForEach(f => s += ", " + f);
				return s.TrimStart(new char[] { ' ', ',' });
			}
		}

		private string WritingSystemsWeWantToFillInCommaSeparated
		{
			get
			{
				string s = string.Empty;
				_writingSystemsWeWantToFillIn.ForEach(f => s += ", " + f);
				return s.TrimStart(new char[] { ' ', ',' });
			}
		}


		public string Label
		{
			get { return GetStringFromConfigNode("label"); }
		}

		public string LongLabel
		{
			get { return "Add " + GetStringFromConfigNode("label"); }
		}

		public string Description
		{
			get { return GetStringFromConfigNode("description"); ; }
		}

		public string RemainingCountText
		{
			get { return StringCatalog.Get("~Entries without this:"); }
		}

		public string ReferenceCountText
		{
			get { return StringCatalog.Get("Total Entries:"); }
		}

		public bool IsPinned
		{
			get { return false; }
		}

		public bool AreEquivalent(ITaskConfiguration taskConfiguration)
		{
			var task = taskConfiguration as MissingInfoConfiguration;
			if (task == null)
				return false;

			if (task.TaskName != TaskName)
				return false;

			if (task.MissingInfoFieldName != MissingInfoFieldName)
			{
				// The field of "Add Meanings" will be definition or gloss depending what the meaning field is
				// the default config has it as definition so must be considered as equivalent to gloss
				if (task.MissingInfoFieldName == LexSense.WellKnownProperties.Definition)
				{
					if (MissingInfoFieldName != LexSense.WellKnownProperties.Gloss)
						return false;
				}
				else if (task.MissingInfoFieldName == LexSense.WellKnownProperties.Gloss)
				{
					if (MissingInfoFieldName != LexSense.WellKnownProperties.Definition)
						return false;
				}
				else
				{
					return false;
				}
			}

			if (task.Label != Label)
				return false;

			return true;
		}

		public string FieldsToShowReadOnlyCommaSeparated
		{
			get
			{
				return string.Join(", ", _fieldsReadOnly);
			}
		}

		public string FieldsToShowCommaSeparated //<showFields>
		{
			get
			{
				return string.Join(", ", _fieldsToShow);
			}
		}

		public void SetInclusionOfField(string fieldName, bool doInclude)
		{
			if (doInclude)
			{
				if (!_fieldsToShow.Contains(fieldName))
					_fieldsToShow.Add(fieldName);
			}
			else
			{
				if (_fieldsToShow.Contains(fieldName))
					_fieldsToShow.Remove(fieldName);
			}
		}

		public string MissingInfoFieldName //<field>
		{
			get { return _field; }
		}

		public string[] WritingSystemsWeWantToFillInArray
		{
			get { return _writingSystemsWeWantToFillIn.ToArray(); }
		}
		public string[] WritingSystemsWhichAreRequiredArray
		{
			get { return _writingSystemsWhichAreRequired.ToArray(); }
		}
		public IList<string> WritingSystemsWhichAreRequired
		{
			get { return _writingSystemsWhichAreRequired; }
		}
		public IList<string> WritingSystemsWeWantToFillIn
		{
			get { return _writingSystemsWeWantToFillIn; }
		}

		public static MissingInfoConfiguration CreateForTests(string missingInfoField,
			string label, string longLabel, string description, string remainingCountText,
			string referenceCountText, string fieldsToShow, string writingSystemsToMatch)
		{
			string x = String.Format(@"   <task taskName='AddMissingInfo' visible='true'>
					  <field>{0}</field>
					  <label>{1}</label>
					  <description>{2}</description>
					  <showFields>{3}</showFields>
					  <readOnly>{4}</readOnly>
					  <writingSystemsToMatch>{5}</writingSystemsToMatch>
					  <writingSystemsWhichAreRequired></writingSystemsWhichAreRequired>
					</task>
				", missingInfoField, label, description, fieldsToShow, fieldsToShow, writingSystemsToMatch);

			return new MissingInfoConfiguration(x);
		}


		public ViewTemplate CreateViewTemplate(ViewTemplate template)
		{
			return MissingInfoViewMaker.CreateViewTemplate(this, template);
		}

		public bool IncludesField(string fieldName)
		{
			return _fieldsToShow.Contains(fieldName);
		}

		public bool IncludesReadOnlyField(string fieldName)
		{
			return _fieldsReadOnly.Contains(fieldName);
		}
	}
}

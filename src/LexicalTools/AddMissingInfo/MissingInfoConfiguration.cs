using System;
using System.Collections.Generic;
using Palaso.Extensions;
using Palaso.i18n;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using System.Linq;
using Palaso.DictionaryServices.Model;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public class MissingInfoConfiguration : TaskConfigurationBase, ITaskConfiguration, ICareThatWritingSystemIdChanged
	{
		private List<string> _fieldsToShow;
		private List<string> _writingSystemsWeWantToFillIn;
		private List<string> _writingSystemsWhichAreRequired;

		public MissingInfoConfiguration(string configurationXml)//, ViewTemplate viewTemplate)
			: base(configurationXml)
		{
			_fieldsToShow = GetStringFromConfigNode("showFields").SplitTrimmed(',');
			_writingSystemsWeWantToFillIn = GetStringFromConfigNode("writingSystemsToMatch", string.Empty).SplitTrimmed(',');
			_writingSystemsWhichAreRequired = GetStringFromConfigNode("writingSystemsWhichAreRequired", string.Empty).SplitTrimmed(',');
		}

		public override string ToString()
		{
			return LongLabel;
		}

		public event EventHandler WritingSystemIdChanged;
		public event EventHandler WritingSystemIdDeleted;

		public void OnWritingSystemIdChanged(string from, string to)
		{
			int indexToReplace =
				_writingSystemsWeWantToFillIn.FindIndex(id => id.Equals(from, StringComparison.OrdinalIgnoreCase));
			if (indexToReplace != -1)
			{
				_writingSystemsWeWantToFillIn.RemoveAt(indexToReplace);
				_writingSystemsWeWantToFillIn.Add(to);
			}


			indexToReplace =
				_writingSystemsWhichAreRequired.FindIndex(id => id.Equals(from, StringComparison.OrdinalIgnoreCase));
			if (indexToReplace != -1)
			{
				_writingSystemsWhichAreRequired.RemoveAt(indexToReplace);
				_writingSystemsWhichAreRequired.Add(to);
			}
			if(WritingSystemIdChanged != null)
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
				yield return new KeyValuePair<string, string>("readOnly", FieldsToShowReadOnly);
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
			get { return "Add " + GetStringFromConfigNode("label") ; }
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
			if(task ==null)
				return false;

			if(task.TaskName != TaskName)
				return false;

			if(task.MissingInfoFieldName != MissingInfoFieldName)
				return false;

			if (task.Label != Label)
				return false;

			return true;
		}

		public string FieldsToShowReadOnly
		{
			get { return GetStringFromConfigNode("readOnly", string.Empty); ; }
		}

		public string FieldsToShowCommaSeparated //<showFields>
		{
			get
			{
				string s = string.Empty;
				_fieldsToShow.ForEach(f=>s+=", "+f);
				return s.TrimStart(new char[] {' ', ','});
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
			get { return GetStringFromConfigNode("field"); }
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
				", missingInfoField,label,description, fieldsToShow,fieldsToShow, writingSystemsToMatch);

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
	}
}

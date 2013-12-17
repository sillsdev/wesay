using System;
using System.Collections.Generic;
using WeSay.Project;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public class MissingInfoConfiguration : TaskConfigurationBase, ITaskConfiguration
	{
		public MissingInfoConfiguration(string  xml)
			: base(xml)
		{
		}



		public override string ToString()
		{
			return LongLabel;
		}

		protected override IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get
			{
				yield return new KeyValuePair<string, string>("label", Label);
				yield return new KeyValuePair<string, string>("longLabel", LongLabel);
				yield return new KeyValuePair<string, string>("description", Description);
				yield return new KeyValuePair<string, string>("field", MissingInfoField);
				yield return new KeyValuePair<string, string>("showFields", FieldsToShow);
				yield return new KeyValuePair<string, string>("readOnly", FieldsToShowReadOnly);
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
			get { return "Entries without this:"; }
		}

		public string ReferenceCountText
		{
			get { return "Total Entries:"; }
		}

		public bool IsPinned
		{
			get { return false; }
		}

		public string FieldsToShowReadOnly
		{
			get { return GetStringFromConfigNode("readOnly", string.Empty); ; }
		}

		public string FieldsToShow //<showFields>
		{
			get { return GetStringFromConfigNode("showFields"); ; }
		}

		public string MissingInfoField //<field>
		{
			get { return GetStringFromConfigNode("field"); }
		}

		public static MissingInfoConfiguration CreateForTests(string missingInfoField, string label, string longLabel, string description, string remainingCountText, string referenceCountText,  string fieldsToShow)
		{
			string x = String.Format(@"   <task taskName='AddMissingInfo' visible='true'>
					  <field>{0}</field>
					  <label>{1}</label>
					  <description>{2}</description>
					  <showFields>{3}</showFields>
					  <readOnly>{4}</readOnly>
					</task>
				", missingInfoField,label,description, fieldsToShow,fieldsToShow);

			 return new MissingInfoConfiguration(x);
		}
	}
}

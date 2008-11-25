using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Palaso.Reporting;
using WeSay.Project;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public class MissingInfoConfiguration : TaskConfigurationBase, ITaskConfiguration
	{
		public MissingInfoConfiguration(string  xml)
		{
			_xmlDoc = new XmlDocument();
			_xmlDoc.LoadXml(xml);
		}


		public override string ToString()
		{
			return LongLabel;
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
			get { return GetStringFromConfigNode("readOnly"); ; }
		}

		public string FieldsToShow //<showfields>
		{
			get { return GetStringFromConfigNode("showfields"); ; }
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
					  <showfields>{3}</showfields>
					  <readOnly>{4}</readOnly>
					</task>
				", missingInfoField,label,description, fieldsToShow,fieldsToShow);

			 return new MissingInfoConfiguration(x);
		}
	}
}

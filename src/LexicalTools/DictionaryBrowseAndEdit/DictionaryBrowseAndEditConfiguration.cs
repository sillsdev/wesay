using System;
using System.Collections.Generic;
using System.Xml;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Project;

namespace WeSay.LexicalTools.DictionaryBrowseAndEdit
{
	public class DictionaryBrowseAndEditConfiguration : TaskConfigurationBase, ITaskConfiguration
	{
		public DictionaryBrowseAndEditConfiguration(string xml)
			:base(xml)
		{
		}

		protected override IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get
			{
				yield break;
			}
		}

		public string Label
		{
			get
			{
				return StringCatalog.Get("Dictionary Browse && Edit",
				   "The label for the task that lets you see all entries, search for entries, and edit various fields.  We don't like the English name very much, so feel free to call this something very different in the language you are translating to.");
			}
		}

		public bool AreEquivalent(ITaskConfiguration taskConfiguration)
		{
			return taskConfiguration is DictionaryBrowseAndEditConfiguration;
		}

		public override string ToString()
		{
			return "Dictionary Browse & Edit";
		}

		public string LongLabel
		{
			get
			{
				return StringCatalog.Get("Dictionary Browse && Edit",
								"The long label for the task that lets you see all entries, search for entries, and edit various fields.  We don't like the English name very much, so feel free to call this something very different in the language you are translating to.");
			}
		}

		public string Description
		{
			get {
				return
					"This task allow and advanced user to see and edit all enabled fields of all entries in the dictionary.  He can add, delete, and search for entries.  NOTE: while this task may be very enticing to those familiar with traditional programs, this is *not* intended to be a primary way of working with WeSay."; }
		}

		public string RemainingCountText
		{
			get { return "todo"; }
		}

		public string ReferenceCountText
		{
			get { return "todo"; }
		}

		public bool IsPinned
		{
			get { return true; }
		}


		public static DictionaryBrowseAndEditConfiguration CreateForTests()
		{
			string x =
			   String.Format(
				   @"   <task taskName='Dictionary' visible='true'>  </task>");
			return new DictionaryBrowseAndEditConfiguration(x);
		}
	}
}

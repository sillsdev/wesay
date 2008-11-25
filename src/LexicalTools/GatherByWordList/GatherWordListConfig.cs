using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using WeSay.Project;

namespace WeSay.LexicalTools.GatherByWordList
{
	public interface IGatherWordListConfig : ITaskConfiguration
	{
		string wordListFileName
		{
			get;
		}
		string wordListWritingSystemId
		{
			get;
		}
	}


	public class GatherWordListConfig : TaskConfigurationBase, IGatherWordListConfig, ITaskConfiguration , ICareThatWritingSystemIdChanged
	{
		private readonly WordListCatalog _catalog;

		public GatherWordListConfig(string xml, WordListCatalog catalog)
		{
			_catalog = catalog;
			_xmlDoc = new XmlDocument();
			_xmlDoc.LoadXml(xml);
		}


		public string wordListFileName
		{
			get
			{
				return GetStringFromConfigNode("wordListFileName");
			}
		}
		public string wordListWritingSystemId
		{
			get
			{
				return GetStringFromConfigNode("wordListWritingSystemId");
			}
		}


		public override string ToString()
		{
			return LongLabel;
		}

		private WordListDescription WordList
		{
			get { return _catalog[wordListFileName]; }
		}

		public string Label
		{
			get { return WordList.Label; }
		}

		public string LongLabel
		{
			get { return WordList.LongLabel; }
		}

		public string Description
		{
			get { return WordList.Description; }
		}

		public string RemainingCountText
		{
			get { return "Remaining Words:"; }
		}

		public string ReferenceCountText
		{
			get { return string.Empty; }
		}

		public bool IsPinned
		{
			get { return false; }
		}



		public bool IsOptional
		{
			get { throw new System.NotImplementedException(); }
		}

		public static IGatherWordListConfig CreateForTests(string wordListFileName, string wordListWritingSystemId)
		{
			string x = String.Format(@"   <task taskName='AddMissingInfo' visible='true'>
					  <wordListFileName>{0}</wordListFileName>
					  <wordListWritingSystemId>{1}</wordListWritingSystemId>
					</task>
				", wordListFileName, wordListWritingSystemId);

			return new GatherWordListConfig(x, new WordListCatalog());

		}



		public void WritingSystemIdChanged(string from, string to)
		{
			  //TODO, when we become writeable
		}
	}
}
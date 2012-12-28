using System;
using System.Collections.Generic;
using Palaso.i18n;
using WeSay.Project;

namespace WeSay.LexicalTools.GatherBySemanticDomains
{


	public class GatherBySemanticDomainConfig : TaskConfigurationBase, ITaskConfiguration
	{

		/// <summary>
		/// Allow user to enter a meaning for each word as it is gathered (this is a little controversial)
		/// </summary>
		public bool ShowMeaningField{get;set;}

		public GatherBySemanticDomainConfig(string xml)
			: base(xml)
		{
			ShowMeaningField = bool.Parse(GetStringFromConfigNode("showMeaningField", "false"));
		}


		public static GatherBySemanticDomainConfig CreateForTests(string semanticDomainsQuestionFileName)
		{
			string x =
				String.Format(
					@"   <task taskName='GatherWordsBySemanticDomains' visible='true'>
	  <semanticDomainsQuestionFileName>{0}</semanticDomainsQuestionFileName>
	</task>",
					semanticDomainsQuestionFileName);
			return new GatherBySemanticDomainConfig(x);
		}

		public string semanticDomainsQuestionFileName
		{
			get
			{
				return GetStringFromConfigNode("semanticDomainsQuestionFileName", "Ddp4Questions-en.xml");
			}
		}

		public bool AreEquivalent(ITaskConfiguration taskConfiguration)
		{
			return taskConfiguration is GatherBySemanticDomainConfig;
		}



		public override string ToString()
		{
			return LongLabel;
		}

		public string Label
		{
			get { return StringCatalog.Get("Semantic Domains"); }
		}

		public string LongLabel
		{
			get { return StringCatalog.Get("Gather Words By Semantic Domain"); }
		}

		public string Description
		{
			get { return StringCatalog.Get("Collect new words organized by semantic domains and questions about those domains."); }
		}

		public string RemainingCountText
		{
			get { return StringCatalog.Get("Domains without words"); }
		}

		public string ReferenceCountText
		{
			get { return StringCatalog.Get("Total domains:"); }
		}

		public bool IsPinned
		{
			get { return false; }
		}


		protected override IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get
			{
				yield return new KeyValuePair<string, string>("semanticDomainsQuestionFileName", semanticDomainsQuestionFileName);
				yield return new KeyValuePair<string, string>("showMeaningField", ShowMeaningField.ToString());
			}
		}
	}
}

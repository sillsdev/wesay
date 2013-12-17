using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public interface IGatherBySemanticDomainsConfig : ITaskConfiguration
	{
		string semanticDomainsQuestionFileName
		{
			get;
		}
	}

	public class GatherBySemanticDomainConfig : TaskConfigurationBase, IGatherBySemanticDomainsConfig
	{
		public GatherBySemanticDomainConfig(string xml)
			: base(xml)
		{
		}


		public static GatherBySemanticDomainConfig CreateForTests(string semanticDomainsQuestionFileName)
		{
			string x =
				String.Format(
					@"   <task taskName='AddMissingInfo' visible='true'>
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


		public override string ToString()
		{
			return LongLabel;
		}

		public string Label
		{
			get { return "Semantic Domains"; }
		}

		public string LongLabel
		{
			get { return "Gather Words By Semantic Domain"; }
		}

		public string Description
		{
			get { return "Collect new words organized by semantic domains and questions about those domains."; }
		}

		public string RemainingCountText
		{
			get { return "Domains without words"; }
		}

		public string ReferenceCountText
		{
			get { return "Total domains:"; }
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
			}
		}
	}
}

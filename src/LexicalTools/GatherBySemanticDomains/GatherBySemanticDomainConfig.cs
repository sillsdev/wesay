using System.Xml.XPath;

namespace WeSay.LexicalTools
{
	public interface IGatherBySemanticDomainsConfig
	{
		string semanticDomainsQuestionFileName
		{
			get;
		}
	}
	public class GatherBySemanticDomainConfig
	{
		public static IGatherBySemanticDomainsConfig Create(string ddpFileName)
		{
			return new GatherBySemanticDomainsConfigMembers(ddpFileName);
		}

		public static IGatherBySemanticDomainsConfig Create(XPathNavigator xmlParameterElements)
		{
			return new GatherBySemanticDomainConfigXml(xmlParameterElements);
		}



		/// <summary>
		/// for use by unit tests
		/// </summary>
		private class GatherBySemanticDomainsConfigMembers : IGatherBySemanticDomainsConfig
		{
			private readonly string _fileName;

			public GatherBySemanticDomainsConfigMembers(string fileName)
			{
				_fileName = fileName;
			}

			public string semanticDomainsQuestionFileName
			{
				get { return _fileName; }
			}
		}
	}
	public class GatherBySemanticDomainConfigXml : IGatherBySemanticDomainsConfig
	{
		private readonly XPathNavigator _nodeNavigator;

		public GatherBySemanticDomainConfigXml(XPathNavigator nodeNavigator)
		{
			_nodeNavigator = nodeNavigator;
		}

		public string semanticDomainsQuestionFileName
		{
			get
			{
				var name=  _nodeNavigator.SelectSingleNode("semanticDomainsQuestionFileName");
				if (name == null || string.IsNullOrEmpty(name.Value))
				{

					return "Ddp4Questions-en.xml";
				}
				return name.Value;
			}
		}
	}
}

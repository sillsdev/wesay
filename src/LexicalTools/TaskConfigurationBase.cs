using System.Xml;
using Palaso.Reporting;

namespace WeSay.LexicalTools
{
	public class TaskConfigurationBase
	{
		protected XmlDocument _xmlDoc;


		public string TaskName
		{
			get { return WeSay.Foundation.XmlUtils.GetManditoryAttributeValue(_xmlDoc.FirstChild, "taskName"); }
		}

		public virtual bool IsOptional
		{
			get{ return true;}
		}

		public bool IsVisible
		{
			set { //todo
			}
			get { return WeSay.Foundation.XmlUtils.GetOptionalBooleanAttributeValue(_xmlDoc.FirstChild, "visible", false); }
		}

		protected string GetStringFromConfigNode(string elementName)
		{
			var name = _xmlDoc.SelectSingleNode("task/"+elementName);
			if (name == null || string.IsNullOrEmpty(name.InnerText))
			{

				throw new ConfigurationException("Missing the element '{0}'", elementName);
			}
			return name.InnerText;
		}

		protected string GetStringFromConfigNode(string elementName, string defaultValue)
		{
			var name = _xmlDoc.SelectSingleNode("task/" + elementName);
			if (name == null || string.IsNullOrEmpty(name.InnerText))
			{

				return defaultValue;
			}
			return name.InnerText;
		}
	}
}
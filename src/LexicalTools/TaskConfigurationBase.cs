using SIL.Reporting;
using SIL.Xml;
using System.Collections.Generic;
using System.Xml;

namespace WeSay.LexicalTools
{
	public abstract class TaskConfigurationBase
	{
		protected XmlDocument _xmlDoc;
		public bool IsVisible { get; set; }

		public TaskConfigurationBase(string xml)
		{
			_xmlDoc = new XmlDocument();
			_xmlDoc.LoadXml(xml);

			IsVisible = XmlUtils.GetOptionalBooleanAttributeValue(_xmlDoc.FirstChild, "visible", false);
		}
		public string TaskName
		{
			get { return XmlUtils.GetManditoryAttributeValue(_xmlDoc.FirstChild, "taskName"); }
		}

		public virtual bool IsOptional
		{
			get { return true; }
		}

		/// <summary>
		/// used to head-off failed task loads due to contstructors which can't even be satified by the container
		/// </summary>
		public virtual bool IsAvailable
		{
			get { return true; }
		}


		protected string GetStringFromConfigNode(string elementName)
		{
			var name = _xmlDoc.SelectSingleNode("task/" + elementName);
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

		protected abstract IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get;
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("task");
			writer.WriteAttributeString("taskName", TaskName);
			writer.WriteAttributeString("visible", IsVisible ? "true" : "false");
			foreach (var pair in ValuesToSave)
			{
				writer.WriteElementString(pair.Key, pair.Value);
			}
			writer.WriteEndElement();
		}
	}
}
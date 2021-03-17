using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using Autofac;
using Autofac.Core;
using SIL.WritingSystems;

namespace WeSay.Project
{
	public class ConfigFileReader
	{
		private readonly string _currentXmlConfiguration;
		private readonly string _defaultXmlConfiguration;
		private readonly TaskTypeCatalog _taskTypes;

		public ConfigFileReader(string currentXmlConfiguration, string defaultXmlConfiguration, TaskTypeCatalog taskTypes)
		{
			_currentXmlConfiguration = currentXmlConfiguration;
			_defaultXmlConfiguration = defaultXmlConfiguration;
			_taskTypes = taskTypes;
		}

		public IEnumerable<ITaskConfiguration> GetTasksConfigurations(IComponentContext context)
		{
			var configs = new List<ITaskConfiguration>();
			GetTasks(_currentXmlConfiguration, context, configs);
			GetTasks(_defaultXmlConfiguration, context, configs);
			return configs;
		}

		private void GetTasks(string xml, IComponentContext context, List<ITaskConfiguration> configs)
		{
			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskListNodeIterator = navigator.Select("configuration/tasks/task");
			foreach (XPathNavigator taskNode in taskListNodeIterator)
			{
				ITaskConfiguration configuration = CreateTaskConfiguration(context,_taskTypes, taskNode);
				if (configuration != null)
				{
					if(!configs.Exists(t=> t.AreEquivalent(configuration)))
					{
						configs.Add(configuration);
					}
				}
			}
		}


		private ITaskConfiguration CreateTaskConfiguration(IComponentContext context, TaskTypeCatalog taskTypes, XPathNavigator taskNode)
		{
			string taskName = taskNode.GetAttribute("taskName", string.Empty);

			if (taskTypes.TaskNameToConfigType.ContainsKey(taskName)) //has a config class
			{
				//make the task configuration class, using the xml we're looking at
				return context.ResolveNamed<ITaskConfiguration>(taskName + "Config",// using this service name
												   new Parameter[] { new TypedParameter(typeof(string), taskNode.OuterXml) });
			}
			return null;
		}
/*
		private List<Parameter> GetParameters(XPathNavigator component)
		{
			List<Parameter> parameters = new List<Parameter>();

			if (component.HasChildren)
			{
				XPathNodeIterator children = component.SelectChildren(string.Empty, string.Empty);
				foreach (XPathNavigator child in children)
				{
					if (child.GetAttribute("UseInConstructor", string.Empty) == "false")
						continue;
					parameters.Add(GetSimpleParameter(child));
				}
			}
			return parameters;
		}

		private Parameter GetSimpleParameter(XPathNavigator child)
		{

			switch (child.GetAttribute("class", string.Empty))
			{
				case "":
					return new NamedParameter(child.Name, child.Value);
					break;
				case "string":
					return new NamedParameter(child.Name, child.Value);
					break;
				case "bool":
					return new NamedParameter(child.Name, child.ValueAsBoolean);
					break;
				case "DateTime":
					return new NamedParameter(child.Name, child.ValueAsDateTime);
					break;
				case "double":
					return new NamedParameter(child.Name, child.ValueAsDouble);
					break;
				case "int":
					return new NamedParameter(child.Name, child.ValueAsInt);
					break;
				case "long":
					return new NamedParameter(child.Name, child.ValueAsLong);
					break;
				default:
					throw new ConfigurationException("Didn't understand this type of paramter in the config file: '{0}'", child.GetAttribute("class", string.Empty));
					break;
			}
		}
		*/
		// review: this might belong in a nother file...
		public static IEnumerable<ViewTemplate> CreateViewTemplates(string xmlConfiguration, IWritingSystemRepository writingSystems)
		{
			XPathDocument doc = new XPathDocument(new StringReader(xmlConfiguration));

			XPathNavigator navigator = doc.CreateNavigator();
			navigator = navigator.SelectSingleNode("//components");
			if (navigator != null)
			{
				bool hasviewTemplate = false;

				// String.Empty fails on mono 2.4. See http://projects.palaso.org/issues/show/276
				XPathNodeIterator componentList = navigator.SelectChildren(
					"viewTemplate", string.Empty
				);
				ViewTemplate factoryTemplate = ViewTemplate.MakeMasterTemplate(writingSystems);

				foreach (XPathNavigator component in componentList)
				{
					Debug.Assert(component.Name == "viewTemplate");
					hasviewTemplate = true;
					ViewTemplate template = new ViewTemplate();
					template.LoadFromString(component.OuterXml);
					ViewTemplate.UpdateUserViewTemplate(factoryTemplate, template);
					yield return template;
				}
				Debug.Assert(hasviewTemplate,
							 "Currently, there must be at least 1 viewTemplate in the WeSayConfig file");
			}
		}

		public static WeSayDataFormat GetDataFormat(string xmlConfiguration)
		{
			XPathDocument doc = new XPathDocument(new StringReader(xmlConfiguration));
			XPathNavigator navigator = doc.CreateNavigator();
			navigator = navigator.SelectSingleNode("//components//dataFormat");
			if (navigator == null)
			{
				return WeSayDataFormat.Lift;
			}
			else
			{
				string format = navigator.Value;
				return (WeSayDataFormat)System.Enum.Parse(typeof(WeSayDataFormat), format);
			}
		}

	}
}

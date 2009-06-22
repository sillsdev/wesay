using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using Autofac;
using WeSay.Foundation;


namespace WeSay.Project
{
	public class ConfigFileReader
	{
		private readonly string _xmlConfiguration;
		private readonly TaskTypeCatalog _taskTypes;

		public ConfigFileReader(string xmlConfiguration, TaskTypeCatalog taskTypes)
		{
			_xmlConfiguration = xmlConfiguration;
			_taskTypes = taskTypes;
		}

		public IEnumerable<ITaskConfiguration> GetTasksConfigurations(IContext context)
		{
			XPathDocument doc = new XPathDocument(new StringReader(_xmlConfiguration));
			var configs = new List<ITaskConfiguration>();
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskListNodeIterator = navigator.Select("configuration/tasks/task");
			foreach (XPathNavigator taskNode in taskListNodeIterator)
			{
				ITaskConfiguration configuration = CreateTaskConfiguration(context,_taskTypes, taskNode);
				if (configuration != null)
				{
					configs.Add(configuration);
				}
			}
			return configs;
		}


		private ITaskConfiguration CreateTaskConfiguration(IContext context, TaskTypeCatalog taskTypes, XPathNavigator taskNode)
		{
			string taskName = taskNode.GetAttribute("taskName", string.Empty);

			if (taskTypes.TaskNameToConfigType.ContainsKey(taskName)) //has a config class
			{
				//make the task configuration class, using the xml we're looking at
				return context.Resolve<ITaskConfiguration>(taskName + "Config",// using this service name
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
		public static IEnumerable<ViewTemplate> CreateViewTemplates(string xmlConfiguration, WritingSystemCollection writingSystems)
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


	}
}

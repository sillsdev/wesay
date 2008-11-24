using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.XPath;
using Autofac;
using Autofac.Builder;
using Palaso.Reporting;

namespace WeSay.Project
{
	public class ConfigFileReader
	{
		private readonly string _xmlConfiguration;
		private readonly ContainerBuilder _autofacBuilder;
		private IContainer _container;
		private readonly Dictionary<string, Type> _taskNameToTaskType = new Dictionary<string, Type>();
		private readonly Dictionary<string, Type> _taskNameToConfigType = new Dictionary<string, Type>();

		public ConfigFileReader(string xmlConfiguration,
									  IContainer parentContainer)
		{
			_xmlConfiguration = xmlConfiguration;
			_autofacBuilder = new Autofac.Builder.ContainerBuilder();

			//todo: how to do this in a more automated way? Should we explorer a set of known dlls, looking for ITasks?
			//  that would be a good step towards unifying Actions and Tasks, as I (jh) think we should think about

			RegisterTask("Dashboard", "WeSay.CommonTools.Dash", null, "CommonTools");
			RegisterTask("Dictionary", "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryTask", "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryBrowseAndEditConfiguration", "LexicalTools");
			RegisterTask("GatherWordsBySemanticDomains", "WeSay.LexicalTools.GatherBySemanticDomainTask",
				"WeSay.LexicalTools.GatherBySemanticDomainConfig","LexicalTools");

			RegisterTask("AddMissingInfo", "WeSay.LexicalTools.AddMissingInfo.MissingInfoTask",
				"WeSay.LexicalTools.AddMissingInfo.MissingInfoConfiguration", "LexicalTools");

			RegisterTask("GatherWordList", "WeSay.LexicalTools.GatherWordListTask",
				 "WeSay.LexicalTools.GatherByWordList.GatherWordListConfig", "LexicalTools");


			//this is weird syntax but what it means is,
			// Make a new container that is a child of the project's container
			_container = parentContainer.CreateInnerContainer();
			_autofacBuilder.Build( _container);

		}

		private void RegisterTask(string name, string classPath, string configClassPath, string assembly)
		{
			// _taskNameToClassPath.Add(name, classPath);
			Type type = GetType(classPath, assembly);
			_taskNameToTaskType.Add(name, type);
			_autofacBuilder.Register(type).Named(name).FactoryScoped();

			//register the class that holds the configuration for this task
			if (!string.IsNullOrEmpty(configClassPath))
			{
				type = GetType(configClassPath, assembly);
				_taskNameToConfigType.Add(name, type);
				_autofacBuilder.Register(type).Named(name + "Config").FactoryScoped();
			}
		}

		private ITaskConfiguration CreateTaskConfiguration(XPathNavigator taskNode)
		{
			string taskName = taskNode.GetAttribute("taskName", string.Empty);

			if (_taskNameToConfigType.ContainsKey(taskName)) //has a config class
			{
				//make the task configuration class, using the xml we're looking at
				return _container.Resolve<ITaskConfiguration>(taskName + "Config",// using this service name
												   new Parameter[] { new TypedParameter(typeof(string), taskNode.OuterXml) });
			}
			return null;
		}

		public IEnumerable<ITaskConfiguration> GetTasksConfigurations()
		{
			XPathDocument doc = new XPathDocument(new StringReader(_xmlConfiguration));
			var configs = new List<ITaskConfiguration>();
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskListNodeIterator = navigator.Select("configuration/tasks/task");
			foreach (XPathNavigator taskNode in taskListNodeIterator)
			{
				configs.Add(CreateTaskConfiguration(taskNode));
			}
			return configs;
		}


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

		// review: this might belong in a nother file...
		public static IEnumerable<ViewTemplate> CreateViewTemplates(string xmlConfiguration)
		{
			XPathDocument doc = new XPathDocument(new StringReader(xmlConfiguration));

			XPathNavigator navigator = doc.CreateNavigator();
			navigator = navigator.SelectSingleNode("//components");
			if (navigator != null)
			{
				bool hasviewTemplate = false;
				XPathNodeIterator componentList = navigator.SelectChildren(string.Empty,
																		   string.Empty);
				foreach (XPathNavigator component in componentList)
				{
					Debug.Assert(component.Name == "viewTemplate");
					hasviewTemplate = true;
					ViewTemplate template = new ViewTemplate();
					template.LoadFromString(component.OuterXml);

					yield return template;
				}
				Debug.Assert(hasviewTemplate,
							 "Currently, there must be at least 1 viewTemplate in the WeSayConfig file");
			}
		}


		public static Type GetType(string className, string assembly)
		{
			return Type.GetType(className + "," + assembly, true);
		}
	}
}

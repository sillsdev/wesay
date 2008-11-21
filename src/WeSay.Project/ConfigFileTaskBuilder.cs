using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using Autofac;
using Autofac.Builder;
using Palaso.Reporting;

namespace WeSay.Project
{
	/// <summary>
	/// Reads an xml config stream and creates tasks from it.
	/// </summary>
	public class ConfigFileTaskBuilder: ITaskBuilder
	{
		private List<ITask> _tasks;
		private readonly ContainerBuilder _autofacBuilder;
		private IContainer _container;
		private readonly Dictionary<string, Type> _taskNameToTaskType = new Dictionary<string, Type>();
		private readonly Dictionary<string, Type> _taskNameToConfigType = new Dictionary<string, Type>();

		public ConfigFileTaskBuilder(Stream xmlConfiguration,
									  IContainer parentContainer)
		{
			_autofacBuilder = new Autofac.Builder.ContainerBuilder();

			//todo: how to do this in a more automated way? Should we explorer a set of known dlls, looking for ITasks?
			//  that would be a good step towards unifying Actions and Tasks, as I (jh) think we should think about

			RegisterTask("Dashboard", "WeSay.CommonTools.Dash", null, "CommonTools");
			RegisterTask("Dictionary", "WeSay.LexicalTools.DictionaryTask", null, "LexicalTools");
			RegisterTask("GatherWordsBySemanticDomains", "WeSay.LexicalTools.GatherBySemanticDomainTask",
				"WeSay.LexicalTools.GatherBySemanticDomainConfigXml","LexicalTools");

			RegisterTask("AddMissingInfo", "WeSay.LexicalTools.MissingInfoTask",
				null, "LexicalTools");

			RegisterTask("GatherWordList", "WeSay.LexicalTools.GatherWordListTask",
				 null, "LexicalTools");

			XPathDocument doc = new XPathDocument(xmlConfiguration);
			InitializeViewTemplate(doc);


			//this is weird syntax but what it means is,
			// Make a new container that is a child of the project's container
			_container = parentContainer.CreateInnerContainer();
			_autofacBuilder.Build( _container);


			BuildTasks(doc);
		}

		/// <summary>
		/// Process the top level components (only ViewTemplates, as of Nov 2008)
		/// </summary>
		/// <param name="doc"></param>
		private void InitializeViewTemplate(IXPathNavigable doc)
		{
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

					//_container.RegisterComponentInstance(template.Id, template);
					_autofacBuilder.Register(template);//.Named(template.Id);
				}
				Debug.Assert(hasviewTemplate,
							 "Currently, there must be at least 1 viewTemplate in the WeSayConfig file");
			}
		}

		private void BuildTasks(IXPathNavigable doc)
		{
			_tasks = new List<ITask>();
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskListNodeIterator = navigator.Select("configuration/tasks/task");
			foreach (XPathNavigator taskNode in taskListNodeIterator)
			{
				string isVisible = taskNode.GetAttribute("visible", string.Empty);
				if (!String.IsNullOrEmpty(isVisible) && isVisible == "false")
				{
						continue; // don't show it
				}
				_tasks.Add(CreateTask(taskNode) as ITask);
			}
		}

		private ITask CreateTask(XPathNavigator taskNode)
		{
			string taskName = "unknown";
			try
			{
				taskName = taskNode.GetAttribute("taskName", string.Empty);

				if (_taskNameToConfigType.ContainsKey(taskName)) //has a config class
				{
					//make the task configuration class, using the xml we're looking at
					object config = _container.Resolve(taskName + "Config",
													   new Parameter[]
														   {new TypedParameter(typeof (XPathNavigator), taskNode)});
					 //make the task itself, handing it this configuration object.
					//its other constructor arguments come "automatically" out of the container
					return _container.Resolve<ITask>(taskName, new Parameter[] { new NamedParameter("config", config) });
			   }
				else if(taskNode.HasChildren)//doesn't yet have a config class, but does have parameters
				{
					List<Parameter> parameters = GetParameters(taskNode);
					return _container.Resolve<ITask>(taskName, parameters.ToArray());
				}
				else
				{
					return _container.Resolve<ITask>(taskName);//no config for this task
				}

			}
			catch (Exception e)
			{
				string message = e.Message;
				while (e.InnerException != null) //the user will see this, so lets dive down to the actual cause
				{
					e = e.InnerException;
					message = e.Message;
				}
				return new FailedLoadTask(taskName, "", message);
			}
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

		public static Type GetType(string className, string assembly)
		{
			return Type.GetType(className + "," + assembly, true);
		}

		public IList<ITask> Tasks
		{
			get { return _tasks; }
		}
	}
}
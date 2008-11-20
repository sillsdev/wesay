using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using Autofac;
using Autofac.Builder;
//using PicoContainer;
//using PicoContainer.Defaults;
using Autofac.Registrars.Delegate;
using Palaso.Reporting;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	/*
	 * Ah, this beast.  This class reads the xml config file and creates task objects from it.
	 *
	 * The system here is currently a weird use of a dependency injection container, "PicoContainer".
	 * It's odd because the xml elements that define the task must be listed in the same order
	 * as the arguments of a constructor, rather than using the element names to match them up.
	 *
	 * Tasks can have simple arguments, like "label", but also complex ones that require building up
	 * another object first, such as a filter.
	 *
	 * It's obvious this needs attention (it was one of the first things written for WeSay).  Some ideas (jh nov 2008):
	 *
	 *       <entries ref="All Entries" />  This is the *only* set there is, and will ever be.  So access to the
	 *       LexEntryRepository shouldn't be specified in the config; it should just be pushed into the container
	 *       automatically, and left out of the xml.
	 *
	 *      It's bad to have these order dependent.  We should match the element name with a constructor parameter (danger: better never rename those parameters)
	 *
	 *       <viewTemplate ref="Default View Template" />  There is currently only one of these... could just remove it, or say that if you don't specify one, you get the default, again out of the container
	 *
	 *        <label UseInConstructor="false">Actions</label>  This is kinda weird... elements marked with this attr are for the configuration app only.
	 *        The config program has one way of dealing with these, and wesayapp has another (making tasks).  But we don't want, for example,
	 *        to load up a whole LexEntryRepository in the Config tool, just because we want to show the descriptions of the tasks.
	 *
	 *        We need to decide if, rather than having these two different ways of dealing with it, if we can't just have one.  Even
	 *        somehow go all the way to a TaskRepository?
	 *
	 *      We really need to do more with this, allowing people to customize tasks, even in simple ways (e.g., do you want a field to
	 *      enter a gloss in the Gather by semantic domain task?)
	 */
	public class ConfigFileTaskBuilder: ITaskBuilder, IDisposable
	{
		private readonly WeSayWordsProject _project;
		private bool _disposed;
		private List<ITask> _tasks;
		private readonly ContainerBuilder _autofacBuilder;
		private IContainer _container;

		public ConfigFileTaskBuilder(Stream xmlConfiguration,
									  WeSayWordsProject project,
									 ICurrentWorkTask currentWorkTask,
									 LexEntryRepository lexEntryRepository)
		{
			_project = project;

			_autofacBuilder = new Autofac.Builder.ContainerBuilder();
			_autofacBuilder.Register(project).ExternallyOwned();
			_autofacBuilder.Register(currentWorkTask);

			_autofacBuilder.Register(lexEntryRepository).ExternallyOwned();

			//todo: how to do this in a more automated way? Should we explorer a set of known dlls, looking for ITasks?
			//  that would be a good step towards unifying Actions and Tasks, as I (jh) think we should think about
			_autofacBuilder.Register(GetType("WeSay.CommonTools.Dash", "CommonTools"));
			_autofacBuilder.Register(GetType("WeSay.LexicalTools.DictionaryTask", "LexicalTools"));
			_autofacBuilder.Register(GetType("WeSay.LexicalTools.GatherWordListTask", "LexicalTools")).FactoryScoped();
			_autofacBuilder.Register(GetType("WeSay.LexicalTools.GatherBySemanticDomainTask", "LexicalTools")).FactoryScoped();
			_autofacBuilder.Register(GetType("WeSay.LexicalTools.MissingInfoTask", "LexicalTools")).FactoryScoped();

			XPathDocument doc = new XPathDocument(xmlConfiguration);
			InitializeViewTemplate(doc);
			_container = _autofacBuilder.Build();
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
					_autofacBuilder.Register(template).Named(template.Id);
				}
				Debug.Assert(hasviewTemplate,
							 "Currently, there must be at least 1 viewTemplate in the WeSayConfig file");
			}
		}

		private void BuildTasks(IXPathNavigable doc)
		{
			UserSettingsRepository userSettingsRepository = _project.Container.Resolve<UserSettingsRepository>();
			//            _autofacBuilder.Register(userSettingsRepository); //todo: when we achieve using the same container as the project, we can remove this

			_tasks = new List<ITask>();
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskListNodeIterator = navigator.Select("configuration/tasks/task");
			foreach (XPathNavigator taskNode in taskListNodeIterator)
			{
				string isVisible = taskNode.GetAttribute("visible", string.Empty);
				//otherwise, it's an older config format, just show all tasks

				if (!String.IsNullOrEmpty(isVisible))
				{
					if (isVisible == "false")
					{
						continue; // don't show it
					}
				}
				ITask iTask = CreateComponent(taskNode) as ITask;
				_tasks.Add(iTask);
			}
		}

		private object CreateComponent(XPathNavigator taskNode)
		{
			object component;//usually a task, but also filter
			string id = string.Empty;
			try
			{
				id = taskNode.GetAttribute("id", string.Empty);
				if (id.Length == 0)
				{
					id = Guid.NewGuid().ToString(); //review: is this right (came from pico implementation)
				}

				List<Parameter> parameters = GetParameters(taskNode);

				Type type = GetType(
					taskNode.GetAttribute("class", string.Empty),
					taskNode.GetAttribute("assembly",
										  string.Empty));
				component = _container.Resolve(type, parameters.ToArray());
			}
			catch (Exception e)
			{
				string message = e.Message;
				while (e.InnerException != null) //the user will see this, so lets dive down to the actual cause
				{
					e = e.InnerException;
					message = e.Message;
				}
				component = new FailedLoadTask(id, id, message);
			}
			return component;
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

					string componentRef = child.GetAttribute("ref", string.Empty);
					if (componentRef.Length > 0)
					{
						// don't bother treating these as named things... we should probably just remove those elements all together.
						if (new List<String>(new string[] { "All Entries", "Current Task Provider" })
							.Contains(componentRef))
						{
							continue;
						}

						//this hooks up a task requesting the "Default View Template" to the view template that says that its id
						object referedToComponent = _container.Resolve(componentRef);
						parameters.Add(new Autofac.TypedParameter(referedToComponent.GetType(), referedToComponent));
					}
					else
					{
						parameters.Add(GetSimpleParameter(child));
					}
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
					object component1 = CreateComponent(child);
					return new TypedParameter(component1.GetType(), component1);
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

		//TODO(JH): having a builder than needs to be kept around so it can be disposed of is all wrong.
		//either I want to change it to something like TaskList rather than ITaskBuilder, or
		//it needs to create some disposable object other than a IList<>.
		//The reason we need to be able to dispose of it is because we need some way to
		//dispose of things that it might create, such as a data source.
		//UPDATE: what we need to do is get the container not belonging to this class, but to
		//the project (which already has one).  Then it is the autofac container's job to handle
		//the lifetimes.  I haven't worked out how to do this yet, though.

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					//without this, pico disposes of the project, which we don't really own
				  //  _container.UnregisterComponent("Project");
					//without this, pico disposes of the db, which we don't really own
				   // _container.UnregisterComponent("All Entries");
					try
					{
						_container.Dispose();
					}
					catch (Exception e)
					{
						if (e.Message.StartsWith("Either do the specified parameters not match any of"))
						{
							// mismatched parameters, don't worry
						}
						else
						{
							throw; //rethrow
						}
					}
					_container = null;
					GC.SuppressFinalize(this);
				}
			}
			_disposed = true;
		}
	}
}
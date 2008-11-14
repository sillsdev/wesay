using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.XPath;
using PicoContainer;
using PicoContainer.Defaults;
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
		private bool _disposed;
		private IMutablePicoContainer _picoContext;
		private List<ITask> _tasks;

		public ConfigFileTaskBuilder(Stream config,
									 WeSayWordsProject project,
									 ICurrentWorkTask currentWorkTask,
									 LexEntryRepository lexEntryRepository)
		{
			_picoContext = new DefaultPicoContainer();

			_picoContext.RegisterComponentInstance("Project", project);
			_picoContext.RegisterComponentInstance("Current Task Provider", currentWorkTask);

			_picoContext.RegisterComponentInstance("All Entries", lexEntryRepository);
			XPathDocument doc = new XPathDocument(config);
			InitializeComponents(doc);
			InitializeTaskList(doc);
		}

		private void InitializeComponents(IXPathNavigable doc)
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
					_picoContext.RegisterComponentInstance(template.Id, template);
				}
				Debug.Assert(hasviewTemplate,
							 "Currently, there must be at least 1 viewTemplate in the WeSayConfig file");
			}
		}

		private void InitializeTaskList(IXPathNavigable doc)
		{
			_tasks = new List<ITask>();
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskList = navigator.Select("configuration/tasks/task");
			foreach (XPathNavigator task in taskList)
			{
				//typical errors here:

				//PicoInitializationException("Either do the specified parameters not match any of....
				//may mean you have an extra (unused), missing, or out-of-order parameter element in the xml.

				//UnsatisfiableDependenciesException:
				// (NB: the "unsatisfiableDependencyTypes" had two types but there was only one problem)
				// Looking in the stack to this point, we see the id of the thing being created.
				//Use that id to figure out which class was being named, and considering the "unsatisfiableDependencyTypes"
				// from near the throw, see if some id
				// used in the config xml for the id'd type doesn't match the target (an unsatisfiableDependencyType),
				// defined elsewhere in the xml.
				// E.g., the template says:     <id>Default View BLAH</id>
				//  but the EntryDetailTask is lookinig for: <viewTemplate ref="Default View Template" />

				string isVisible = task.GetAttribute("visible", string.Empty);
				//otherwise, it's an older config format, just show all tasks

				if (!String.IsNullOrEmpty(isVisible))
				{
					if (isVisible == "false")
					{
						continue; // don't show it
					}
				}
				ITask iTask;
				string id = string.Empty;
				try
				{
					id = RegisterComponent(task);
					iTask = (ITask) _picoContext.GetComponentInstance(id);
				}
				catch (Exception e)
				{
					string message;
					if (e.Message.StartsWith("Either do the specified parameters not match any of"))
					{
						message = "The parameters given in " + id + " (" + task.InnerXml +
								  ") do not match those required by " +
								  task.GetAttribute("class", string.Empty);
					}
					else
					{
						message = e.Message;
						while (e.InnerException != null)
								//the user will see this, so lets dive down to the actual cause
						{
							e = e.InnerException;
							message = e.Message;
						}
					}
					iTask = new FailedLoadTask(id, id, message);
				}
				_tasks.Add(iTask);
			}
		}

		private string RegisterComponent(XPathNavigator component)
		{
			string id = component.GetAttribute("id", string.Empty);
			if (_picoContext.GetComponentInstance(id) != null)
			{
				throw new ApplicationException("The id '" + id + "' already exists (" +
											   component.OuterXml + ")");
			}
			if (id.Length == 0)
			{
				id = Guid.NewGuid().ToString();
			}

			if (component.HasChildren)
			{
				List<IParameter> parameters = new List<IParameter>();
				XPathNodeIterator children = component.SelectChildren(string.Empty, string.Empty);
				foreach (XPathNavigator child in children)
				{
					if (child.GetAttribute("UseInConstructor", string.Empty) != "false")
					{
						IParameter parameter;
						string componentRef = child.GetAttribute("ref", string.Empty);
						if (componentRef.Length > 0)
						{
							parameter = new ComponentParameter(componentRef);
						}
						else
						{
							switch (child.GetAttribute("class", string.Empty))
							{
								case "":
									parameter = new ConstantParameter(child.Value);
									break;
								case "string":
									parameter = new ConstantParameter(child.Value);
									break;
								case "bool":
									parameter = new ConstantParameter(child.ValueAsBoolean);
									break;
								case "DateTime":
									parameter = new ConstantParameter(child.ValueAsDateTime);
									break;
								case "double":
									parameter = new ConstantParameter(child.ValueAsDouble);
									break;
								case "int":
									parameter = new ConstantParameter(child.ValueAsInt);
									break;
								case "long":
									parameter = new ConstantParameter(child.ValueAsLong);
									break;
								default:
									parameter = new ComponentParameter(RegisterComponent(child));
									break;
							}
						}
						parameters.Add(parameter);
					}
				}
				_picoContext.RegisterComponentImplementation(id,
															 GetType(
																	 component.GetAttribute(
																			 "class", string.Empty),
																	 component.GetAttribute(
																			 "assembly",
																			 string.Empty)),
															 parameters.ToArray());
			}
			else
			{
				_picoContext.RegisterComponentImplementation(id,
															 GetType(
																	 component.GetAttribute(
																			 "class", string.Empty),
																	 component.GetAttribute(
																			 "assembly",
																			 string.Empty)));
			}
			return id;
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
					_picoContext.UnregisterComponent("Project");
					//without this, pico disposes of the db, which we don't really own
					_picoContext.UnregisterComponent("All Entries");
					try
					{
						_picoContext.Dispose();
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
					_picoContext = null;
					GC.SuppressFinalize(this);
				}
			}
			_disposed = true;
		}
	}
}
using System;
using System.Collections;
using System.Diagnostics;
using PicoContainer;
using PicoContainer.Defaults;
using WeSay.Project;
using System.IO;
using System.Collections.Generic;
using WeSay.Data;
using System.Xml.XPath;

namespace WeSay.App
{
	public class ConfigFileTaskBuilder : ITaskBuilder, IDisposable
	{
		private bool _disposed;
		private IMutablePicoContainer _picoContext;
		List<ITask> _tasks;

		public ConfigFileTaskBuilder(Stream config, WeSayWordsProject project, ICurrentWorkTask currentWorkTask, IRecordListManager recordListManager)
		{
			_picoContext = new DefaultPicoContainer();
			_picoContext.RegisterComponentInstance("Project", project);
			_picoContext.RegisterComponentInstance("Current Task Provider", currentWorkTask);

			_picoContext.RegisterComponentInstance("All Entries", recordListManager);
			XPathDocument doc = new XPathDocument(config);
			InitializeComponents(doc);
			InitializeTaskList(doc);
		}

		private void InitializeComponents(XPathDocument doc)
		{
			XPathNavigator navigator = doc.CreateNavigator();
			navigator = navigator.SelectSingleNode("//components");
			if (navigator != null)
			{
				bool hasFieldInventory = false;
				XPathNodeIterator componentList = navigator.SelectChildren(string.Empty, string.Empty);
				foreach (XPathNavigator component in componentList)
				{
					Debug.Assert(component.Name == "fieldInventory");
					hasFieldInventory = true;
					FieldInventory inventory = new FieldInventory();
					inventory.LoadFromString(component.OuterXml);
					_picoContext.RegisterComponentInstance(inventory.Id, inventory);
				}
				Debug.Assert(hasFieldInventory, "Currently, there must be exactly 1 FieldInventory in the tasks.xml");
			}
		}

		private void InitializeTaskList(XPathDocument doc)
		{
			_tasks = new List<ITask>();
			XPathNavigator navigator = doc.CreateNavigator();
			XPathNodeIterator taskList = navigator.SelectDescendants("task", string.Empty, false);
			foreach (XPathNavigator task in taskList)
			{
				string id = RegisterComponent(task);

				//typical error here: PicoInitializationException("Either do the specified parameters not match any of....
				//may mean you have an extra (unused), missing, or out-of-order parameter element in the xml.
				ITask iTask = (ITask)_picoContext.GetComponentInstance(id);
				System.Diagnostics.Debug.Assert(iTask != null);
				_tasks.Add(iTask);
			}
		}

		private string RegisterComponent(XPathNavigator component)
		{
			string id = component.GetAttribute("id", string.Empty);
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
													GetType(component.GetAttribute("class", string.Empty),
															component.GetAttribute("assembly", string.Empty)),
													parameters.ToArray());
			}
			else
			{
				_picoContext.RegisterComponentImplementation(id,
													GetType(component.GetAttribute("class", string.Empty),
															component.GetAttribute("assembly", string.Empty)));
			}
			return id;
		}

		public static Type GetType(string className, string assembly){
			return Type.GetType(className + "," + assembly, true);
		}

		public IList<ITask> Tasks
		{
			get
			{
				return _tasks;
			}
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
			if (!this._disposed)
			{
				if (disposing)
				{
					_picoContext.Dispose();
					_picoContext = null;
					GC.SuppressFinalize(this);
				}
			}
			_disposed = true;

		}
	}
}

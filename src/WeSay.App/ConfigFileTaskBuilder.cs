using System;
using PicoContainer;
using PicoContainer.Defaults;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using WeSay.Data;
using System.Xml.XPath;

namespace WeSay.App
{
	public class ConfigFileTaskBuilder : ITaskBuilder, IDisposable
	{
		private bool _disposed;
		private IMutablePicoContainer _picoContext;
		List<ITask> _tasks;

		public ConfigFileTaskBuilder(WeSayWordsProject project, Stream config)
		{
			_picoContext = new DefaultPicoContainer();
			_picoContext.RegisterComponentInstance(project);
			IRecordListManager recordListManager;

			if (project.PathToWeSaySpecificFilesDirectory.IndexOf("PRETEND") > -1)
			{
				IBindingList entries = new PretendRecordList();
				recordListManager = new InMemoryRecordListManager();
				IRecordList<LexEntry> masterRecordList = recordListManager.Get<LexEntry>();
				foreach (LexEntry entry in entries)
				{
					masterRecordList.Add(entry);
				}
			}
			else
			{
				com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
				com.db4o.config.ObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(Language.LanguageForm));
				objectClass.ObjectField("_writingSystemId").Indexed(true);
				objectClass.ObjectField("_form").Indexed(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LexEntry));
				objectClass.ObjectField("_modifiedDate").Indexed(true);

				recordListManager = new Db4oRecordListManager(project.PathToLexicalModelDB);
			}
			_picoContext.RegisterComponentInstance("All Entries", recordListManager);
			InitializeTaskList(config);
		}

		private void InitializeTaskList(Stream config)
		{
			_tasks = new List<ITask>();
			XPathDocument doc = new XPathDocument(config);
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

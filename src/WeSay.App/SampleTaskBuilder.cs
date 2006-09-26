using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PicoContainer;
using PicoContainer.Defaults;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;

namespace WeSay.App
{
	delegate ITask TaskDelegate();

	public class SampleTaskBuilder : ITaskBuilder, IDisposable
	{
		private bool _disposed;
		private IMutablePicoContainer _parentPicoContext;
		BasilProject _project;

		public SampleTaskBuilder(WeSayWordsProject project)
		{
			_parentPicoContext = CreateContainer();
			_parentPicoContext.RegisterComponentInstance(project);
			_project = project;

			if (project.PathToWeSaySpecificFilesDirectory.IndexOf("PRETEND") > -1)
			{
				IBindingList pEntries = new PretendRecordList();
				_parentPicoContext.RegisterComponentInstance(pEntries);
			}
			else
			{
				com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
				com.db4o.config.ObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(Language.LanguageForm));
				objectClass.ObjectField("_writingSystemId").Indexed(true);
				objectClass.ObjectField("_form").Indexed(true);

				objectClass = db4oConfiguration.ObjectClass(typeof(LexEntry));
				objectClass.ObjectField("_modifiedDate").Indexed(true);


				Db4oDataSource ds = new Db4oDataSource(project.PathToLexicalModelDB);
				IComponentAdapter dsAdaptor = _parentPicoContext.RegisterComponentInstance(ds);

				///* Because the data source is never actually touched by the normal pico container code,
				// * it never gets  added to this ordered list.  The ordered list is used for the lifecycle
				// * functions, such as dispose.  Without adding it explicitly, this will end up
				// * getting disposed of first, whereas we need it to be disposed of last.
				// * Adding it explicity to the ordered list gives proper disposal order.
				// */
				_parentPicoContext.AddOrderedComponentAdapter(dsAdaptor);

				Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds);
				_parentPicoContext.RegisterComponentInstance("All Entries", entries);
			}

		}


		public IList<ITask> Tasks
		{
			get
			{
				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool("WeSay.CommonTools.DashboardControl,CommonTools"));

				tools.Add(new TaskProxy("Words", delegate
					{
						return CreateTool("WeSay.LexicalTools.EntryDetailTask,LexicalTools") ;
					}));


				tools.Add(new TaskProxy("Add Meanings", delegate
				{
					return CreateLexFieldTask("AddMeanings", "WeSay.LexicalTools.LexFieldTask,LexicalTools",
								"Add Meanings", "Add glosses to entries when missing.", "GhostGloss Gloss");
				}));


				tools.Add(CreatePictureTask("CollectWords", "WeSay.CommonTools.PictureControl,CommonTools",
					"Collect Words", "Collect words using words in another language.", "RealWord.gif"));
				tools.Add(CreatePictureTask("SemDom", "WeSay.CommonTools.PictureControl,CommonTools",
					"Semantic Domains", "Collect words using semantic domains.", "SemDom.gif"));
				//tools.Add(CreateTool("WeSay.CommonTools.PictureControl,CommonTools",
				//    CreatePictureConfiguration("Semantic Domains", "SemDom.gif")));
				return tools;
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
					_parentPicoContext.Dispose();
					_parentPicoContext = null;
					GC.SuppressFinalize(this);
				}
			}
			_disposed = true;

		}

		//private IMutablePicoContainer NewChildContainer(IList instances)
		//{
		//    IMutablePicoContainer child = new DefaultPicoContainer(_parentPicoContext);
		//    _parentPicoContext.AddChildContainer(child);
		//    if (instances != null)
		//    {
		//        foreach (object instance in instances)
		//        {
		//            //REVIEW: Huh? This is registering every LexEntry.
		//            Debug.Assert(null == instance as LexicalModel.LexEntry,"Flying Saucer Siting");
		//            child.RegisterComponentInstance(instance);
		//        }
		//    }
		//    return child;
		//}

		private static ITask CreateTool(IMutablePicoContainer picoContext, string fullToolClass)
		{
			RegisterType(picoContext, fullToolClass);

			ITask i = (ITask)picoContext.GetComponentInstance(fullToolClass);
			Debug.Assert(i != null);
			return i;
		}

		private static void RegisterType(IMutablePicoContainer picoContext, string fullToolClass)
		{
			picoContext.RegisterComponentImplementation(fullToolClass, Type.GetType(fullToolClass, true));
		}

		//private ITask CreateTool(string fullToolClass, IList instances)
		//{
		//    return CreateTool(NewChildContainer(instances), fullToolClass);
		//}

		private ITask CreateTool(string fullToolClass)
		{
			return CreateTool(_parentPicoContext, fullToolClass);
		}

		private ITask CreatePictureTask(string id, string fullToolClass, string label, string description, string pictureFilePath)
		{
			_parentPicoContext.RegisterComponentImplementation(id, Type.GetType(fullToolClass, true),
				new IParameter[]{
					new ConstantParameter(label),
					new ConstantParameter(description),
					new ConstantParameter(pictureFilePath)
				});

			ITask i = (ITask)_parentPicoContext.GetComponentInstance(id);
			Debug.Assert(i != null);
			return i;
		}

		private ITask CreateLexFieldTask(string id, string fullToolClass, string label, string description, string fieldsToShow)
		{
			_parentPicoContext.RegisterComponentImplementation("GlossFilter", Type.GetType("WeSay.LexicalModel.MissingGlossFilter,LexicalModel", true),
				new IParameter[]{
					new ConstantParameter("en"),
				});

			_parentPicoContext.RegisterComponentImplementation(id, Type.GetType(fullToolClass, true),
				new IParameter[]{
					new ComponentParameter("All Entries"),
					new ComponentParameter("GlossFilter"),
					new ConstantParameter(label),
					new ConstantParameter(description),
					new ConstantParameter(fieldsToShow)
				});

			ITask i = (ITask)_parentPicoContext.GetComponentInstance(id);
			Debug.Assert(i != null);
			return i;
		}

		private static IMutablePicoContainer CreateContainer()
		{
			return new DefaultPicoContainer();
		}
	}


	class TaskProxy : ITask
	{
		private string _label;
		private TaskDelegate _makeTask;
		private ITask _realTask;

		public TaskProxy(string label, TaskDelegate makeTask)
		{
			_label = label;
			_makeTask = makeTask;
}

		#region ITask Members

		public void Activate()
		{
			RealTask.Activate();
		}


		public void Deactivate()
		{
			  RealTask.Deactivate();
	  }

		public string Label
		{
			get { return _label; }
		}

		public string Description
		{
			get
			{
				return RealTask.Description;
			}
		}

		public Control Control
		{
			get
			{
				return RealTask.Control;
			}
		}

		private ITask RealTask
		{
			get
			{
				if (_realTask == null)
				{
					_realTask = _makeTask();
				}
				return _realTask;
			}
		}

		#endregion
	}
}

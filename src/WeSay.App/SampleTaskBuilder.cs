using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
		private IMutablePicoContainer _picoContext;

		public SampleTaskBuilder(WeSayWordsProject project, ICurrentWorkTask currentWorkTask)
		{
			_picoContext = CreateContainer();
			_picoContext.RegisterComponentInstance("Project", project);
			_picoContext.RegisterComponentInstance("Current Task Provider", currentWorkTask);
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
			_picoContext.RegisterComponentInstance("Record List Manager", recordListManager);
		}


		public IList<ITask> Tasks
		{
			get
			{
				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool("WeSay.CommonTools.DashboardControl,CommonTools"));

				tools.Add(CreateTool("WeSay.LexicalTools.EntryDetailTask,LexicalTools"));


				tools.Add(CreateLexFieldTask("AddMeanings", "WeSay.LexicalTools.LexFieldTask,LexicalTools",
								"Add Meanings", "Add glosses to entries when missing.", "GhostGloss Gloss"));


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
					_picoContext.Dispose();
					_picoContext = null;
					GC.SuppressFinalize(this);
				}
			}
			_disposed = true;

		}

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

		private ITask CreateTool(string fullToolClass)
		{
			return CreateTool(_picoContext, fullToolClass);
		}

		private ITask CreatePictureTask(string id, string fullToolClass, string label, string description, string pictureFilePath)
		{
			_picoContext.RegisterComponentImplementation(id, Type.GetType(fullToolClass, true),
				new IParameter[]{
					new ConstantParameter(label),
					new ConstantParameter(description),
					new ConstantParameter(pictureFilePath)
				});

			ITask i = (ITask)_picoContext.GetComponentInstance(id);
			Debug.Assert(i != null);
			return i;
		}

		private ITask CreateLexFieldTask(string id, string fullToolClass, string label, string description, string fieldsToShow)
		{
			_picoContext.RegisterComponentImplementation("GlossFilter", Type.GetType("WeSay.LexicalModel.MissingGlossFilter,LexicalModel", true),
				new IParameter[]{
					new ConstantParameter("en"),
				});

			_picoContext.RegisterComponentImplementation(id, Type.GetType(fullToolClass, true),
				new IParameter[]{
					new ComponentParameter("Record List Manager"),
					new ComponentParameter("GlossFilter"),
					new ConstantParameter(label),
					new ConstantParameter(description),
					new ConstantParameter(fieldsToShow)
				});

			ITask i = (ITask)_picoContext.GetComponentInstance(id);
			Debug.Assert(i != null);
			return i;
		}

		private static IMutablePicoContainer CreateContainer()
		{
			return new DefaultPicoContainer();
		}
	}
}

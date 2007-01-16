using System;
using System.Collections.Generic;
using System.Diagnostics;
using PicoContainer;
using PicoContainer.Defaults;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.App
{
	delegate ITask TaskDelegate();

	public class SampleTaskBuilder : ITaskBuilder, IDisposable
	{
		private bool _disposed;
		private IMutablePicoContainer _picoContext;

		public SampleTaskBuilder(WeSayWordsProject project, ICurrentWorkTask currentWorkTask, IRecordListManager recordListManager)
		{
			_picoContext = CreateContainer();
			_picoContext.RegisterComponentInstance("Project", project);
			_picoContext.RegisterComponentInstance("Current Task Provider", currentWorkTask);
			_picoContext.RegisterComponentInstance("Record List Manager", recordListManager);

			string[] analysisWritingSystemIds = new string[] { project.WritingSystems.AnalysisWritingSystemDefaultId };
			string[] vernacularWritingSystemIds = new string[] {project.WritingSystems.VernacularWritingSystemDefaultId};
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss .ToString(), analysisWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), analysisWritingSystemIds));
			_picoContext.RegisterComponentInstance("Default Field Inventory", viewTemplate);
		}


		public IList<ITask> Tasks
		{
			get
			{
				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool("WeSay.CommonTools.DashboardControl,CommonTools"));

				tools.Add(CreateTool("WeSay.LexicalTools.EntryDetailTask,LexicalTools"));


				_picoContext.RegisterComponentImplementation("GlossFilter", Type.GetType("WeSay.LexicalTools.MissingGlossFilter,LexicalTools", true),
				new IParameter[]{
					new ComponentParameter("Default Field Inventory"),
				});

				tools.Add(CreateLexFieldTask("AddMeanings", "WeSay.LexicalTools.LexFieldTask,LexicalTools", "GlossFilter",
								"Add Meanings", "Add glosses to entries when missing.", "Gloss"));


				_picoContext.RegisterComponentImplementation("ExampleFilter", Type.GetType("WeSay.LexicalTools.MissingExampleSentenceFilter,LexicalTools", true),
				new IParameter[]{
					new ComponentParameter("Default Field Inventory"),
				});
				tools.Add(CreateLexFieldTask("AddExampleSentences", "WeSay.LexicalTools.LexFieldTask,LexicalTools", "ExampleFilter",
								"Add Examples", "Add example sentences to entries.", "Sentence"));

				tools.Add(CreatePictureTask("CollectWords", "WeSay.CommonTools.PictureControl,CommonTools",
					"Collect Words", "Collect words using words in another language.", "RealWord.gif"));
				tools.Add(CreatePictureTask("SemDom", "WeSay.CommonTools.PictureControl,CommonTools",
					"Semantic Domains", "Collect words by domains.", "SemDom.gif"));
				return tools;
			}
		}

		//TODO(JH): having a builder that needs to be kept around so it can be disposed of is all wrong.
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

		private ITask CreateLexFieldTask(string id, string fullToolClass, string filter, string label, string description, string fieldsToShow)
		{
			_picoContext.RegisterComponentImplementation(id, Type.GetType(fullToolClass, true),
				new IParameter[]{
					new ComponentParameter("Record List Manager"),
					new ComponentParameter(filter),
					new ConstantParameter(label),
					new ConstantParameter(description),
					new ComponentParameter("Default Field Inventory"),
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

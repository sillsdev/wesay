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

		public SampleTaskBuilder(BasilProject project)
		{
			_parentPicoContext = CreateContainer();
			_parentPicoContext.RegisterComponentInstance(project);
			_project = project;

			if (project.PathToLexicalModelDB.IndexOf("PRETEND") > -1)
			{
				IBindingList pEntries = new PretendRecordList();
				_parentPicoContext.RegisterComponentInstance(pEntries);
			}
			else
			{
				Db4oDataSource ds = new Db4oDataSource(project.PathToLexicalModelDB);
				IComponentAdapter dsAdaptor = _parentPicoContext.RegisterComponentInstance(ds);

				///* Because the data source is never actually touched by the normal pico container code,
				// * it never gets  added to this ordered list.  The ordered list is used for the lifecycle
				// * functions, such as dispose.  Without adding it explicitly, this will end up
				// * getting disposed of first, whereas we need it to be disposed of last.
				// * Adding it explicity to the ordered list gives proper disposal order.
				// */
				_parentPicoContext.AddOrderedComponentAdapter(dsAdaptor);

				Db4oBindingList<LexEntry> entries = new Db4oBindingList<LexEntry>(ds);
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
					return CreateTool("WeSay.LexicalTools.LexFieldTask,LexicalTools",
					CreateLexFieldConfiguration("Add Meanings", "Gloss GhostGloss"));
				}));



				tools.Add(CreateTool("WeSay.CommonTools.PictureControl,CommonTools",
					CreatePictureConfiguration("Collect Words", "RealWord.gif")));
				tools.Add(CreateTool("WeSay.CommonTools.PictureControl,CommonTools",
					CreatePictureConfiguration("Semantic Domains", "SemDom.gif")));
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

		private IMutablePicoContainer NewChildContainer(IList instances)
		{
			IMutablePicoContainer child = new DefaultPicoContainer(_parentPicoContext);
			_parentPicoContext.AddChildContainer(child);
			if (instances != null)
			{
				foreach (object instance in instances)
				{
					//REVIEW: Huh? This is registering every LexEntry.
					child.RegisterComponentInstance(instance);
				}
			}
			return child;
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


		private ITask CreateTool(string fullToolClass, IList instances)
		{
			return CreateTool(NewChildContainer(instances), fullToolClass);
		}

		private ITask CreateTool(string fullToolClass)
		{
			return CreateTool(_parentPicoContext, fullToolClass);
		}

		private static IList CreatePictureConfiguration(string label, string pictureFilePath)
		{
			IList instances = new List<object>();
			instances.Add(label);
			instances.Add(new Bitmap(pictureFilePath));

			return instances;
		}

		private IList CreateLexFieldConfiguration(string label, string fieldToShow)
		{
			IList instances = new List<object>();
			//Predicate<LexEntry> entryFilter = delegate(LexEntry entry)
			//            {
			//                if (entry.Senses.Count == 0)
			//                {
			//                    return true;
			//                }
			//                foreach (LexSense sense in entry.Senses)
			//                {
			//                    foreach (LanguageForm form in sense.Gloss)
			//                    {
			//                        if (form.WritingSystemId == _project.AnalysisWritingSystemDefault.Id &&
			//                           form.Form == string.Empty)
			//                        {
			//                            return true;
			//                        }
			//                    }
			//                }
			//                return false;
			//            };

			Db4oDataSource ds = (Db4oDataSource)_parentPicoContext.GetComponentInstance(typeof(Db4oDataSource));
			Db4oBindingList<LexEntry> entries = new Db4oBindingList<LexEntry>(ds/*, entryFilter*/);
			entries.SODAQuery = delegate(com.db4o.query.Query query)
									{
										query.Constrain(typeof(LexEntry));
										com.db4o.query.Query forms = query.Descend("_senses").Descend("_gloss").Descend("_forms");
										forms.Descend("_writingSystemId").Constrain(_project.AnalysisWritingSystemDefault.Id)
											.And(forms.Descend("_form").Constrain(string.Empty));

										return query;
									};

			instances.Add(entries);
			instances.Add(label);
			Predicate<string> fieldFilter = delegate(string s)
							{
								return fieldToShow.Contains(s);
							};
			instances.Add(fieldFilter);

			return instances;
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

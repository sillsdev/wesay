using System.Collections.Generic;
using Autofac;
using WeSay.Project;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public class TaskListPresentationModel
	{
		private readonly TaskCollection _taskCollection;
		public TaskListView View{ get; private set;}

		public TaskListPresentationModel(TaskListView view, TaskCollection taskCollection)
		{
			_taskCollection = taskCollection;
			View = view;
			View.Model = this;

			WeSayWordsProject.Project.WritingSystemChanged += OnProject_WritingSystemChanged;
		}

		public IEnumerable<ITaskConfiguration> Tasks
		{
			get { return _taskCollection; }
		}


		private void OnProject_WritingSystemChanged(object sender, WeSayWordsProject.StringPair pair)
		{
			foreach (object task in Tasks)
			{
				if (null == task as ICareThatWritingSystemIdChanged)
					continue;
				((ICareThatWritingSystemIdChanged)task).OnWritingSystemIdChanged(pair.from, pair.to);
			}
		}
	}

}

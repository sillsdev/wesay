using SIL.WritingSystems;
using System.Collections.Generic;
using System.Linq;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public class TaskListPresentationModel
	{
		private readonly TaskCollection _taskCollection;
		public TaskListView View { get; private set; }

		public TaskListPresentationModel(TaskListView view, TaskCollection taskCollection)
		{
			_taskCollection = taskCollection;
			View = view;
			View.Model = this;

			WeSayWordsProject.Project.WritingSystemChanged += OnProject_WritingSystemChanged;
			WeSayWordsProject.Project.WritingSystemDeleted += OnProject_WritingSystemDeleted;

			WeSayWordsProject.Project.MeaningFieldChanged += OnProject_MeaningFieldChanged;
		}

		private void OnProject_WritingSystemDeleted(object sender, WritingSystemDeletedEventArgs e)
		{
			foreach (var task in Tasks.OfType<ICareThatWritingSystemIdChanged>())
			{
				task.OnWritingSystemIdDeleted(e.Id);
			}
		}

		public IEnumerable<ITaskConfiguration> Tasks
		{
			get { return _taskCollection; }
		}


		private void OnProject_WritingSystemChanged(object sender, WeSayWordsProject.StringPair pair)
		{
			foreach (var task in Tasks.OfType<ICareThatWritingSystemIdChanged>())
			{
				task.OnWritingSystemIdChanged(pair.from, pair.to);
			}
		}

		private void OnProject_MeaningFieldChanged(object sender, WeSayWordsProject.StringPair pair)
		{
			foreach (var task in Tasks.OfType<ICareThatMeaningFieldChanged>())
			{
				task.OnMeaningFieldChanged(pair.from, pair.to);
			}
		}

		public bool DoShowTask(ITaskConfiguration task)
		{
			return task.Label != "Dashboard";
		}
	}

}

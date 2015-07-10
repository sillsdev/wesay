using System;
using System.Collections.Generic;
using Autofac;
using SIL.WritingSystems;
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
			WeSayWordsProject.Project.WritingSystemDeleted += OnProject_WritingSystemDeleted;
		}

		private void OnProject_WritingSystemDeleted(object sender, WritingSystemDeletedEventArgs e)
		{
			foreach (var task in ICareThatWritingSystemIdChangedTasks)
			{
				task.OnWritingSystemIdDeleted(e.Id);
			}
		}

		private IEnumerable<ICareThatWritingSystemIdChanged> ICareThatWritingSystemIdChangedTasks
		{
			get
			{
				foreach (object task in Tasks)
				{
					if (null == task as ICareThatWritingSystemIdChanged)
						continue;
					yield return ((ICareThatWritingSystemIdChanged) task);
				}
			}
		}

		public IEnumerable<ITaskConfiguration> Tasks
		{
			get { return _taskCollection; }
		}


		private void OnProject_WritingSystemChanged(object sender, WeSayWordsProject.StringPair pair)
		{
			foreach (var task in ICareThatWritingSystemIdChangedTasks)
			{
				task.OnWritingSystemIdChanged(pair.from, pair.to);
			}
		}

		public bool DoShowTask(ITaskConfiguration task)
		{
			return task.Label != "Dashboard";
		}
	}

}

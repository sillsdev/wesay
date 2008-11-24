using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Palaso.Reporting;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public class TaskListPresentationModel
	{
		private readonly ConfigFileReader _configFileReader;
		public IEnumerable<ITaskConfiguration> Tasks { get; private set;}
		public TaskListView View{ get; private set;}

		public TaskListPresentationModel(TaskListView view, ConfigFileReader configFileReader)
		{
			View = view;
			View.Model = this;

			_configFileReader = configFileReader;
		}

		public void Init()
		{
			LoadInventory();
			WeSayWordsProject.Project.EditorsSaveNow += Project_HackedEditorsSaveNow;
		}

		public void LoadInventory()
		{
			try
			{
				Tasks = _configFileReader.GetTasksConfigurations();

			 //TODO: clean this up
				WeSayWordsProject.Project.WritingSystemChanged += OnProject_WritingSystemChanged;
			}
			catch (Exception error)
			{
				throw new ApplicationException(
						String.Format("There was a problem reading {0}.  The error was: {1}",
									  WeSayWordsProject.Project.PathToDefaultConfig,
									  error.Message));
			}
		}


		private void Project_HackedEditorsSaveNow(object owriter, EventArgs e)
		{
			Debug.Fail("Don't know how to save yet.");
//            XmlWriter writer = (XmlWriter)owriter;
//
//            IList<ViewTemplate> viewTemplates = WeSayWordsProject.Project.ViewTemplates;
//            writer.WriteStartElement("components");
//            foreach (ViewTemplate template in viewTemplates)
//            {
//                template.Write(writer);
//            }
//            writer.WriteEndElement();
//
//            writer.WriteStartElement("tasks");
//            foreach (TaskInfo t in _taskList.Items)
//            {
//                t.IsVisible = _taskList.GetItemChecked(_taskList.Items.IndexOf(t));
//                t.Node.WriteTo(writer);
//            }
//            writer.WriteEndElement();
		}


		private void OnProject_WritingSystemChanged(object sender, WeSayWordsProject.StringPair pair)
		{
			foreach (ICareThatWritingSystemIdChanged task in Tasks)
			{
				task.WritingSystemIdChanged(pair.from, pair.to);
			}
		}

	}

}

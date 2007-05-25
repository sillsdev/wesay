using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class TaskListControl : UserControl
	{

		public TaskListControl()
		{
			InitializeComponent();
			splitContainer1.Resize += new EventHandler(splitContainer1_Resize);
		}

		void splitContainer1_Resize(object sender, EventArgs e)
		{
			//this is part of dealing with .net not adjusting stuff well for different dpis
			splitContainer1.Dock = DockStyle.None;
			splitContainer1.Width = this.Width - 25;
		}

		private void TaskList_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			LoadInventory();

			WeSayWordsProject.Project.HackedEditorsSaveNow += new EventHandler(Project_HackedEditorsSaveNow);
		}

		void Project_HackedEditorsSaveNow(object owriter, EventArgs e)
		{
			XmlWriter writer = (XmlWriter)owriter;

			IList<ViewTemplate> viewTemplates = WeSayWordsProject.Project.ViewTemplates;
			writer.WriteStartElement("components");
			foreach (ViewTemplate template in viewTemplates)
			{
				template.Write(writer);
			}
			writer.WriteEndElement();

			foreach (TaskInfo t in _taskList.Items)
			{
				if (_taskList.GetItemChecked(_taskList.Items.IndexOf(t)))
				{
					t.Node.WriteTo(writer);
//                    writer.WriteRaw(t.Node.OuterXml);
				}
			}
		}

		private void LoadInventory()
		{
			try
			{
				XmlDocument inventoryDoc = new XmlDocument();
				inventoryDoc.Load(Path.Combine(BasilProject.Project.ApplicationCommonDirectory, "taskInventory.xml"));
				XmlDocument projectDoc = GetProjectDoc();

				//if there are no tasks, might as well be no document, so clear it out
				if(projectDoc != null && (null == projectDoc.SelectSingleNode("tasks/task")))
				{
					projectDoc = null;
				}

				foreach (XmlNode node in inventoryDoc.SelectNodes("tasks/task"))
				{
					TaskInfo task = new TaskInfo(node);
					bool showCheckMark;

					if (projectDoc == null)
					{
						XmlAttribute isDefault = node.Attributes["default"];
						showCheckMark = task.IsDefault;
					}
					else
					{
						XmlNode foundMatchingTask;
						foundMatchingTask = projectDoc.SelectSingleNode("tasks/task[@id='" + task.Id + "']");
						showCheckMark = !task.IsOptional || foundMatchingTask != null;
					}
					this._taskList.Items.Add(task, showCheckMark);
				}
			}
			catch (Exception error)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("There may have been a problem reading the master task inventory xml. " + error.Message);
			}
		}

		private static XmlDocument GetProjectDoc()
		{
			XmlDocument projectDoc = null;
			if (File.Exists(WeSayWordsProject.Project.PathToConfigFile))
			{
				try
				{
					projectDoc = new XmlDocument();
					projectDoc.Load(WeSayWordsProject.Project.PathToConfigFile);
				}
				catch (Exception e)
				{
					Reporting.ErrorReporter.ReportNonFatalMessage("There was a problem reading the task xml. " + e.Message);
					projectDoc = null;
				}
			}
			return projectDoc;
		}

		private void _taskList_SelectedIndexChanged(object sender, EventArgs e)
		{
			TaskInfo i = _taskList.SelectedItem as TaskInfo;
			if (i == null)
				return;
			_description.Text = i.Description;
		}

		private void _taskList_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			TaskInfo i = _taskList.SelectedItem as TaskInfo;
			if (i == null)
			{
				return;
			}
			if (!i.IsOptional)
			{
				e.NewValue = CheckState.Checked;
			}

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}

	public class TaskInfo
	{
		private XmlNode _node;
		public TaskInfo(XmlNode node)
		{
			_node  = node;
		}

		public string Id
		{
			get
			{
				Debug.Assert(Node.Attributes["id"] != null,"Tasks must have ids.");
				return GetOptionalAttributeString(Node, "id", "task");
			}
		}

		public string Description
		{
			get
			{
				return _node.SelectSingleNode("description").InnerText;
			}
		}


		public bool IsDefault
		{
			get
			{
				bool b = GetOptionalAttributeString(_node, "default", "false") == "true";
				return b || !IsOptional;
			}
		}

		public bool IsOptional
		{
			get
			{
				XmlNode x = _node.SelectSingleNode("optional"); ;
				if (x != null && x.InnerText.Trim() == "false")
					return false;
				return true;
			}
		}


		public XmlNode Node
		{
			get { return this._node; }
		}

		public override string ToString()
		{
				XmlNode label = Node.SelectSingleNode("label");
				if (label != null)
				{
					return label.InnerText;
				}
				else
				{
					return GetOptionalAttributeString(Node, "id", "task");
				}

		}

		static private string GetOptionalAttributeString(XmlNode xmlNode, string name, string defaultValue)
		{
			XmlAttribute attr = xmlNode.Attributes[name];
			if (attr == null)
				return defaultValue;
			return attr.Value;
		}

	}
}

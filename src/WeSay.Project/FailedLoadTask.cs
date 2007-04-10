using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WeSay.Project
{
	public class FailedLoadTask :ITask
	{
		private string _label;
		private string _description;

		public FailedLoadTask(string label, string description)
		{
			_label = label;
			_description = description;
		}

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}

		public bool IsActive
		{
			get { return false; }
		}

		public string Label
		{
			get { return String.Format("Failed To Load: {0}", _label); }
		}

		public string Description
		{
			get { return String.Format("Error: {0}", _description); }
		}

		public Control Control
		{
			get {
				TextBox t = new TextBox();
				t.Multiline = true;
				t.Dock = DockStyle.Fill;
				t.Text =
					String.Format(
						"Could not load the task '{0}'. Possibly, the setup in the admin program can be used to fix this.  The error was: [{1}]",
						_label, _description);
				return t;
			}
		}

		public bool IsPinned
		{
			get { return false; }
		}

		public string Status
		{
			get { return ""; }
		}

		public string ExactStatus
		{
			get { return Status; }
		}
	}
}

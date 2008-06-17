using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;

namespace WeSay.Project
{
	public class FailedLoadTask: ITask
	{
		private readonly string _label;
		private readonly string _description;

		public FailedLoadTask(string label, string description)
		{
			_label = label;
			_description = description;
		}

		public void Activate() {}

		public void Deactivate() {}

		#region ITask Members

		public void GoToUrl(string url)
		{
			throw new NotImplementedException();
		}

		#endregion

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

		public bool MustBeActivatedDuringPreCache
		{
			get { return false; }
		}

		public Control Control
		{
			get
			{
				TextBox t = new TextBox();
				t.Multiline = true;
				t.Dock = DockStyle.Fill;
				t.Text =
						String.Format(
								"Could not load the task '{0}'. Possibly, the setup in the admin program can be used to fix this.  The error was: [{1}]",
								_label,
								_description);
				return t;
			}
		}

		public bool IsPinned
		{
			get { return false; }
		}

		private const int CountNotApplicable = -1;

		public int GetRemainingCount()
		{
			return CountNotApplicable;
		}

		public int ExactCount
		{
			get { return CountNotApplicable; }
		}

		/// <summary>
		/// Gives a sense of the overall size of the task versus what's left to do
		/// </summary>
		public int GetReferenceCount()
		{
			return 0;
		}

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Describe; }
		}

		public string LocalizedLabel
		{
			get { return StringCatalog.Get(_label); }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.FixedAmount; }
		}

		public Image DashboardButtonImage
		{
			get { return null; }
		}

		#endregion
	}
}
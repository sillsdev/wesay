using System;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.LexicalModel;

namespace WeSay.UI
{
	public partial class CheckBoxControl: UserControl, IBindableControl<bool>
	{
		private readonly string _nameForLogging;
		public event EventHandler ValueChanged;
		public event EventHandler GoingAway;

		public CheckBoxControl(bool initialValue, string displayLabel, string nameForLogging)
		{
			InitializeComponent();
			checkBox1.Text = displayLabel;
			checkBox1.Checked = initialValue;
			checkBox1.CheckedChanged += checkBox1_CheckedChanged;
			_nameForLogging = nameForLogging;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("CheckBoxControl_CheckChanged ({0})", _nameForLogging);
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this, null);
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null); //shake any bindings to us loose
			}
			GoingAway = null;
			base.OnHandleDestroyed(e);
		}

		public bool Value
		{
			get { return checkBox1.Checked; }
			set { checkBox1.Checked = value; }
		}
	}
}
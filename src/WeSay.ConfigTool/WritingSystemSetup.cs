using System;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.WritingSystems;
using Palaso.WritingSystems;

namespace WeSay.ConfigTool
{
	public class WritingSystemSetup: ConfigurationControlBase
	{
		private WritingSystemSetupView _view;

		public WritingSystemSetup(ILogger logger, IWritingSystemRepository store)
			: base("set up fonts, keyboards, and sorting", logger, "writingSystems")
		{
			InitializeComponent();
			_view = new WritingSystemSetupView(new WritingSystemSetupModel(store))
						{
							LeftColumnWidth = 350,
							Dock = DockStyle.Fill
						};
			Controls.Add(_view);
		}

		public void SetWritingSystemsInRepo_OnLeave(object sender, EventArgs e)
		{
			_view.SetWritingSystemsInRepo();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			//
			// WritingSystemSetup
			//
			this.Name = "WritingSystemSetup";
			this.Leave += new System.EventHandler(this.SetWritingSystemsInRepo_OnLeave);
			this.ResumeLayout(false);

		}

	}
}
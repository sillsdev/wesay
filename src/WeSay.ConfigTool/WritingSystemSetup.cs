using System;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.WritingSystems;
using Palaso.WritingSystems;

namespace WeSay.ConfigTool
{
	public class WritingSystemSetup: ConfigurationControlBase
	{
		public WritingSystemSetup(ILogger logger, IWritingSystemRepository store)
			: base("set up fonts, keyboards, and sorting", logger, "writingSystems")
		{
			store.WritingSystemIdChanged += OnWritingSystemIdChanged;
			var view = new WritingSystemSetupView(new WritingSystemSetupModel(store))
						{
							LeftColumnWidth = 350,
							Dock = DockStyle.Fill
						};
			Controls.Add(view);
		}

		private static void OnWritingSystemIdChanged(object sender, WritingSystemIdChangedEventArgs e)
		{
			Project.WeSayWordsProject.Project.MakeWritingSystemIdChange(e.NewId, e.OldId);
		}
	}
}
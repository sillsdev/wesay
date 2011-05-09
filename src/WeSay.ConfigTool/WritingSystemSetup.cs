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
			var writingSystemSetupModel = new WritingSystemSetupModel(store);
			var view = new WritingSystemSetupView(writingSystemSetupModel)
						{
							LeftColumnWidth = 350,
							Dock = DockStyle.Fill
						};
			writingSystemSetupModel.BeforeDeleted += OnBeforeDeleted;
			Controls.Add(view);
		}

		private void OnBeforeDeleted(object sender, BeforeDeletedEventArgs args)
		{
			args.CanDelete = !Project.WeSayWordsProject.Project.IsWritingSystemInUse(args.WritingSystemId);
		}

		private static void OnWritingSystemIdChanged(object sender, WritingSystemIdChangedEventArgs e)
		{
			Project.WeSayWordsProject.Project.MakeWritingSystemIdChange(e.NewId, e.OldId);
		}
	}
}
using System;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.WritingSystems;
using Palaso.UI.WindowsForms.WritingSystems.WSTree;
using Palaso.WritingSystems;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public class WritingSystemSetup: ConfigurationControlBase
	{
		private WritingSystemSetupView _view;

		public WritingSystemSetup(ILogger logger, IWritingSystemRepository store)
			: base("set up fonts, keyboards, and sorting", logger, "writingSystems")
		{
			InitializeComponent();
			store.WritingSystemIdChanged += OnWritingSystemIdChanged;
			var writingSystemSetupModel = new WritingSystemSetupModel(store);
			writingSystemSetupModel.WritingSystemSuggestor.SuggestVoice = true;
			//nb: I (JH) wanted to hide IPA, but then in one week 2 people locally asked for it...
			writingSystemSetupModel.WritingSystemSuggestor.SuggestIpa = true;
			writingSystemSetupModel.WritingSystemSuggestor.SuggestDialects = false; // pretty unlikely in WeSay

			_view = new WritingSystemSetupView(writingSystemSetupModel)
						{
							LeftColumnWidth = 350,
							Dock = DockStyle.Fill
						};
			writingSystemSetupModel.BeforeDeleted += OnBeforeDeleted;
			writingSystemSetupModel.BeforeConflated += OnBeforeConflated;
			store.WritingSystemDeleted += OnWritingSystemDeleted;
			store.WritingSystemConflated += OnWritingStemConflated;
			Controls.Add(_view);
			WeSayWordsProject.Project.EditorsSaveNow += OnEditorSaveNow;
		}

		private void OnWritingStemConflated(object sender, WritingSystemConflatedEventArgs e)
		{
			WeSayWordsProject.Project.MakeWritingSystemIdChange(e.OldId, e.NewId);
		}

		private void OnEditorSaveNow(object sender, EventArgs e)
		{
			SetWritingSystemsInRepo();
			UnwireBeforeClosing();
		}

		public void SetWritingSystemsInRepo_OnLeave(object sender, EventArgs e)
		{
			SetWritingSystemsInRepo();
		}

		private void SetWritingSystemsInRepo()
		{
			_view.SetWritingSystemsInRepo();
		}

		private void UnwireBeforeClosing()
		{
			Leave -= SetWritingSystemsInRepo_OnLeave;
			_view.UnwireBeforeClosing();
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

		private static void OnBeforeDeleted(object sender, BeforeDeletedEventArgs args)
		{
			args.CanDelete = !WeSayWordsProject.Project.IsWritingSystemUsedInLiftFile(args.WritingSystemId);
			args.ErrorMessage = "It's in use in the LIFT file.";
		}

		private void OnBeforeConflated(object sender, BeforeConflatedEventArgs args)
		{
			args.CanConflate = true; //WeSay always lets people conflate.
		}

		private static void OnWritingSystemDeleted(object sender, WritingSystemDeletedEventArgs args)
		{
			WeSayWordsProject.Project.DeleteWritingSystemId(args.Id);
		}

		private static void OnWritingSystemIdChanged(object sender, WritingSystemIdChangedEventArgs e)
		{
			WeSayWordsProject.Project.MakeWritingSystemIdChange(e.OldId, e.NewId);
		}
	}
}
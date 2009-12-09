using System.Drawing;
using System.Windows.Forms;
using Autofac;
using Chorus;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;


namespace WeSay.LexicalTools.Review.NotesBrowser
{
	public class NotesBrowserTask: TaskBase
	{
		private readonly ChorusNotesSystem _chorusNotesSystem;
		//   private readonly IContainer _diContainer;
		private Control _control;

		public NotesBrowserTask(INotesBrowserConfig config,
								LexEntryRepository lexEntryRepository,
								TaskMemoryRepository taskMemoryRepository,
								ChorusNotesSystem chorusNotesSystem
								)

			: base(config, lexEntryRepository, taskMemoryRepository)
		{
			_chorusNotesSystem = chorusNotesSystem;
			//   _diContainer = diContainer;
		}


		public bool IsTaskComplete
		{
			get
			{
				return false;
			}
		}

		public override DashboardGroup Group
		{
			get { return DashboardGroup.Review; }
		}
		/// <summary>
		/// The GatherWordListControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _control;
			}
		}

		public override Image DashboardButtonImage
		{
			get { return Resources.noteBrowser32x32; }
		}

		public override ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}


		public override void Activate()
		{
			if (_control == null)
			{
				//_control = _diContainer.Resolve<Chorus.UI.Notes.NotesPage>();
				_control = _chorusNotesSystem.CreateNotesBrowserPage();
			}
			base.Activate();

		}


		public override void Deactivate()
		{
			base.Deactivate();
//            if (_control != null)
//            {
//                _control.Dispose();
//            }
//            _control = null;
		}


		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return CountNotRelevant;
		}

		protected override int ComputeReferenceCount()
		{
			return CountNotRelevant;
		}

	}
}
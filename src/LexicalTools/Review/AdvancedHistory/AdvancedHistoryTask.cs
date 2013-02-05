using System.Drawing;
using System.Windows.Forms;
using Autofac;
using Chorus.UI.Review;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.UI;

namespace WeSay.LexicalTools.Review.AdvancedHistory
{
	public class AdvancedHistoryTask: TaskBase
	{
		private readonly IAdvancedHistoryConfig _config;
		private readonly IComponentContext _diContainer;
		private Control _control;

		public AdvancedHistoryTask(IAdvancedHistoryConfig config,
									LexEntryRepository lexEntryRepository,
									TaskMemoryRepository taskMemoryRepository,
									IComponentContext diContainer)

			: base(config, lexEntryRepository, taskMemoryRepository)
		{
			_config = config;
			_diContainer = diContainer;
		}


		public bool IsTaskComplete
		{
			get
			{
				return false;
			}
		}

		public override bool Available
		{
			get { return string.IsNullOrEmpty(Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en")); }
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
			get { return Resources.AdvancedHistory; }
		}

		public override ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public override void Activate()
		{
			if (_control == null)
			{
				var f =  _diContainer.Resolve<Chorus.UI.Review.HistoryPage.Factory>();
				_control = f(new HistoryPageOptions() {RevisionListOptions = new RevisionListOptions()
																				 {
																					 //TODO: introduce a box to enable this mode,
																					 //but it's too complicated to have by default
																					 //in wesay
																					 ShowRevisionChoiceControls = false
																				 }});
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
			return CountNotRelevant; //Todo
		}

	}
}
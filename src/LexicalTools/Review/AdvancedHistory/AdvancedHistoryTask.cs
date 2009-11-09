using System.Drawing;
using System.Windows.Forms;
using Autofac;
using Chorus.UI.Review;
using Palaso.LexicalModel;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.UI;

namespace WeSay.LexicalTools.Review.AdvancedHistory
{
	public class AdvancedHistoryTask: TaskBase
	{
		private readonly IContainer _diContainer;
		private Control _control;

		public AdvancedHistoryTask(IAdvancedHistoryTaskConfig config,
									LexEntryRepository lexEntryRepository,
								  TaskMemoryRepository taskMemoryRepository,
			Autofac.IContainer diContainer)

			: base(config, lexEntryRepository, taskMemoryRepository)
		{
			_diContainer = diContainer;
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
				_control = _diContainer.Resolve<Chorus.UI.Review.ReviewPage>();
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
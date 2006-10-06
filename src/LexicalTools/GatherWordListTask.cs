using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public class GatherWordListTask : TaskBase
	{
		private readonly string _wordListFileName;
		private GatherWordListControl  _gatherControl;

		public GatherWordListTask(IRecordListManager recordListManager, string label, string description, string wordListFileName)
			: base(label, description, recordListManager)
		{
			_wordListFileName = wordListFileName;
		}

		/// <summary>
		/// The GatherWordListControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _gatherControl;
			}
		}

		public override void Activate()
		{
			base.Activate();
			_gatherControl = new GatherWordListControl();
		  //  this._gatherControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

		}

		public override void Deactivate()
		{
			base.Deactivate();
		   // _gatherControl.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_gatherControl.Dispose();
			_gatherControl = null;
			this.RecordListManager.GoodTimeToCommit();
		}
	}
}

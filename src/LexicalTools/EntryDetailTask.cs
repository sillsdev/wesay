using System;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class EntryDetailTask : TaskBase
	{
		private EntryDetailControl _entryDetailControl;
		private readonly FieldInventory _fieldInventory;

		public EntryDetailTask(IRecordListManager recordListManager,
							FieldInventory fieldInventory)
			: base("Dictionary", string.Empty, true, recordListManager)
		{
			if (fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_fieldInventory = fieldInventory;
		}

		public override void Activate()
		{
			base.Activate();
			_entryDetailControl = new EntryDetailControl(DataSource, FieldInventory);
			_entryDetailControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			_entryDetailControl.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_entryDetailControl.Dispose();
			_entryDetailControl = null;
			RecordListManager.GoodTimeToCommit();
		}

		/// <summary>
		/// The entry detail control associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _entryDetailControl;
			}
		}

		public override string Status
		{
			get
			{
				return DataSource.Count.ToString();
			}
		}

		public override string Description
		{
			get
			{
				return String.Format(StringCatalog.Get("See all {0} {1} words."), DataSource.Count, BasilProject.Project.Name);
			}
		}

		public IRecordList<LexEntry> DataSource
		{
			get
			{
				IRecordList<LexEntry> data = RecordListManager.Get<LexEntry>();
				return data;
			}
		}

		public FieldInventory FieldInventory
		{
			get { return this._fieldInventory; }
		}
	}
}
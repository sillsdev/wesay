using System;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public class LexFieldTask : TaskBase
	{
		private LexFieldTool _lexFieldTool;
		private IFilter<LexEntry> _filter;

		private Predicate<string> _showField;
		public Predicate<string> ShowField
		{
			get
			{
				return _showField;
			}
		 }

		public LexFieldTask(IRecordListManager recordListManager, IFilter<LexEntry> filter, string label, string description, string fieldsToShow)
			:base(label, description, recordListManager)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			 if (fieldsToShow == null)
			{
				throw new ArgumentNullException("fieldsToShow");
			}
		   recordListManager.Register<LexEntry>(filter);
			_filter = filter;
			InitializeFieldFilter(fieldsToShow);
		}

		private void InitializeFieldFilter(string fieldsToShow)
		{
			string[] fields = fieldsToShow.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			_showField = delegate(string s)
						 {
							return Array.Exists<string>(fields, delegate (string field){
								return s==field;
							});
						 };
		}


		public override void Activate()
		{
			base.Activate();
			_lexFieldTool = new LexFieldTool(DataSource, _showField);
			_lexFieldTool.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
		   _lexFieldTool.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_lexFieldTool.Dispose();
			_lexFieldTool = null;
			RecordListManager.GoodTimeToCommit();
		}

		/// <summary>
		/// The LexFieldTool associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _lexFieldTool;
			}
		}

		public override string Status
		{
			get
			{
				if (DataHasBeenRetrieved)
				{
					return DataSource.Count.ToString();
				}
				return "?";
			}
		}

		public IRecordList<LexEntry> DataSource
		{
			get
			{
				IRecordList<LexEntry> data = RecordListManager.Get<LexEntry>(_filter);
				DataHasBeenRetrieved = true;
				return data;
			}
		}
	}
}

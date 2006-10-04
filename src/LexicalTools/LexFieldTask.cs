using System;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public partial class LexFieldTask : ITask
	{
		private LexFieldTool _lexFieldTool;
		private IRecordListManager _recordListManager;
		private IFilter<LexEntry> _filter;
		private string _label;
		private string _description;

		public string Description
		{
			get
			{
				return StringCatalog.Get(_description);
			}
		}

		private Predicate<string> _showField;

		public Predicate<string> ShowField
		{
			get
			{
				return _showField;
			}
		 }

		public LexFieldTask(IRecordListManager recordListManager, IFilter<LexEntry> filter, string label, string description, string fieldsToShow)
		{
			if (recordListManager == null)
			{
				throw new ArgumentNullException("recordListManager");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (label == null)
			{
				throw new ArgumentNullException("label");
			}
			if (description == null)
			{
				throw new ArgumentNullException("description");
			}
			if (fieldsToShow == null)
			{
				throw new ArgumentNullException("fieldsToShow");
			}
			_recordListManager = recordListManager;
			recordListManager.Register<LexEntry>(filter);
			_label = label;
			_description = description;
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


		public void Activate()
		{
			_lexFieldTool = new LexFieldTool(DataSource, _showField);
		}


		public void Deactivate()
		{
			_lexFieldTool.Dispose();
			_lexFieldTool = null;
		}

		public string Label
		{
			get
			{
				return StringCatalog.Get(_label);
			}
		}

		/// <summary>
		/// The LexFieldTool associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public Control Control
		{
			get
			{
				return _lexFieldTool;
			}
		}

		public bool IsPinned
		{
			get
			{
				return false;
			}
		}

		public string Status
		{
			get
			{
				return DataSource.Count.ToString();
			}
		}

		public IRecordList<LexEntry> DataSource
		{
			get
			{
				return _recordListManager.Get<LexEntry>(_filter);
			}
		}

	}
}

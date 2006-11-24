using System;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.Data;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class LexFieldTask : TaskBase
	{
		private LexFieldControl _lexFieldControl;
		private readonly IFilter<LexEntry> _filter;
		private readonly FieldInventory _fieldInventory;
		private bool _dataHasBeenRetrieved;

		public LexFieldTask(IRecordListManager recordListManager,
					IFilter<LexEntry> filter,
					string label,
					string description,
					FieldInventory fieldInventory)
			: base(label, description, false, recordListManager)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			recordListManager.Register<LexEntry>(filter);
			_filter = filter;
			_fieldInventory = fieldInventory;
		}

		/// <summary>
		/// Creates a generic Lexical Field editing task
		/// </summary>
		/// <param name="recordListManager">The recordListManager that will provide the data</param>
		/// <param name="filter">The filter that should be used to filter the data</param>
		/// <param name="label">The task label</param>
		/// <param name="description">The task description</param>
		/// <param name="fieldInventory">The base FieldInventory</param>
		/// <param name="fieldsToShow">The fields to show from the base Field Inventory</param>
		public LexFieldTask(IRecordListManager recordListManager,
							IFilter<LexEntry> filter,
							string label,
							string description,
							FieldInventory fieldInventory,
							string fieldsToShow)
			:this(recordListManager, filter, label, description, fieldInventory)
		{
			if (fieldsToShow == null)
			{
				throw new ArgumentNullException("fieldsToShow");
			}
			_fieldInventory = FilterFieldInventory(fieldInventory, fieldsToShow);
		}

		private FieldInventory FilterFieldInventory(FieldInventory baseFieldInventory, string fieldsToShow)
		{
			string[] fields = fieldsToShow.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			FieldInventory fieldInventory = new FieldInventory();
			foreach (Field field in baseFieldInventory)
			{
				if(Array.IndexOf<string>(fields, field.FieldName) >= 0)
				{
					fieldInventory.Add(field);
				}
			}
			return fieldInventory;
		}

		public override void Activate()
		{
			base.Activate();
			_lexFieldControl = new LexFieldControl(DataSource, FieldInventory, _filter.FilteringPredicate);
			_lexFieldControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
		   _lexFieldControl.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_lexFieldControl.Dispose();
			_lexFieldControl = null;
			RecordListManager.GoodTimeToCommit();
		}

		/// <summary>
		/// The LexFieldControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _lexFieldControl;
			}
		}

		public override string Status
		{
			get
			{
				if (_dataHasBeenRetrieved)
				{
					return DataSource.Count.ToString();
				}
				return "-";// String.Empty;
			}
		}

		public IRecordList<LexEntry> DataSource
		{
			get
			{
				IRecordList<LexEntry> data = RecordListManager.GetListOfTypeFilteredFurther<LexEntry>(_filter);
				_dataHasBeenRetrieved = true;
				return data;
			}
		}

		public FieldInventory FieldInventory
		{
			get { return this._fieldInventory; }
		}
	}
}

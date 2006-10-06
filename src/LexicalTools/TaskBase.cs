using System;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public abstract class TaskBase : ITask
	{
		private IRecordListManager _recordListManager;
		private string _label;
		private string _description;
		private bool _dataHasBeenRetrieved;

		public TaskBase(string label, string description, IRecordListManager recordListManager)
		{
			if (recordListManager == null)
			{
				throw new ArgumentNullException("recordListManager");
			}
			if (label == null)
			{
				throw new ArgumentNullException("label");
			}
			if (description == null)
			{
				throw new ArgumentNullException("description");
			}

			_recordListManager = recordListManager;
			_label = label ;
			_description = description;
		}

		public string Description
		{
			get
			{
				return StringCatalog.Get(_description);
			}
		}

		private bool _isActive;

		public virtual void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException("Activate should not be called when object is active.");
			}
			_isActive = true;
		}

		public virtual void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException("Deactivate should only be called once after Activate.");
			}
			_isActive = false;
		}

		public bool IsActive
		{
			get { return this._isActive; }
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
		public abstract Control Control
		{
			get;
		}

		public bool IsPinned
		{
			get
			{
				return false;
			}
		}

		public virtual string Status
		{
			get
			{
				return "?";
			}
		}

		protected bool DataHasBeenRetrieved
		{
			get { return this._dataHasBeenRetrieved; }
			set { this._dataHasBeenRetrieved = value; }
		}

		protected IRecordListManager RecordListManager
		{
			get { return this._recordListManager; }
		}
	}
}

using System;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public abstract class TaskBase : ITask
	{
		private IRecordListManager _recordListManager;
		private string _label;
		private string _description;
		private readonly bool _isPinned;

		public TaskBase(string label, string description, bool isPinned, IRecordListManager recordListManager)
		{
			if (label == null)
			{
				throw new ArgumentNullException("label");
			}
			if (description == null)
			{
				throw new ArgumentNullException("description");
			}
			if (recordListManager == null)
			{
				throw new ArgumentNullException("recordListManager");
			}
			_recordListManager = recordListManager;
			_label = label;
			_description = description;
			_isPinned = isPinned;
		}

		public virtual string Description
		{
			get { return StringCatalog.Get(_description); }
		}

		private bool _isActive = false;

		public virtual void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException("Activate should not be called when object is active.");
			}
			IsActive = true;
		}

		public virtual void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException("Deactivate should only be called once after Activate.");
			}
			IsActive = false;
		}

		protected void VerifyTaskActivated()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException("Task must be activated first");
			}
		}

		public string Label
		{
			get { return StringCatalog.Get(_label); }
		}

		/// <summary>
		/// The control associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public abstract Control Control { get; }

		public bool IsPinned
		{
			get { return _isPinned; }
		}

		public virtual string Status
		{
			get { return string.Empty; }
		}

		public virtual string ExactStatus
		{
			get { return Status; }
		}

		/// <summary>
		/// Gives a sense of the overall size of the task versus what's left to do
		/// </summary>
		public virtual int ReferenceCount
		{
			get
			{
				//this is obviously flawed ;-)
				return _recordListManager.GetListOfType<LexEntry>().Count;
			}
		}

		protected IRecordListManager RecordListManager
		{
			get { return _recordListManager; }
		}

		public bool IsActive
		{
			get { return _isActive; }
			protected set { _isActive = value; }
		}
	}
}
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;
using System.Text.RegularExpressions;

namespace WeSay.LexicalTools
{
	public partial class LexFieldTask : ITask
	{
		private LexFieldTool _lexFieldTool;
		private IBindingList _records;
		private string _label;
		private string _description;

		public string Description
		{
			get
			{
				return _description;
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

		public LexFieldTask(IRecordList<LexEntry> records, IFilter<LexEntry> filter, string label, string description, string fieldsToShow)
		{
			if (records == null)
			{
				throw new ArgumentNullException("records");
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
			_records = records;
			_label = label;
			_description = description;
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
			_lexFieldTool = new LexFieldTool(_records, _showField);
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
				return _label;
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

		public IBindingList DataSource
		{
			get
			{
				return _records;
			}
		}

	}
}

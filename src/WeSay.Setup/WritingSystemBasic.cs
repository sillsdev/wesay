using System;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Language;

namespace WeSay.Setup
{
	public partial class WritingSystemBasic : UserControl
	{
		private WritingSystem _writingSystem;
		private WritingSystemCollection _writingSystemCollection;

		public event EventHandler WritingSystemIdChanged;

//        public class PropertyChangingEventArgs : PropertyChangedEventArgs
//        {
//            public bool Cancel = false;
//
//            public PropertyChangingEventArgs(string propertyName)
//                : base(propertyName)
//            {
//            }
//        }

		/// <summary>
		/// called when the user wants to change the actual id of a ws, which has large reprocussions
		/// </summary>
		//  public event System.EventHandler IdChanging;
		public WritingSystemBasic()
		{
			InitializeComponent();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;
				_writingSystemProperties.SelectedObject = _writingSystem;
				// _fontProperties.SelectedObjects = new object[] { _writingSystem, helper };
				Refresh();
			}
		}

		/// <summary>
		/// for checking that ids are unique
		/// </summary>
		public WritingSystemCollection WritingSystemCollection
		{
			get { return _writingSystemCollection; }
			set { _writingSystemCollection = value; }
		}

		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.PropertyDescriptor.Name != "Id")
			{
				return;
			}

			string id = e.ChangedItem.Value as string;

			if (_writingSystemCollection.ContainsKey(id))
			{
				ErrorReport.ReportNonFatalMessage("Sorry, there is already a Writing System with that ID.");
				_writingSystem.Id = e.OldValue.ToString();
			}
			else
			{
				if (WritingSystemIdChanged != null)
				{
					WritingSystemIdChanged.Invoke(_writingSystem, e);
				}
			}
		}
	}
}
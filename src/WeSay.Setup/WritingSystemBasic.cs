using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.Setup
{
	public partial class WritingSystemBasic : UserControl
	{
		private WritingSystem _writingSystem;
		private WritingSystemCollection _writingSystemCollection;

		public event System.EventHandler WritingSystemIdChanged;

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
			get { return this._writingSystem; }
			set
			{
				this._writingSystem = value;
				_writingSystemProperties.SelectedObject = _writingSystem;
			   // _fontProperties.SelectedObjects = new object[] { _writingSystem, helper };
				this.Refresh();
			}
		}

		/// <summary>
		/// for checking that ids are unique
		/// </summary>
		public WritingSystemCollection WritingSystemCollection
		{
			get { return this._writingSystemCollection; }
			set { this._writingSystemCollection = value; }
		}

		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.PropertyDescriptor.Name != "Id")
				return;

			string id = e.ChangedItem.Value as string;

			if (_writingSystemCollection.ContainsKey(id))
			{
				MessageBox.Show("Sorry, there is already a Writing System with that ID.");
				_writingSystem.Id = e.OldValue.ToString();
			}
			else
			{
				if (WritingSystemIdChanged != null)
				{
					WritingSystemIdChanged.Invoke(_writingSystem,e);
				}
			}
		}
	}


}

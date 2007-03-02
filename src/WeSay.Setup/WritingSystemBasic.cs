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

		public event System.EventHandler DisplayPropertiesChanged;

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
				_fontProperties.SelectedObject = _writingSystem;
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

		private void _fontProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
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
				if (DisplayPropertiesChanged != null)
				{
					DisplayPropertiesChanged.Invoke(_writingSystem,e);
				}
			}
		}
//
//        private void btnUseForVernacular_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
//        {
//            _writingSystemCollection.VernacularWritingSystemDefaultId  = _writingSystem.Id;
//
//            if (DisplayPropertiesChanged != null)
//            {
//                DisplayPropertiesChanged.Invoke(_writingSystem, null);
//            }
//        }
//
//        private void btnUseForAnalysis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
//        {
//            _writingSystemCollection.AnalysisWritingSystemDefaultId = _writingSystem.Id;
//
//            if (DisplayPropertiesChanged != null)
//            {
//                DisplayPropertiesChanged.Invoke(_writingSystem, null);
//            }
//        }




	}


}

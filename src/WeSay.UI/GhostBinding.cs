using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	/// <summary>
	/// Enables us to display plain text boxes which cannot correspond to any actual data objects yet.
	/// when the user enters information in this "ghost" text box, events are fired that will cause the
	/// actual object to the created and filled in with the data the user has entered.
	/// </summary>
	public class GhostBinding
	{
		private string _writingSystemId;
		private string _propertyName;
		private IBindingList _listTarget;
		private TextBox _textBoxTarget;
		private Control _referenceControl;

		public delegate void GhostTriggered(GhostBinding sender, object newGuy, EventArgs args);

		/// <summary>
		/// Fires at some point after the user has entered some information in the ghost text box.
		/// (client should not count on the definition of when)
		/// </summary>
		public event GhostTriggered Triggered;

		public GhostBinding(IBindingList targetList, string propertyName,  string writingSystemId, TextBox textBoxTarget)
		{
		   _listTarget= targetList;
		   _listTarget.ListChanged +=new ListChangedEventHandler(_listTarget_ListChanged);
		   _propertyName=propertyName;
		   _writingSystemId = writingSystemId;

		   _textBoxTarget = textBoxTarget;
		   _textBoxTarget.Leave += new EventHandler(OnTextBoxTarget_Leave);
		   _textBoxTarget.Disposed+=new EventHandler(_textBoxTarget_Disposed); //+= new EventHandler(_textBoxTarget_HandleDestroyed);
		   _textBoxTarget.VisibleChanged += new EventHandler(_textBoxTarget_VisibleChanged);
		}


		/// <summary>
		/// Change of visibility is not a very satisfying to time to trigger this,
		/// but it does the best I've found.
		/// </summary>
		 void _textBoxTarget_VisibleChanged(object sender, EventArgs e)
		{
			if (((TextBox)sender).Visible == false)
			{
				TearDown();
			}
		}

	   /// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			_referenceControl = null;
			_listTarget.ListChanged -=new ListChangedEventHandler(_listTarget_ListChanged);
			_listTarget = null;
			_textBoxTarget.Leave -= new EventHandler(OnTextBoxTarget_Leave);
			_textBoxTarget.Disposed -= new EventHandler(_textBoxTarget_Disposed); //+= new EventHandler(_textBoxTarget_HandleDestroyed);
			_textBoxTarget.VisibleChanged -= new EventHandler(_textBoxTarget_VisibleChanged);
			_textBoxTarget = null;
		}


		/// <summary>
		/// We get this when closing the app, rather than the visibility changed event.
		/// </summary>
		void _textBoxTarget_Disposed(object sender, EventArgs e)
		{
			TearDown();
		}

		/// <summary>
		/// The reference control is the one we need to use when it comes time to insert
		/// new controls into the detail view.  For example, in the original implementation
		/// of DetailList, this will be a panel which encloses the label and text box.
		/// </summary>
		public Control ReferenceControl
		{
			get { return _referenceControl; }
			set { _referenceControl = value; }
		}

		void OnTextBoxTarget_Leave(object sender, EventArgs e)
		{
			if(_textBoxTarget.Text.Trim().Length>0)
			{
				TimeForRealObject();
			}
		}


		/// <summary>
		/// Handled the case where some mechanism (including this class) makes the list we are targeting nonempty.
		/// Our job in this case is fire the events which switch the UI for this list over from Ghost to Real.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void  _listTarget_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				object newGuy = _listTarget[e.NewIndex];
				FillInMultiTextOfNewObject(newGuy, _propertyName, _writingSystemId, _textBoxTarget.Text);
				if (Triggered != null)
				{
					Triggered.Invoke(this, newGuy, null);
				}
			}
		}

		protected  void TimeForRealObject()
		{
			IBindingList list = _listTarget as IBindingList;
			//in addition to adding a list item, this will fire events on the object that owns the list
			list.AddNew();
			_textBoxTarget.Text = ""; //ready for the next one
			_textBoxTarget.BackColor = System.Drawing.SystemColors.Control;
			_textBoxTarget.BorderStyle  = BorderStyle.None;
		}

		private void FillInMultiTextOfNewObject(object o, string propertyName, string writingSystemId, string value)
		{
		   PropertyInfo info = o.GetType().GetProperty(propertyName);
		   MultiText text = (MultiText) info.GetValue(o, null);
		   text.SetAlternative(writingSystemId, value);
		}
	}
}

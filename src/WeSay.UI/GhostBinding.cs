using System;
using System.ComponentModel;
using System.Diagnostics;
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
		/// <summary>
		/// Can be used to track which data item the user is currently editting, to,
		/// for example, hilight that piece in a preview control
		/// </summary>
		public event EventHandler<CurrentItemEventArgs> CurrentItemChanged = delegate
																	 {
																	 };

		private WritingSystem _writingSystem;
		private string _propertyName;
		private IBindingList _listTarget;
		private WeSayTextBox _textBoxTarget;
		private Control _referenceControl;

		public delegate void GhostTriggered(GhostBinding sender, IBindingList list, int index, MultiTextControl previouslyGhostedControlToReuse, bool doGoToNextField, EventArgs args);

		/// <summary>
		/// Fires at some point after the user has entered some information in the ghost text box.
		/// (client should not count on the definition of when)
		/// </summary>
		public event GhostTriggered Triggered;

		private bool _inMidstOfTrigger = false;

		public GhostBinding(IBindingList targetList, string propertyName, WritingSystem writingSystem, WeSayTextBox textBoxTarget)
		{
		   _listTarget= targetList;
		   _listTarget.ListChanged +=new ListChangedEventHandler(_listTarget_ListChanged);
		   _propertyName=propertyName;
		   _writingSystem = writingSystem;

		   _textBoxTarget = textBoxTarget;
		   _textBoxTarget.KeyDown += new KeyEventHandler(_textBoxTarget_KeyDown);
		   _textBoxTarget.LostFocus += new EventHandler(_textBoxTarget_LostFocus);
		   _textBoxTarget.TextChanged += new EventHandler(_textBoxTarget_TextChanged);
		   _textBoxTarget.Enter += new EventHandler(OnTextBoxEntered);
		   _textBoxTarget.HandleDestroyed += new EventHandler(_textBoxTarget_HandleDestroyed);
		}

		void _textBoxTarget_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; //none of this
				this.TimeForRealObject(false);
			}
			else
			{
				e.Handled = false;
			}
		}


		/// <summary>
		/// this only gets called when the control is actually, like, maybe finalized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _textBoxTarget_HandleDestroyed(object sender, EventArgs e)
		{
			TearDown();
		}

		void _textBoxTarget_LostFocus(object sender, EventArgs e)
		{
				TimeForRealObject(true);
		}

		void _textBoxTarget_TextChanged(object sender, EventArgs e)
		{
//            if ( _textBoxTarget.Text.Trim().Length > 0)
//            {
//                TimeForRealObject();
//            }
		}

		void OnTextBoxEntered(object sender, EventArgs e)
		{
			CurrentItemChanged(sender, new CurrentItemEventArgs(_propertyName, _writingSystem.Id));
		}

		 /// <summary>
		/// Change of visibility is not a very satisfying to time to trigger this,
		/// but it does the best I've found.
		/// </summary>
		 void _textBoxTarget_VisibleChanged(object sender, EventArgs e)
		{
			//once I wrapped textbox in wesaytextbox, this was never false anymore!
			 if (((WeSayTextBox)sender).Visible == false)
			{
				TearDown();
			}
		}

		/// <summary>
		/// We get this when closing the app, rather than the visibility changed event.
		/// </summary>
		void _textBoxTarget_Disposed(object sender, EventArgs e)
		{
			TearDown();
		}

	   /// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			Debug.Assert(!_inMidstOfTrigger);
		   // Debug.WriteLine(" GhostBindingTearDown boundTo: " + this._textBoxTarget.Name);

			if (_listTarget == null)
			{
				return; //teardown was called twice
			}
			_referenceControl = null;
			_listTarget.ListChanged -=new ListChangedEventHandler(_listTarget_ListChanged);
			_listTarget = null;
			_textBoxTarget.TextChanged -= new EventHandler(_textBoxTarget_TextChanged);
			_textBoxTarget.HandleDestroyed -= new EventHandler(_textBoxTarget_HandleDestroyed);
			_textBoxTarget = null;
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




		/// <summary>
		/// Handle the case where some mechanism (including this class) makes a change to the list we are
		/// here to add items to.
		/// Our job in this case is fire the events which switch the UI for the ghost widget over from Ghost to Real.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void  _listTarget_ListChanged(object sender, ListChangedEventArgs e)
		{

			//REVIEW: JH says... dont we only want to trigger if it was *this* guy that is responsible?)


//            if (e.ListChangedType == ListChangedType.ItemAdded)
//            {
//                object newGuy = _listTarget[e.NewIndex];
//                FillInMultiTextOfNewObject(newGuy, _propertyName, _writingSystem, _textBoxTarget.Text);
//                if (Triggered != null)
//                {
//                    Triggered.Invoke(this, _listTarget, e.NewIndex, null);
//                }
//            }
		}

		protected  void TimeForRealObject(bool doGoToNextField)
		{
			if (_textBoxTarget.Text.Trim().Length == 0)
			{
				return;
			}

			//don't let the code here trigger the ghost all over again
			if (_inMidstOfTrigger)
			{
				return;
			}
			_inMidstOfTrigger = true;

			IBindingList list = _listTarget as IBindingList;
			//in addition to adding a list item, this will fire events on the object that owns the list
			Object newGuy = list.AddNew();


		  //  object newGuy = _listTarget[e.NewIndex];
			FillInMultiTextOfNewObject(newGuy, _propertyName, _writingSystem, _textBoxTarget.Text);
			if (Triggered != null)
			{
				Triggered.Invoke(this, _listTarget, list.IndexOf(newGuy), null/*todo*/, doGoToNextField, null);
			}

			_textBoxTarget.Text = "";
			_inMidstOfTrigger = false;
			//_textBoxTarget.PrepareForFadeIn();
		 }

		private static void FillInMultiTextOfNewObject(object o, string propertyName, WritingSystem writingSystem, string value)
		{
		   PropertyInfo info = o.GetType().GetProperty(propertyName);
		   MultiText text = (MultiText) info.GetValue(o, null);
		   text.SetAlternative(writingSystem.Id, value);
		}
	}
}

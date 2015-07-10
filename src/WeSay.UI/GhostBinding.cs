using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using SIL.Lift;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.TextBoxes;

namespace WeSay.UI
{
	/// <summary>
	/// Enables us to display plain text boxes which cannot correspond to any actual data objects yet.
	/// when the user enters information in this "ghost" text box, events are fired that will cause the
	/// actual object to the created and filled in with the data the user has entered.
	/// </summary>
	public class GhostBinding<T> where T : new()
	{
		/// <summary>
		/// Can be used to track which data item the user is currently editting, to,
		/// for example, hilight that piece in a preview control
		/// </summary>
		public event EventHandler<CurrentItemEventArgs> CurrentItemChanged = delegate { };

		private readonly WritingSystemDefinition _writingSystem;
		private readonly string _propertyName;
		private readonly PalasoDataObject _parent;
		private IList<T> _listTarget;
		private Control _textBoxTarget;
		private Control _referenceControl;

		public delegate void LayoutNeededHandler(
				GhostBinding<T> sender,
				IList<T> list,
				int index,
				MultiTextControl previouslyGhostedControlToReuse,
				bool doGoToNextField,
				EventArgs args);

		/// <summary>
		/// Fires at some point after the user has entered some information in the ghost text box.
		/// (client should not count on the definition of when)
		/// </summary>
		public event LayoutNeededHandler LayoutNeededAfterMadeReal;

		private bool _inMidstOfTrigger;

		public GhostBinding(PalasoDataObject parent,
							IList<T> targetList,
							string propertyName,
							WritingSystemDefinition writingSystem,
							IWeSayTextBox textBoxTarget)
		{
			_parent = parent;
			_listTarget = targetList;
			//           _listTarget.ListChanged +=new ListChangedEventHandler(_listTarget_ListChanged);
			_propertyName = propertyName;
			_writingSystem = writingSystem;

			_textBoxTarget = (Control) textBoxTarget;
			_textBoxTarget.KeyDown += _textBoxTarget_KeyDown;
			// Lost Focus doesn't seem to fire for the GeckoBox so added leaving
			_textBoxTarget.LostFocus += _textBoxTarget_LostFocus;
			_textBoxTarget.Leave += _textBoxTarget_LostFocus;
			_textBoxTarget.Enter += OnTextBoxEntered;
			_textBoxTarget.HandleDestroyed += _textBoxTarget_HandleDestroyed;
			_textBoxTarget.Disposed += _textBoxTarget_Disposed;
			if (_textBoxTarget is IWeSayTextBox)
			{
				((IWeSayTextBox)_textBoxTarget).UserLostFocus += _textBoxTarget_LostFocus;
				((IWeSayTextBox)_textBoxTarget).UserGotFocus += OnTextBoxEntered;
			}
		}

		private void _textBoxTarget_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; //none of this
				TimeForRealObject(false);
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
		private void _textBoxTarget_HandleDestroyed(object sender, EventArgs e)
		{
			TearDown();
		}

		private void _textBoxTarget_Disposed(object sender, EventArgs e)
		{
			TearDown();
		}

		private void _textBoxTarget_LostFocus(object sender, EventArgs e)
		{
			TimeForRealObject(true);
		}

		//void _textBoxTarget_TextChanged(object sender, EventArgs e)
		//{
		//            if ( _textBoxTarget.Text.Trim().Length > 0)
		//            {
		//                TimeForRealObject();
		//            }
		//}

		private void OnTextBoxEntered(object sender, EventArgs e)
		{
			CurrentItemChanged(sender, new CurrentItemEventArgs(_parent, _propertyName, _writingSystem.LanguageTag));
		}

		// /// <summary>
		///// Change of visibility is not a very satisfying to time to trigger this,
		///// but it does the best I've found.
		///// </summary>
		// void _textBoxTarget_VisibleChanged(object sender, EventArgs e)
		//{
		//    //once I wrapped textbox in IWeSayTextBox, this was never false anymore!
		//     if (((IWeSayTextBox)sender).Visible == false)
		//    {
		//        TearDown();
		//    }
		//}

		///// <summary>
		///// We get this when closing the app, rather than the visibility changed event.
		///// </summary>
		//void _textBoxTarget_Disposed(object sender, EventArgs e)
		//{
		//    TearDown();
		//}

		/// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			//   Debug.Assert(!_inMidstOfTrigger);
			// Debug.WriteLine(" GhostBindingTearDown boundTo: " + this._textBoxTarget.Name);

			if (_listTarget == null)
			{
				return; //teardown was called twice
			}
			_referenceControl = null;
			//            _listTarget.ListChanged -=new ListChangedEventHandler(_listTarget_ListChanged);
			_listTarget = null;
			//            _textBoxTarget.TextChanged -= new EventHandler(_textBoxTarget_TextChanged);

			_textBoxTarget.KeyDown -= _textBoxTarget_KeyDown;
			_textBoxTarget.LostFocus -= _textBoxTarget_LostFocus;
			_textBoxTarget.Leave -= _textBoxTarget_LostFocus;
			_textBoxTarget.Enter -= OnTextBoxEntered;
			if (_textBoxTarget is IWeSayTextBox)
			{
				((IWeSayTextBox)_textBoxTarget).UserLostFocus -= _textBoxTarget_LostFocus;
				((IWeSayTextBox)_textBoxTarget).UserGotFocus -= OnTextBoxEntered;
			}
			_textBoxTarget.HandleDestroyed -= _textBoxTarget_HandleDestroyed;
			_textBoxTarget.Disposed -= _textBoxTarget_Disposed;
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

		//        /// <summary>
		//        /// Handle the case where some mechanism (including this class) makes a change to the list we are
		//        /// here to add items to.
		//        /// Our job in this case is fire the events which switch the UI for the ghost widget over from Ghost to Real.
		//        /// </summary>
		//        /// <param name="sender"></param>
		//        /// <param name="e"></param>
		//        void  _listTarget_ListChanged(object sender, ListChangedEventArgs e)
		//        {

		//            //REVIEW: JH says... dont we only want to trigger if it was *this* guy that is responsible?)

		////            if (e.ListChangedType == ListChangedType.ItemAdded)
		////            {
		////                object newGuy = _listTarget[e.NewIndex];
		////                FillInMultiTextOfNewObject(newGuy, _propertyName, _writingSystem, _textBoxTarget.Text);
		////                if (LayoutNeededAfterMadeReal != null)
		////                {
		////                    LayoutNeededAfterMadeReal.Invoke(this, _listTarget, e.NewIndex, null);
		////                }
		////            }
		//        }

		protected void TimeForRealObject(bool doGoToNextField)
		{
			if (_listTarget == null)
			{
				return; //teardown was already called
			}

			//IWeSayTextBox textBoxTarget = _textBoxTarget;
			if (_textBoxTarget.Text.Trim().Length == 0)
			{
				return;
			}
			Logger.WriteMinorEvent("TimeForRealObject:" + _propertyName);

			//don't let the code here trigger the ghost all over again
			if (_inMidstOfTrigger)
			{
				return;
			}
			_inMidstOfTrigger = true;

			IList<T> list = _listTarget;
			//in addition to adding a list item, this will fire events on the object that owns the list
			Logger.WriteMinorEvent("Before AddNew in TimeForRealObject");

			//!!!! Anything can happen to our internal state after this point so don't rely on any objects sticking
			// around and not being null!
			T newGuy = new T();
			Logger.WriteMinorEvent("After AddNew in TimeForRealObject");

			//if (_textBoxTarget == null)
			//{
			//    Reporting.Logger.WriteMinorEvent("Looks like this ghost was disposed while its method was running.");
			//    throw new ApplicationException(string.Format("PLEASE HELP. This is a bug we're having trouble reproducing.  PLEASE TELL US HOW YOU DID THIS (related to issue ws-306) property={0}. CAN YOU DO IT AGAIN?", this._propertyName));
			//}

			//  object newGuy = _listTarget[e.NewIndex];
			FillInMultiTextOfNewObject(newGuy, _propertyName, _writingSystem, _textBoxTarget.Text);
			list.Add(newGuy);
			if (LayoutNeededAfterMadeReal != null && ReferenceControl != null)
			{
				// The Layouter subscribes to this event, and includes an Application.DoEvents
				// which can cause the _textBoxTarget dispose to be handled before we complete
				// the remainder of TimeForRealObject.
				LayoutNeededAfterMadeReal.Invoke(this,
												 list,
												 list.IndexOf(newGuy),
												 null /*todo*/,
												 doGoToNextField,
												 null);
			}
			if (_textBoxTarget != null)
			{
				_textBoxTarget.Text = "";
			}
			_inMidstOfTrigger = false;
			//_textBoxTarget.PrepareForFadeIn();
		}

		private static void FillInMultiTextOfNewObject(object o,
													   string propertyName,
													   WritingSystemDefinition writingSystem,
													   string value)
		{
			PropertyInfo info = o.GetType().GetProperty(propertyName);
			MultiText text = (MultiText) info.GetValue(o, null);
			text.SetAlternative(writingSystem.LanguageTag, value);
		}
	}
}
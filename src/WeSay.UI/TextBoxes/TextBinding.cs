using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.UI.TextBoxes
{
	/// <summary>
	/// This simple binding class connects a text box with a MultiText or AudioField (which just
	/// looks from here like something with  a text (which happens to be an audio file name).
	/// Changes in either one are reflected in the other.
	/// </summary>
	public class TextBinding
	{
		public event EventHandler<CurrentItemEventArgs> ChangeOfWhichItemIsInFocus = delegate { };
		private readonly string _writingSystemId;
		private INotifyPropertyChanged _dataTarget;
		private Control _textBoxTarget;
		private bool _inMidstOfChange;
		private string _pendingValueChange=null;

		public TextBinding(INotifyPropertyChanged dataTarget,
						   string writingSystemId,
						   Control widgetTarget)
		{
			Debug.Assert(dataTarget != null);
			_dataTarget = dataTarget;
			_dataTarget.PropertyChanged += OnDataPropertyChanged;
			_writingSystemId = writingSystemId;
			_textBoxTarget = widgetTarget;

			_textBoxTarget.TextChanged += OnTextBoxChanged;
			_textBoxTarget.HandleDestroyed += _textBoxTarget_HandleDestroyed;
			_textBoxTarget.Disposed += _textBoxTarget_Disposed;
			_textBoxTarget.Enter += OnTextBoxEntered;
			_textBoxTarget.Leave += OnTextBoxExit;
			_textBoxTarget.LostFocus+=OnTextBoxExit;
		}

		private void OnTextBoxExit(object sender, EventArgs e)
		{
			if(null != _pendingValueChange) //nb: string.emtpy is still a value we want to set!
			{
				SetTargetValue(_textBoxTarget.Text);
			}

		}

		private void _textBoxTarget_Disposed(object sender, EventArgs e)
		{
			TearDown();
		}

		private void _textBoxTarget_HandleDestroyed(object sender, EventArgs e)
		{
			TearDown();
		}

		private void OnTextBoxEntered(object sender, EventArgs e)
		{
			ChangeOfWhichItemIsInFocus(sender,
									   new CurrentItemEventArgs(DataTarget, _writingSystemId));
		}

		private void OnTextBoxChanged(object sender, EventArgs e)
		{
			_pendingValueChange = _textBoxTarget.Text;
			// can't afford to do this every keystroke when file gets large or computer slow:
			//          SetTargetValue(_textBoxTarget.Text);
		}

		/// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			if (null != _pendingValueChange)//nb: string.emtpy is still a value we want to set!
			{
				SetTargetValue(_pendingValueChange);
			}
			//Debug.WriteLine(" BindingTearDown boundTo: " + this._textBoxTarget.Name);

			if (_dataTarget == null)
			{
				return; //teardown was called twice
			}

			_dataTarget.PropertyChanged -= OnDataPropertyChanged;
			_dataTarget = null;
			_textBoxTarget.TextChanged -= OnTextBoxChanged;
			_textBoxTarget.HandleDestroyed -= _textBoxTarget_HandleDestroyed;
			_textBoxTarget.Disposed -= _textBoxTarget_Disposed;
			_textBoxTarget = null;
		}

		/// <summary>
		/// Respond to a change in the data object that we are attached to.
		/// </summary>
		protected virtual void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// _dataTarget should not normally be null. However, there is a strange case where it can
			// be: the teardown happens as a result of code which is also listening to the notifypropertychanged event
			// but which happens before this. Even though the event handler has been removed, it will
			// still fire this event.
			if (_dataTarget == null || _inMidstOfChange || e.PropertyName != _writingSystemId)
				//FIX THIS
			{
				return;
			}

			try
			{
				_inMidstOfChange = true;
				_textBoxTarget.Text = GetTargetValue();
			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		protected string GetTargetValue()
		{
			Debug.Assert(_dataTarget != null, "Perhaps the binding was already torn down?");
			MultiText text = _dataTarget as MultiText;
			if (text == null)
			{
				throw new ArgumentException("Binding can't handle that type of target.");
			}
			return text[_writingSystemId];
		}

		protected virtual void SetTargetValue(string s)
		{
			_pendingValueChange = null;

			Debug.Assert(_dataTarget != null, "Perhaps the binding was already torn down?");
			if (_inMidstOfChange)
			{
				return;
			}

			try
			{
				_inMidstOfChange = true;

				if (_dataTarget is MultiText)
				{
					MultiText text = (MultiText) _dataTarget;
					text[_writingSystemId] = s;
				}
					//else if (_dataTarget as IBindingList != null)
					//{
					//    IBindingList list = _dataTarget as IBindingList;
					//    //in addition to add a menu item, this will fire events on the object that owns the list
					//    list.AddNew();
					//}
				else
				{
					throw new ArgumentException("Binding doesn't understand that type of target.");
				}
			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		public INotifyPropertyChanged DataTarget
		{
			get { return _dataTarget; }
		}

		//protected string WritingSystemId
		//{
		//    get { return _writingSystemId; }
		//}
		//        public WeSayTextBox TextBoxTarget
		//        {
		//            get
		//            {
		//                return _textBoxTarget;
		//            }
		//        }
	}
}
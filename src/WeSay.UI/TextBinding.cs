using System;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.UI
{
	/// <summary>
	/// This simple binding class connects a text box with a MultiText.
	/// Changes in either one are reflected in the other.
	/// </summary>
	public class TextBinding
	{
		public event EventHandler<CurrentItemEventArgs> CurrentItemChanged = delegate
																			 {
																			 };
		private string _writingSystemId;
		private INotifyPropertyChanged _dataTarget;
		private WeSayTextBox _textBoxTarget;
		private bool _inMidstOfChange;

		public TextBinding(INotifyPropertyChanged dataTarget, string writingSystemId, WeSayTextBox widgetTarget)
		{
			System.Diagnostics.Debug.Assert(dataTarget != null);
			_dataTarget = dataTarget;
			_dataTarget.PropertyChanged += new PropertyChangedEventHandler(OnDataPropertyChanged);
			_writingSystemId = writingSystemId;
			_textBoxTarget = widgetTarget;
			_textBoxTarget.TextChanged += new EventHandler(OnTextBoxChanged);
			_textBoxTarget.HandleDestroyed += new EventHandler(_textBoxTarget_HandleDestroyed);
			_textBoxTarget.Enter += new EventHandler(OnTextBoxEntered);
		}

		void _textBoxTarget_HandleDestroyed(object sender, EventArgs e)
		{
			TearDown();
		}

		void OnTextBoxEntered(object sender, EventArgs e)
		{
			CurrentItemChanged(sender, new CurrentItemEventArgs(DataTarget, _writingSystemId));
		}


		void OnTextBoxChanged(object sender, EventArgs e)
		{
			SetTargetValue(_textBoxTarget.Text);
		}

		/// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			//Debug.WriteLine(" BindingTearDown boundTo: " + this._textBoxTarget.Name);

			if (_dataTarget == null)
			{
				return; //teardown was called twice
			}

			_dataTarget.PropertyChanged -= new PropertyChangedEventHandler(OnDataPropertyChanged);
			_dataTarget = null;
			_textBoxTarget.TextChanged -= new EventHandler(OnTextBoxChanged);
			_textBoxTarget.HandleDestroyed -= new EventHandler(_textBoxTarget_HandleDestroyed);
			_textBoxTarget = null;
		}

		/// <summary>
		/// Respond to a change in the data object that we are attached to.
		/// </summary>
		protected virtual void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_inMidstOfChange ||
				e.PropertyName != _writingSystemId) //FIX THIS
				return;

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
			System.Diagnostics.Debug.Assert(_dataTarget != null, "Perhaps the binding was already torn down?");
			MultiText text = _dataTarget as MultiText;
			if (text == null)
				throw new ArgumentException("Binding can't handle that type of target.");
			return text[_writingSystemId];
		}

		protected virtual void SetTargetValue(string s)
		{
			System.Diagnostics.Debug.Assert(_dataTarget != null, "Perhaps the binding was already torn down?");
			if (_inMidstOfChange)
				return;

			try
			{
				_inMidstOfChange = true;

				if (_dataTarget as MultiText != null)
				{
					MultiText text = _dataTarget as MultiText;
					text[_writingSystemId] = s;
				}
				//else if (_dataTarget as IBindingList != null)
				//{
				//    IBindingList list = _dataTarget as IBindingList;
				//    //in addition to add a menu item, this will fire events on the object that owns the list
				//    list.AddNew();
				//}
				else
					throw new ArgumentException("Binding doesn't understand that type of target.");

			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		public INotifyPropertyChanged DataTarget
		{
			get
			{
				return _dataTarget;
			}
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

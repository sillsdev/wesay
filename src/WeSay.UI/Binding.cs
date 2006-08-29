using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public class Binding
	{
		protected string _writingSystemId;
		protected INotifyPropertyChanged _dataTarget;
		private TextBox _textBoxTarget;
		protected bool _inMidstOfChange;

		public Binding(INotifyPropertyChanged dataTarget, string writingSystemId, TextBox widgetTarget)
		{
			_inMidstOfChange = false;
		   _dataTarget= dataTarget;
		   _dataTarget.PropertyChanged += new PropertyChangedEventHandler(OnDataPropertyChanged);
		   _writingSystemId=writingSystemId;
		   _textBoxTarget = widgetTarget;
		   _textBoxTarget.TextChanged += new EventHandler(OnTextBoxChanged);
		   _textBoxTarget.Disposed += new EventHandler(_textBoxTarget_Disposed);
		   _textBoxTarget.VisibleChanged += new EventHandler(_textBoxTarget_VisibleChanged);
	   }

		void _textBoxTarget_VisibleChanged(object sender, EventArgs e)
		{
			TearDown();
		}

		void _textBoxTarget_Disposed(object sender, EventArgs e)
		{
			TearDown();
		}

		void OnTextBoxChanged(object sender, EventArgs e)
		{
		   SetTargetValue(_textBoxTarget.Text);
		}

		private void TearDown()
		{
			_dataTarget.PropertyChanged -= new PropertyChangedEventHandler(OnDataPropertyChanged);
			_dataTarget = null;
			_textBoxTarget.TextChanged -= new EventHandler(OnTextBoxChanged);
			_textBoxTarget.Disposed -= new EventHandler(_textBoxTarget_Disposed);
			_textBoxTarget.VisibleChanged -= new EventHandler(_textBoxTarget_VisibleChanged);
			_textBoxTarget = null;
		}

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
			WeSay.Language.MultiText text = _dataTarget as WeSay.Language.MultiText;
			if (text == null)
				throw new ArgumentException("Binding can't handle that type of target.");
			return text[_writingSystemId];
		}

		protected virtual void SetTargetValue(string s)
		{
			if (_inMidstOfChange)
				return;

			try
			{
				_inMidstOfChange = true;

				if (_dataTarget as MultiText != null)
				{
					MultiText text = _dataTarget as WeSay.Language.MultiText;
					text[_writingSystemId] = s;
				}
				else if (_dataTarget as IBindingList != null)
				{
					IBindingList list = _dataTarget as IBindingList;
					//in addition to add a menu item, this will fire events on the object that owns the list
					list.AddNew();
				}
				else throw new ArgumentException("Binding doesn't understand that type of target.");

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

		protected string WritingSystemId
		{
			get { return _writingSystemId; }
		}
		public TextBox TextBoxTarget
		{
			get { return _textBoxTarget; }
		}
	}
}

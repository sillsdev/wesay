using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WeSay.UI
{
	public class Binding
	{
		private string _propertyName;
		private INotifyPropertyChanged _dataTarget;
		private Gtk.Entry _widgetTarget;
		private bool _inMidstOfChange;

		public Binding(INotifyPropertyChanged dataTarget, string propertyName, Gtk.Entry widgetTarget)
		{
			_inMidstOfChange = false;
		   _dataTarget= dataTarget;
		   _dataTarget.PropertyChanged += new PropertyChangedEventHandler(_dataTarget_PropertyChanged);
		   _propertyName=propertyName;
		   _widgetTarget = widgetTarget;
		   _widgetTarget.TextInserted += new Gtk.TextInsertedHandler(_widgetTarget_TextInserted);
		   _widgetTarget.TextDeleted +=new Gtk.TextDeletedHandler(_widgetTarget_TextDeleted);
		}

		void _widgetTarget_TextDeleted(object o, Gtk.TextDeletedArgs args)
		{
			 SetTargetValue(_widgetTarget.Text);

		}

		void _widgetTarget_TextInserted(object o, Gtk.TextInsertedArgs args)
		{
			SetTargetValue(_widgetTarget.Text);
		}

		void _dataTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_inMidstOfChange ||
				e.PropertyName != _propertyName)
				return;

			try
			{
				_inMidstOfChange = true;
				_widgetTarget.Text = GetTargetValue();
			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		private string GetTargetValue()
		{
			WeSay.Language.MultiText text = _dataTarget as WeSay.Language.MultiText;
			if (text == null)
				throw new ArgumentException("Binding can't handle that type of target.");
			return text[_propertyName];
		}

		private void SetTargetValue(string s)
		{
			if (_inMidstOfChange)
				return;

			try
			{
				_inMidstOfChange = true;

				WeSay.Language.MultiText text = _dataTarget as WeSay.Language.MultiText;
				if (text == null)
					throw new ArgumentException("Binding can't handle that type of target.");
				text[_propertyName] = s;

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

		protected string PropertyName
		{
			get { return _propertyName; }
		}
		public Gtk.Widget WidgetTarget
		{
			get { return _widgetTarget; }
		}
	}
}

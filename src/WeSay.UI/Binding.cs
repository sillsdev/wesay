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
		   _dataTarget.PropertyChanged += new PropertyChangedEventHandler(OnDataPropertyChanged);
		   _propertyName=propertyName;
		   _widgetTarget = widgetTarget;
		   _widgetTarget.TextInserted += new Gtk.TextInsertedHandler(OnWidgetTextInserted);
		   _widgetTarget.TextDeleted +=new Gtk.TextDeletedHandler(OnWidgetTextDeleted);
		}

		protected void OnWidgetTextDeleted(object o, Gtk.TextDeletedArgs args)
		{
			 SetTargetValue(_widgetTarget.Text);

		}

		protected void OnWidgetTextInserted(object o, Gtk.TextInsertedArgs args)
		{
			SetTargetValue(_widgetTarget.Text);
		}

		protected void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
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

		protected string GetTargetValue()
		{
			WeSay.Language.MultiText text = _dataTarget as WeSay.Language.MultiText;
			if (text == null)
				throw new ArgumentException("Binding can't handle that type of target.");
			return text[_propertyName];
		}

		protected void SetTargetValue(string s)
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

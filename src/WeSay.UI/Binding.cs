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
		private Gtk.Widget _widgetTarget;

		public Binding(INotifyPropertyChanged dataTarget, string propertyName, Gtk.Widget widgetTarget)
		{
		   _dataTarget= dataTarget;
		   _propertyName=propertyName;
		   _widgetTarget = widgetTarget;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Gtk;
using WeSay.Language;

namespace WeSay.UI
{
	public class GhostBinding
	{
		protected string _writingSystemId;
		protected string _propertyName;
		protected IBindingList _listTarget;
		private Gtk.Entry _widgetTarget;

		public delegate void GhostTriggered(object sender, EventArgs args);
		public event GhostTriggered Triggered;

		public GhostBinding(IBindingList targetList, string propertyName,  string writingSystemId, Entry widgetTarget)
		{

		   _listTarget= targetList;
		   _listTarget.ListChanged +=new ListChangedEventHandler(_listTarget_ListChanged);
		   _propertyName=propertyName;
		   _writingSystemId = writingSystemId;

		   _widgetTarget = widgetTarget;
		   _widgetTarget.FocusOutEvent += new FocusOutEventHandler(_widgetTarget_FocusOutEvent);
		}

		void _widgetTarget_FocusOutEvent(object o, FocusOutEventArgs args)
		{
			if(_widgetTarget.Text.Trim().Length>0)
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
				FillInMultiTextOfNewObject(_listTarget[e.NewIndex], _propertyName, _writingSystemId, _widgetTarget.Text);
				if (Triggered != null)
				{
					Triggered.Invoke(this, null);
				}
			}
		}

		protected  void OnWidgetTextDeleted(object o, Gtk.TextDeletedArgs args)
		{

		}

		protected  void OnWidgetTextInserted(object o, Gtk.TextInsertedArgs args)
		{
			TimeForRealObject();
		}

		protected  void TimeForRealObject()
		{
			_widgetTarget.FocusOutEvent -= new FocusOutEventHandler(_widgetTarget_FocusOutEvent);

			IBindingList list = _listTarget as IBindingList;
			//in addition to adding a list item, this will fire events on the object that owns the list
			list.AddNew();
		}

		private void FillInMultiTextOfNewObject(object o, string propertyName, string writingSystemId, string value)
		{
		   PropertyInfo info = o.GetType().GetProperty(propertyName);
		   MultiText text = (MultiText) info.GetValue(o, null);
		   text.SetAlternative(writingSystemId, value);
		}
	}
}

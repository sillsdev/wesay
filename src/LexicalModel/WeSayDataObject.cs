using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using com.db4o;

namespace WeSay.LexicalModel
{
	public class WeSayDataObject : INotifyPropertyChanged
	{
		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		[Transient]
		protected ArrayList _listEventHelpers;

		public void ObjectOnActivate(ObjectContainer container)
		{
			container.Activate(this, int.MaxValue);
			WireUpEvents();
		}

		protected void WireUpList<T>(BindingList<T> list, string listName) where T : new()
		{
			_listEventHelpers.Add(new ListEventHelper<T>(this, list, listName));
		}

		protected virtual void WireUpEvents()
		{
			_listEventHelpers = new ArrayList();
			PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SomethingWasModified();
		}

		internal void WireUpChild(INotifyPropertyChanged child)
		{
			child.PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
		}

		/// <summary>
		/// called by the binding list when senses are added, removed, reordered, etc.
		/// </summary>
		public virtual void SomethingWasModified()
		{
		}

		internal void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}
	}

	public class ListEventHelper<T> where T : new()
	{
		protected WeSayDataObject _parent;
		protected string _listName;

		public ListEventHelper(WeSayDataObject parent, BindingList<T> list, string listName)
		{
			_parent = parent;
			_listName = listName;
			list.AddingNew += new AddingNewEventHandler(OnAddingNewToAList);
			list.ListChanged += new ListChangedEventHandler(OnListChanged);
			foreach (INotifyPropertyChanged x in list)
			{
				x.PropertyChanged += new PropertyChangedEventHandler(OnListItemPropertyChanged);
			}
		}

		void OnListItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_parent.NotifyPropertyChanged(e.PropertyName);
		}

		void OnListChanged(object sender, ListChangedEventArgs e)
		{
			_parent.NotifyPropertyChanged(_listName);
		}

		///// <summary>
		///// Called by the binding list when a AddNew() is called on the list.
		///// </summary>
		void OnAddingNewToAList(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new T();
			_parent.WireUpChild((INotifyPropertyChanged)e.NewObject);
			_parent.NotifyPropertyChanged(this._listName);
			_parent.SomethingWasModified();
		}
	}
}

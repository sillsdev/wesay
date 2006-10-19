using System;
using System.Collections;
using System.ComponentModel;
using com.db4o;

namespace WeSay.LexicalModel
{
	public abstract class WeSayDataObject : INotifyPropertyChanged
	{
		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate
																	{
																	};

		[Transient]
		private ArrayList _listEventHelpers;

		[CLSCompliant(false)]
		public void ObjectOnActivate(ObjectContainer container)
		{
			container.Activate(this, int.MaxValue);
			WireUpEvents();
		}

		public abstract bool Empty{get;}

		protected void WireUpList(IBindingList list, string listName)
		{
			_listEventHelpers.Add(new ListEventHelper(this, list, listName));
		}

		protected virtual void WireUpEvents()
		{
			_listEventHelpers = new ArrayList();
			PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SomethingWasModified(e.PropertyName);
		}

		internal void WireUpChild(INotifyPropertyChanged child)
		{
			child.PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
		}

		/// <summary>
		/// called by the binding list when senses are added, removed, reordered, etc.
		/// </summary>
		public virtual void SomethingWasModified(string PropertyModified)
		{
		}

		internal void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}
	}

	public class ListEventHelper
	{
		private WeSayDataObject _parent;
		private string _listName;

		public ListEventHelper(WeSayDataObject parent, IBindingList list, string listName)
		{
			_parent = parent;
			_listName = listName;
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
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				IBindingList list = (IBindingList) sender;
				_parent.WireUpChild((INotifyPropertyChanged)list[e.NewIndex]);
//                _parent.SomethingWasModified(_listName);
			}
			_parent.NotifyPropertyChanged(_listName);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using WeSay.Foundation;

namespace WeSay.LexicalModel
{
	public abstract class WeSayDataObject : INotifyPropertyChanged
	{
		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate{};
		public event EventHandler EmptyObjectsRemoved = delegate{};

		private Dictionary<string,object> _properties;

		/// <summary>
		/// see comment on _parent field of MultiText for an explanation of this field
		/// </summary>
		private WeSayDataObject _parent;

		protected WeSayDataObject(WeSayDataObject parent)
		{
			_properties = new Dictionary<string, object>();
			_parent = parent;
		}

		[NonSerialized]
		private ArrayList _listEventHelpers;
//
//        [CLSCompliant(false)]
//        public void objectOnActivate(Db4objects.Db4o.IObjectContainer container)
//        {
//            container.Activate(this, int.MaxValue);
//            EmptyObjectsRemoved = delegate{};
//            WireUpEvents();
//        }

		/// <summary>
		/// Do the non-db40-specific parts of becoming activated
		/// </summary>
		public void FinishActivation()
		{
			EmptyObjectsRemoved = delegate{};
			WireUpEvents();
		}

		public abstract bool Empty{get;}

		/// <summary>
		/// see comment on _parent field of MultiText for an explanation of this field
		/// </summary>
		public WeSayDataObject Parent
		{
			get { return _parent; }
			set
			{
				System.Diagnostics.Debug.Assert(value != null);
				_parent = value;
			}
	   }

		protected Dictionary<string, object> Properties
		{
			get {
				if (_properties == null)
				{
					_properties = new Dictionary<string, object>();
				}

				return _properties;
			}
		}

		protected void WireUpList(IBindingList list, string listName)
		{
			_listEventHelpers.Add(new ListEventHelper(this, list, listName));
		}

		protected virtual void WireUpEvents()
		{
			_listEventHelpers = new ArrayList();
			PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
		}

		void OnEmptyObjectsRemoved(object sender, EventArgs e)
		{
			// perculate up
			EmptyObjectsRemoved(sender, e);
		}

		protected void OnEmptyObjectsRemoved() {
			EmptyObjectsRemoved(this, new EventArgs());
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SomethingWasModified(e.PropertyName);
		}

		internal void WireUpChild(INotifyPropertyChanged child)
		{
			child.PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
			if (child is WeSayDataObject)
			{
			   ((WeSayDataObject)child).EmptyObjectsRemoved += new EventHandler(OnEmptyObjectsRemoved);
			}
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

		public TContents GetProperty<TContents>(string fieldName) where TContents : class, new()
		{
			object val;
			if (Properties.TryGetValue(fieldName, out val))
			{
				return val as TContents;
			}
			TContents newGuy = new TContents();
			_properties.Add(fieldName, newGuy);

			return newGuy;
		}
	}

	/// <summary>
	/// This class enables creating the necessary event subscriptions. It was added
	/// before we were forced to add "parent" fields to everything.  I could probably
	/// be removed now, since that field could be used by children to cause the wiring,
	/// but we are hoping that the parent field might go away with future version of db4o.
	/// </summary>
	public class ListEventHelper
	{
		private WeSayDataObject _listOwner;
		private string _listName;

		public ListEventHelper(WeSayDataObject listOwner, IBindingList list, string listName)
		{
			_listOwner = listOwner;
			_listName = listName;
			list.ListChanged += new ListChangedEventHandler(OnListChanged);
			foreach (INotifyPropertyChanged x in list)
			{
				_listOwner.WireUpChild(x);
				//x.PropertyChanged += new PropertyChangedEventHandler(OnListItemPropertyChanged);
			}
		}

		void OnListItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_listOwner.NotifyPropertyChanged(e.PropertyName);
		}

		void OnListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				IBindingList list = (IBindingList) sender;
				INotifyPropertyChanged newGuy = (INotifyPropertyChanged)list[e.NewIndex];
				_listOwner.WireUpChild(newGuy);
				if (newGuy is WeSayDataObject)
				{
					((WeSayDataObject) newGuy).Parent =  this._listOwner;
				}
//                _parent.SomethingWasModified(_listName);
			}
			_listOwner.NotifyPropertyChanged(_listName);
		}

	}
}

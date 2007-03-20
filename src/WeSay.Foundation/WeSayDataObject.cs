using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.Foundation
{
	public interface IParentable
	{
		WeSayDataObject Parent
		{
			set;
		}
	}

	public abstract class WeSayDataObject : INotifyPropertyChanged
	{
		public class WellKnownProperties
		{
			static public string Note = "note";
		} ;

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate{};
		public event EventHandler EmptyObjectsRemoved = delegate{};

		// abandoned due to db4o difficulties
		//          private  Dictionary<string,object>  _properties;

		private List<KeyValuePair<string, object>> _properties;

		/// <summary>
		/// see comment on _parent field of MultiText for an explanation of this field
		/// </summary>
		private WeSayDataObject _parent;

		protected WeSayDataObject(WeSayDataObject parent)
		{
			_properties = new List<KeyValuePair<string, object>>();
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

		public List<KeyValuePair<string, object>> Properties
		{
			get {
				if (_properties == null)
				{
					_properties = new List<KeyValuePair<string, object>>();
					NotifyPropertyChanged("properties dictionary");
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

		public void WireUpChild(INotifyPropertyChanged child)
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
			RemoveEmptyProperties();
		}

		public void RemoveEmptyProperties()
		{
			// remove any custom fields that are empty
			int count = Properties.Count;

			for (int i = count - 1; i >= 0; i--)
			{
				if (IsPropertyEmpty(Properties[i].Value))
				{
					Properties.RemoveAt(i);
				}
			}
			if (count != Properties.Count)
			{
				OnEmptyObjectsRemoved();
			}
		}

		private bool IsPropertyEmpty(object property)
		{
			if (property is MultiText)
			{
				return ((MultiText)property).Empty;
			}
			else if (property is OptionRef)
			{
				return ((OptionRef)property).Empty;
			}
			else if (property is OptionRefCollection)
			{
				return ((OptionRefCollection)property).Empty;
			}
			Debug.Fail("Unknown property type");
			return true;
		}

		public bool HasProperties
		{
			get
			{
				foreach (KeyValuePair<string, object> pair in _properties)
				{
					if(!IsPropertyEmpty(pair.Value))
					{
						return true;
					}
				}
				return false;
			}
		}

		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}

		public TContents GetOrCreateProperty<TContents>(string fieldName) where TContents : class, IParentable, new()
		{
			TContents value = GetProperty<TContents>(fieldName);
			if (value != null)
			{
				return value;
			}

			TContents newGuy = new TContents();
			//Properties.Add(fieldName, newGuy);
			Properties.Add(new KeyValuePair<string, object>(fieldName, newGuy));
			newGuy.Parent = this;

			//temp hack until mt's use parents for notification
			if (newGuy is MultiText)
			{
				WireUpChild((INotifyPropertyChanged) newGuy);
			}

			return newGuy;
		}

		/// <summary>
		/// Will return null if not found
		/// </summary>
		/// <typeparam name="TContents"></typeparam>
		/// <returns></returns>
		public TContents GetProperty<TContents>(string fieldName) where TContents : class, IParentable
		{
			KeyValuePair<string, object> found = Properties.Find(delegate(KeyValuePair<string, object> p) { return p.Key == fieldName; });
			if (found.Key == fieldName)
			//if (Properties.Exists(delegate(KeyValuePair<string,object> p) { return p.Key == fieldName; }))
			//.TryGetValue(fieldName, out val))
			{
				//temp hack until mt's use parents for notification
				if (found.Value is MultiText)
				{
					WireUpChild((INotifyPropertyChanged)found.Value);
				}
				return found.Value as TContents;
			}
			return null;
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

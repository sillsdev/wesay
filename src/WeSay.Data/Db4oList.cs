using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Inside.Query;
using Db4objects.Db4o.Query;

namespace WeSay.Data
{
	[CLSCompliant(false)]
	public delegate IQuery SodaQueryProvider(IQuery query);

	/// <summary>
	/// db4o aware list with items stored in db4o database.
	/// Can be used to manipulate db4o query results.
	///
	/// *****************************************
	/// Author:  Marek Ištvánek (Marek Istvanek)
	///          Slušovice (Slusovice)
	///          Morava (Moravia)
	///          Èeská republika (Czech Republic)
	///          Marek.Istvanek@atlas.cz
	/// _________________________________________
	/// Version: 2006.07.07
	/// * Compiled with db4o 5.5.1.
	/// * Optimized item activation - item is not searched in read cache.
	/// * Added <see cref="StoreItemOnPropertyChanged"/> feature.
	/// * Added <see cref="Db4oList<T>.Refresh"/>, <see cref="RefreshAt"/> and <see cref="RequeryAndRefresh"/> method.
	/// * Bug removed: When <see cref="FilteringInDatabase"/> was true and <see cref="Db4objects.Db4o.Query.IQuery"/> was called, the result was endless loop.
	/// * <see cref="Rollback"/> does not requery anymore, but uses new cache of deleted item identifiers to add those items back to this list.
	/// _________________________________________
	/// Version: 2006.04.10
	/// * Compiled with db4o 5.2.2.
	///	* In database sorting added.
	/// * <see cref="FilteringInDatabase"/> default value changed to true.
	/// * GetDb4oFilter and GetSystemFilter methods (for converting filter from/to System to/from db4o predicate type) removed, because db4o now uses <see cref="Predicate{}"/>.
	/// _________________________________________
	/// Version: 2005.10.18
	/// *****************************************
	///
	/// </summary>
	/// <remarks>
	/// Only item`s db4o identifiers and fixed amount of activated objects are held in memory.
	/// </remarks>
	/// <typeparam name="T">List item type.</typeparam>
	[TypeConverter(typeof (ExpandableObjectConverter))]
	internal class Db4oList<T>
			: IList<T>, IDisposable
			where T : class
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="database">See <see cref="Database"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="database"/> is null.</exception>
		public Db4oList(IObjectContainer database)
		{
			if (database == null)
			{
				Dispose();
				throw new ArgumentNullException("database");
			}
			_database = database.Ext();
			ReadCacheSize = 1000;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>Calls <see cref="InitItems"/>.</remarks>
		/// <param name="database">See <see cref="Database"/>.</param>
		/// <param name="items">Collection of items to be added to the list.</param>
		/// <param name="filter">Initial <see cref="Filter"/>.</param>
		/// <param name="sorter">Initial <see cref="Sorter"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="database"/> is null.</exception>
		public Db4oList(IObjectContainer database, IEnumerable items,
						Predicate<T> filter, Comparison<T> sorter)
				: this(database)
		{
			InitItems(items, filter, sorter, false);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>Calls <see cref="InitItems"/>.</remarks>
		/// <param name="database">See <see cref="Database"/>.</param>
		/// <param name="itemIds">Collection of items to be added to the list.</param>
		/// <param name="filter">Initial <see cref="Filter"/>.</param>
		/// <param name="sorter">Initial <see cref="Sorter"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="database"/> is null.</exception>
		public Db4oList(IObjectContainer database, IList<long> itemIds,
						Predicate<T> filter, Comparison<T> sorter)
				: this(database)
		{
			if (itemIds == null)
			{
				Dispose();
				throw new ArgumentNullException("itemIds");
			}
			ItemIds.Clear();
			if (filter == null)
			{
				filter = ComparisonHelper<T>.DefaultPredicate;
			}
			_filter = filter;
			_itemIds.AddRange(itemIds);
			_initFilter = filter;
			_initSorter = sorter;
			_filterCalled = false;
			_sortCalled = false;
		}

		#endregion

		#region Events

		/// <summary>
		/// Fired when activating of <see cref="Db4oListEventArgs<T>.Item"/> by db4o is needed.
		/// If item was activated during the event, <see cref="Db4oListEventArgs<T>.Cancel"/> must be set to true.
		/// </summary>
		public event EventHandler<Db4oListEventArgs<T>> Activating = delegate {};

		/// <summary>
		/// Fired when deactivating of <see cref="Db4oListEventArgs<T>.Item"/> by db4o is needed.
		/// If item was deactivated during the event, <see cref="Db4oListEventArgs<T>.Cancel"/> must be set to true.
		/// </summary>
		public event EventHandler<Db4oListEventArgs<T>> Deactivating = delegate {};

		/// <summary>
		/// Fired when storing (adding or updating) of <see cref="Db4oListEventArgs<T>.Item"/> by db4o is needed.
		/// If item was stored during the event, <see cref="Db4oListEventArgs<T>.Cancel"/> must be set to true.
		/// </summary>
		public event EventHandler<Db4oListEventArgs<T>> Storing = delegate {};

		/// <summary>
		/// Fired when deleting of <see cref="Db4oListEventArgs<T>.Item"/> (which is not activated) by db4o is needed.
		/// If item was deleted during the event, <see cref="Db4oListEventArgs<T>.Cancel"/> must be set to true.
		/// </summary>
		public event EventHandler<Db4oListEventArgs<T>> Deleting = delegate {};

		/// <summary>
		/// Fires <see cref="Activating"/> event.
		/// </summary>
		/// <param name="item">List item.</param>
		/// <returns>true, if <paramref name="item"/> was activated.</returns>
		protected virtual bool OnActivating(T item)
		{
			bool cancel;
			Db4oListEventArgs<T> args = new Db4oListEventArgs<T>(item);
			Activating(this, args);
			cancel = args.Cancel;
			return cancel;
		}

		/// <summary>
		/// Fires <see cref="Deactivating"/> event.
		/// </summary>
		/// <param name="item">List item.</param>
		/// <returns>true, if <paramref name="item"/> was deactivated.</returns>
		protected virtual bool OnDeactivating(T item)
		{
			bool cancel;
			Db4oListEventArgs<T> args = new Db4oListEventArgs<T>(item);
			Deactivating(this, args);
			cancel = args.Cancel;
			return cancel;
		}

		/// <summary>
		/// Fires <see cref="Storing"/> event.
		/// </summary>
		/// <param name="item">List item.</param>
		/// <returns>true, if <paramref name="item"/> was stored.</returns>
		protected virtual bool OnStoring(T item, string propertyName)
		{
			bool cancel;
			Db4oListEventArgs<T> args = new Db4oListEventArgs<T>(item, propertyName);
			Storing(this, args);
			cancel = args.Cancel;
			return cancel;
		}

		/// <summary>
		/// Fires <see cref="Deleting"/> event.
		/// </summary>
		/// <param name="item">List item.</param>
		/// <returns>true, if <paramref name="item"/> was deleted.</returns>
		protected virtual bool OnDeleting(T item)
		{
			bool cancel;
			Db4oListEventArgs<T> args = new Db4oListEventArgs<T>(item);
			Deleting(this, args);
			cancel = args.Cancel;
			return cancel;
		}

		#endregion

		/// <summary>
		/// List of items` db4o identifiers.
		/// </summary>
		/// <remarks>
		/// Direct manipulation of this list is not validated.
		/// If invalid id (not associated with any db4o stored object) is stored in this list, <see cref="InvalidOperationException"/> will be thrown in some operations done by this class IList implementation (get indexer etc.).
		/// </remarks>
		public List<long> ItemIds
		{
			get
			{
				VerifyNotDisposed();
				return _itemIds;
			}
						set
			{
				VerifyNotDisposed();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_itemIds = value;
			}

		}

		/// <summary>
		/// db4o database object container used for items manipulation.
		/// </summary>
		public IExtObjectContainer Database
		{
			get
			{
				VerifyNotDisposed();
				return _database;
			}
		}

		/// <summary>
		/// Maximum number of db4o activated items in memory.
		/// </summary>
		/// <remarks>
		/// When activating an item, oldest activated item is deactivated, when read cache size is reached.
		/// References to activated items are not held in read cache, only their db4o ids.
		/// </remarks>
		/// <value>
		/// When 0, read cache is disabled and item deactivating is never done.
		/// Default: 1000.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException">set: Value is &lt; 0.</exception>
		public int ReadCacheSize
		{
			get
			{
				VerifyNotDisposed();
				return _readCacheSize;
			}
			set
			{
				VerifyNotDisposed();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("ReadCacheSize", value, "Must be >= 0.");
				}
				if (_readCacheSize == value)
				{
					return;
				}
				_readCacheSize = value;
				DeactivateExcessItems(0);
				_readCache.TrimExcess();
			}
		}

		/// <summary>
		/// Maximum number of changes (item added, deleted, changed) before <see cref="Commit"/> is called automatically.
		/// </summary>
		/// <value>
		/// When 0, <see cref="Commit"/> is never called automatically.
		/// Default: 100.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException">set: Value is &lt; 0.</exception>
		public int WriteCacheSize
		{
			get
			{
				VerifyNotDisposed();
				return _writeCacheSize;
			}
			set
			{
				VerifyNotDisposed();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("WriteCacheSize", value, "Must be >= 0.");
				}
				if (_writeCacheSize == value)
				{
					return;
				}
				_writeCacheSize = value;
				CommitFullCache();
			}
		}

		/// <summary>
		/// Activation/deactivation depth used during item activation/deactivation when no custom activation/deactivation is done using <see cref="Activating"/>/<see cref="Deactivating"/> events.
		/// </summary>
		/// <value>Default: 5.</value>
		/// <exception cref="ArgumentOutOfRangeException">set: Value is &lt; 0.</exception>
		public int ActivationDepth
		{
			get
			{
				VerifyNotDisposed();
				return _activationDepth;
			}
			set
			{
				VerifyNotDisposed();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("ActivationDepth", value, "Must be >= 0.");
				}
				_activationDepth = value;
			}
		}

		/// <summary>
		/// Activation depth used when calling <see cref="Db4oList<T>[]"/>.
		/// </summary>
		/// <value>Default: 5.</value>
		/// <exception cref="ArgumentOutOfRangeException">set: Value is &lt; 0.</exception>
		public int SetActivationDepth
		{
			get { return _setActivationDepth; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("SetActivationDepth", value, "Must be >= 0.");
				}
				_setActivationDepth = value;
			}
		}

		/// <summary>
		/// Activation (instantiation) depth used when calling <see cref="IExtObjectContainer.PeekPersisted"/>.
		/// </summary>
		/// <remarks>
		/// See <see cref="Db4oList<T>[]"/>.
		/// Used during item comparison using <see cref="Equaler"/>.
		/// </remarks>
		/// <value>Default: 5.</value>
		/// <exception cref="ArgumentOutOfRangeException">set: Value is &lt;= 0.</exception>
		public int PeekPersistedActivationDepth
		{
			get
			{
				VerifyNotDisposed();
				return _peekPersistedActivationDepth;
			}
			set
			{
				VerifyNotDisposed();
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("PeekPersistedActivationDepth", value, "Must be > 0.");
				}
				_peekPersistedActivationDepth = value;
			}
		}

		/// <summary>
		/// Activation depth used when calling <see cref="Refresh(T)"/>.
		/// </summary>
		/// <value>Default: 5.</value>
		/// <exception cref="ArgumentOutOfRangeException">set: Value is &lt;= 0.</exception>
		public int RefreshActivationDepth
		{
			get
			{
				VerifyNotDisposed();
				return _peekPersistedActivationDepth;
			}
			set
			{
				VerifyNotDisposed();
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("PeekPersistedActivationDepth", value, "Must be > 0.");
				}
				_peekPersistedActivationDepth = value;
			}
		}

		/// <summary>
		/// Gets whether the list has some changes which can be committed to or rollbacked from <see cref="Database"/>.
		/// </summary>
		public bool HasChanges
		{
			get
			{
				VerifyNotDisposed();
				return _writeCacheCurrentSize > 0;
			}
		}

		/// <summary>
		/// Whether to store item to database when its <see cref="INotifyPropertyChanged.PropertyChanged"/> is fired.
		/// </summary>
		/// <remarks>
		/// If enabled, it only works when <typeparamref name="T"/> implements <see cref="INotifyPropertyChanged"/> interface and only for items in activation cache, ie. for <see cref="ReadCacheSize"/> items. Local <see cref="INotifyPropertyChanged.PropertyChanged"/> event handler is registered when item is activated and unregistered when item is deactivated (so that garbage collector can free deactivated items). It means that when <see cref="ReadCacheSize"/> is 0, event handler stays registered still.
		/// If this property is changed from false to true (true to false) handler is (un)registered in items which are in activation cache. But when <see cref="ReadCacheSize"/> is 0, handler is not (un)registered, because activation cache is not used (and instantiating/activating all items is not desired) - in this case set this property to true before getting any item from this list.
		/// </remarks>
		/// <value>Default: true.</value>
		public bool StoreItemOnPropertyChanged
		{
			get
			{
				VerifyNotDisposed();
				return _storeItemOnPropertyChanged;
			}
			set
			{
				VerifyNotDisposed();
				if (_storeItemOnPropertyChanged == value)
				{
					return;
				}
				_storeItemOnPropertyChanged = value;
				foreach (long id in _readCache)
				{
					RegisterItemPropertyChangedHandler(GetItem(id, false), value);
				}
			}
		}

		/// <summary>
		/// Initializes list from <paramref name="items"/> collection.
		/// </summary>
		/// <param name="items">
		/// Collection of items to be added to the list.
		/// If the collection comes from db4o query (IObjectSet or native query IList`1), no objects are instantiated and activated.
		/// </param>
		/// <param name="filter">
		/// Initial <see cref="Filter"/>.
		/// <paramref name="items"/> collection will be filtered using this filter.
		/// If null, <see cref="ComparisonHelper{}.DefaultPredicate"/> is used.
		/// If <paramref name="items"/> collection comes from db4o query, the filter should be the query constrain/predicate/filter.
		/// </param>
		/// <param name="sorter">
		/// Initial <see cref="Sorter"/>.
		/// <paramref name="items"/> collection will be sorted using this sorter if it is not null.
		/// If <paramref name="items"/> collection comes from db4o query, the sorter should be the query sorter.
		/// </param>
		/// <param name="commit">Whether to call <see cref="Commit"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
		public void InitItems(IEnumerable items,
									  Predicate<T> filter, Comparison<T> sorter, bool commit)
		{
			VerifyNotDisposed();

			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (commit)
			{
				Commit();
			}
			ItemIds.Clear();
			// db4o query collection
			IEnumerable<long> ids = GetIds(items);
			bool fromDatabase = ids != null;
			if (filter == null)
			{
				filter = ComparisonHelper<T>.DefaultPredicate;
			}
			_filter = filter;
			if (fromDatabase)
			{
				_itemIds.AddRange(ids);
			}
					// any enumerable collection
			else
			{
				foreach (T item in items)
				{
					try
					{
						Add(item);
					}
					catch (ArgumentException) {}
				}
			}
			// sorting is called only in !fromDatabase case, otherwise it was done in db4o query
			if (sorter != null && !fromDatabase)
			{
				Sort(sorter);
			}
			_initFilter = filter;
			_initSorter = sorter;
			_filterCalled = false;
			_sortCalled = false;
		}

		/// <summary>
		/// Performs db4o native query and calls <see cref="InitItems"/> with query results.
		/// </summary>
		/// <param name="filter">
		/// Query filtering predicate (like <see cref="Filter"/>).
		/// If null, <see cref="ComparisonHelper<T>.DefaultPredicate"/> is used.
		/// </param>
		/// <param name="sorter">Query sorting (like <see cref="Sorter"/>). If null, no sorting is done.</param>
		/// <param name="commit">Whether to call <see cref="Commit"/>.</param>
		public void Query(Predicate<T> filter, Comparison<T> sorter, bool commit)
		{
			VerifyNotDisposed();
			IEnumerable list;

			if (_SODAQuery != null)
			{
				IQuery query = _SODAQuery(Database.Query());
				list = query.Execute();
			}
			else
			{
				if (filter == null)
				{
					filter = ComparisonHelper<T>.DefaultPredicate;
				}
				if (sorter == null)
				{
					if (filter == ComparisonHelper<T>.DefaultPredicate)
					{
						list = Database.Query<T>();
					}
					else
					{
						list = Database.Query<T>(filter);
					}
				}
				else
				{
					list = Database.Query<T>(filter, sorter);
				}
			}
			InitItems(list, filter, sorter, commit);
		}

		public SodaQueryProvider SODAQuery
		{
			get
			{
				VerifyNotDisposed();
				return _SODAQuery;
			}
			set
			{
				VerifyNotDisposed();
				_SODAQuery = value;
				if (_SODAQuery != null)
				{
					Query(null, null, true);
				}
			}
		}

		private SodaQueryProvider _SODAQuery;

		/// <summary>
		/// Instantiates new <see cref="Db4oList{T}"/> object and calls its <see cref="Db4objects.Db4o.Query.IQuery"/> method without committing anything.
		/// </summary>
		/// <param name="database">See <see cref="Database"/>.</param>
		/// <param name="filter">Query filtering predicate (like <see cref="Filter"/>).</param>
		/// <param name="sorter">Query sorting (like <see cref="Sorter"/>).</param>
		/// <returns>List.</returns>
		public static Db4oList<T> Query(IObjectContainer database, Predicate<T> filter, Comparison<T> sorter)
		{
			Db4oList<T> list = new Db4oList<T>(database);
			list.Query(filter, sorter, false);
			return list;
		}

		/// <summary>
		/// Calls <see cref="Db4objects.Db4o.Query.IQuery"/> method with current <see cref="Filter"/> and <see cref="Sorter"/>.
		/// </summary>
		/// <param name="commit">Whether to call <see cref="Commit"/>.</param>
		public void Requery(bool commit)
		{
			VerifyNotDisposed();
			Query(Filter, Sorter, commit);
		}

		/// <summary>
		/// Calls <see cref="RefreshAt(int)"/> for each item.
		/// </summary>
		public void Refresh()
		{
			VerifyNotDisposed();
			for (int i = Count - 1; i >= 0; i--)
			{
				RefreshAt(i);
			}
		}

		/// <summary>
		/// Calls <see cref="Refresh(T)"/> for item at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">Index of item to be refreshed.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is invalid.</exception>
		public void RefreshAt(int index)
		{
			VerifyNotDisposed();
			T item = GetItem(ItemIds[index], false);
			Refresh(item);
		}

		/// <summary>
		/// Calls <see cref="IExtObjectContainer.Refresh"/> for <paramref name="item"/> with <see cref="RefreshActivationDepth"/>.
		/// </summary>
		/// <param name="item">Item to be refreshed.</param>
		public void Refresh(T item)
		{
			VerifyNotDisposed();
			if (!FilteringInDatabase)
			{
				if (!Filter(item))
				{
					ItemIds.Remove(GetItemId(item));
					RegisterItemPropertyChangedHandler(item, false);
					return;
				}
			}
			Database.Refresh(item, RefreshActivationDepth);
		}

		/// <summary>
		/// Calls <see cref="Requery"/> and then <see cref="Refresh()"/>.
		/// </summary>
		/// <param name="commit">Passed to <see cref="Requery"/>.</param>
		public void RequeryAndRefresh(bool commit)
		{
			VerifyNotDisposed();
			Requery(commit);
			Refresh();
		}

		/// <summary>
		/// Commits previously made changes to <see cref="Database"/>.
		/// </summary>
		/// <remarks>If <see cref="HasChanges"/> is false, does nothing.</remarks>
		/// <returns>true, if any changes were committed.</returns>
		public virtual bool Commit()
		{
			VerifyNotDisposed();
			bool hasChanges = HasChanges;
			if (hasChanges)
			{
				Database.Commit();
				_writeCacheCurrentSize = 0;
				_deleteCache.Clear();
			}
			return hasChanges;
		}

		private void SetDatabaseLastModified()
		{
			IList<DatabaseModified> modifiedList = Database.Query<DatabaseModified>();
			DatabaseModified databaseModified;
			int count = modifiedList.Count;
			while(count > 1)
			{
				DatabaseModified modified = modifiedList[1];
				Database.Delete(modified);
				modifiedList.RemoveAt(1);
			}
			if (modifiedList.Count == 1)
			{
				databaseModified = modifiedList[0];
			}
			else
			{
				databaseModified = new DatabaseModified();
			}
			databaseModified.LastModified = DateTime.Now;
			Database.Set(databaseModified);
		}

		/// <summary>
		/// Rollbacks (voids) previously (from last <see cref="Commit"/> call which returned true or constructor call) made changes in <see cref="Database"/>, removes previously added items, adds previously removed items (to the end of list) and refreshes all items in list by calling <see cref="Refresh()"/>.
		/// </summary>
		/// <remarks>If <see cref="HasChanges"/> is false, does nothing.</remarks>
		/// <returns>true, if any changes were rollbacked.</returns>
		public virtual bool Rollback()
		{
			VerifyNotDisposed();
			bool hasChanges = HasChanges;
			if (hasChanges)
			{
				Database.Rollback();
				_writeCacheCurrentSize = 0;
				object item;
				int count = _deleteCache.Count;
				for (int i = 0; i < count; i++)
				{
					ItemIds.Add(_deleteCache.Dequeue());
				}
				for (int i = Count - 1; i >= 0; i--)
				{
					item = GetItem(ItemIds[i], false);
					if (item == null)
					{
						ItemIds.RemoveAt(i);
					}
				}
				Refresh();
			}
			return hasChanges;
		}

		/// <summary>
		/// Does <paramref name="action"/> with each item in this list.
		/// </summary>
		/// <param name="action">Action to be done.</param>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
		public void ForEach(Action<T> action)
		{
			VerifyNotDisposed();
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T item in this)
			{
				action(item);
			}
		}

		/// <summary>
		/// Does <paramref name="action"/> with each item identifier (see <see cref="ItemIds"/>) in this list.
		/// </summary>
		/// <param name="action">Action to be done.</param>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
		public void ForEach(Action<long> action)
		{
			VerifyNotDisposed();
			_itemIds.ForEach(action);
		}

		/// <summary>
		/// Copies specified items to <paramref name="array"/>.
		/// </summary>
		/// <param name="index">Starting index of this list.</param>
		/// <param name="array">Target array.</param>
		/// <param name="arrayIndex">Starting index of target array from which items will be assigned.</param>
		/// <param name="count">Number of items to copy from this list.</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> does not have enough space or is multidimensional.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is &lt; 0 or &gt;= <see cref="Count"/>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="count"/> is &lt;= 0 or is bigger then available items from <paramref name="index"/> to the end of the list.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is &lt; 0.</exception>
		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			VerifyNotDisposed();
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Must be >= 0.");
			}
			if (index >= Count)
			{
				throw new ArgumentOutOfRangeException("index", index, "Must be < Count.");
			}
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "Must be > 0.");
			}
			if (count > Count - index)
			{
				throw new ArgumentOutOfRangeException("count", count, "So many items are not available.");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Must be one dimensional.", "array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Must be >= 0.");
			}
			if (array.Length - arrayIndex < count)
			{
				throw new ArgumentException("Does not have enough space.", "array");
			}
			for (int i = 0; i < count; i++)
			{
				array[arrayIndex + i] = this[index + i];
			}
		}

		/// <summary>
		/// Removes a list change.
		/// </summary>
		/// <param name="change">List change to remove. Can be more than one change "or"ed together.</param>
		/// <returns>true, if something in the list was changed.</returns>
		public virtual bool RemoveChange(Db4oListChanges change)
		{
			VerifyNotDisposed();
			bool ret = false;
			if (IsChange(change, Db4oListChanges.Filter) && IsChange(change, Db4oListChanges.Sort))
			{
				if (
						_filterCalled && _initFilter != Filter ||
						_sortCalled && _initSorter != Sorter)
				{
					Query(_initFilter, _initSorter, false);
					ret = true;
				}
			}
			else if (IsChange(change, Db4oListChanges.Filter))
			{
				if (_filterCalled && _initFilter != Filter)
				{
					Comparison<T> sorter = _initSorter;
					bool sortCalled = _sortCalled;
					Query(_initFilter, Sorter, false);
					_initSorter = sorter;
					_sortCalled = sortCalled;
					ret = true;
				}
				_filterCalled = false;
			}
			else if (IsChange(change, Db4oListChanges.Sort))
			{
				if (_sortCalled && Count > 0 && _initSorter != Sorter)
				{
					if (_initSorter != null)
					{
						Sort(_initSorter);
					}
					else
					{
						Predicate<T> filter = _initFilter;
						bool filterCalled = _filterCalled;
						Query(Filter, _initSorter, false);
						_initFilter = filter;
						_filterCalled = filterCalled;
					}
					ret = true;
				}
				_sortCalled = false;
			}
			return ret;
		}

		/// <summary>
		/// (Un)Registers local event handler which stores changed item to database.
		/// </summary>
		/// <remarks>
		/// If <paramref name="item"/> does not implement <see cref="INotifyPropertyChanged"/> interface, nothing is done.
		/// </remarks>
		/// <param name="item">Activated item with <see cref="INotifyPropertyChanged.PropertyChanged"/> event.</param>
		/// <param name="register">true to register handler, false to unregister it.</param>
		public void RegisterItemPropertyChangedHandler(T item, bool register)
		{
			VerifyNotDisposed();
			INotifyPropertyChanged localItem = item as INotifyPropertyChanged;
			if (localItem == null)
			{
				return;
			}
			localItem.PropertyChanged -= Item_PropertyChanged;
			if (register)
			{
				localItem.PropertyChanged += Item_PropertyChanged;
			}
		}

		#region Finding

		/// <summary>
		/// Finds index of first item satisfying <paramref name="match"/> condition.
		/// </summary>
		/// <param name="startIndex">Index to start from.</param>
		/// <param name="count">Number of items to search from <paramref name="startIndex"/>.</param>
		/// <param name="match">Find condition.</param>
		/// <returns>Index of found item otherwise -1.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is invalid or
		/// <paramref name="count"/> is &lt; 0 or
		/// <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the list.
		/// </exception>
		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			VerifyNotDisposed();
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			return _itemIds.FindIndex(startIndex, count, GetIdPredicate(match));
		}

		/// <summary>
		/// Calls <see cref="Db4oList<T>.FindIndex(int, int, Predicate<T>)"/> with count representing number of items from <paramref name="startIndex"/> to the end of the list.
		/// </summary>
		public int FindIndex(int startIndex, Predicate<T> match)
		{
			VerifyNotDisposed();
			return FindIndex(startIndex, Count - startIndex, match);
		}

		/// <summary>
		/// Calls <see cref="Db4oList<T>.FindIndex(int, Predicate<T>)"/> with startIndex set to 0 and count to <see cref="Count"/>.
		/// </summary>
		public int FindIndex(Predicate<T> match)
		{
			VerifyNotDisposed();
			return FindIndex(0, match);
		}

		#endregion

		#region Filtering

		public bool IsFiltered
		{
			get
			{
				VerifyNotDisposed();
				return _filter != ComparisonHelper<T>.DefaultPredicate;
			}
		}

		/// <summary>
		/// Filter used for filtering (removing) and validating items.
		/// </summary>
		/// <remarks>
		/// Only items satisfying the filter can be in the list.
		/// set: Items not satisfying the filter are removed from the list and optionally (<see cref="DeleteItemsWhileFiltering"/>) deleted from <see cref="Database"/>.
		/// </remarks>
		/// <value>set: If null, <see cref="ComparisonHelper{}.DefaultPredicate"/> is used.</value>
		public Predicate<T> Filter
		{
			get
			{
				VerifyNotDisposed();
				return _filter;
			}
			set
			{
				VerifyNotDisposed();
				if (value == null)
				{
					value = ComparisonHelper<T>.DefaultPredicate;
				}
				if (_filter == value)
				{
					return;
				}
				if (Count > 0 && (!FilteringInDatabase || DeleteItemsWhileFiltering))
				{
					Predicate<T> removeFilter = ComparisonHelper<T>.GetInversePredicate(value);
					if (DeleteItemsWhileFiltering)
					{
						removeFilter = delegate(T item)
									   {
										   bool remove = removeFilter(item);
										   if (remove && GetItemId(item) > 0)
										   {
											   DeleteItem(item);
										   }
										   return remove;
									   };
					}
					_itemIds.RemoveAll(GetIdPredicate(removeFilter));
				}
				if (FilteringInDatabase)
				{
					Predicate<T> initFilter = _initFilter;
					Comparison<T> initSorter = _initSorter;
					bool sortCalled = _sortCalled;
					Query(value, Sorter, false);
					_initFilter = initFilter;
					_initSorter = initSorter;
					_sortCalled = sortCalled;
				}
				_filterCalled = true;
				_filter = value;
			}
		}

		/// <summary>
		/// If true, when filtering, all removed items are deleted from <see cref="Database"/>.
		/// </summary>
		/// <value>Default: false.</value>
		public bool DeleteItemsWhileFiltering
		{
			get
			{
				VerifyNotDisposed();
				return _deleteItemsWhileFiltering;
			}
			set
			{
				VerifyNotDisposed();
				_deleteItemsWhileFiltering = value;
			}
		}

		/// <summary>
		/// If true, filtering is done using <see cref="Database"/> by <see cref="Db4objects.Db4o.Query.IQuery"/>ing with current <see cref="Sorter"/>.
		/// If false, it is done in memory.
		/// </summary>
		/// <value>Default: true.</value>
		public bool FilteringInDatabase
		{
			get
			{
				VerifyNotDisposed();
				return _filteringInDatabase;
			}
			set
			{
				VerifyNotDisposed();
				_filteringInDatabase = value;
			}
		}

		/// <summary>
		/// Calls <see cref="RemoveChange"/> with <see cref="Db4oListChanges.Filter"/> to remove any filtering done by setting <see cref="Filter"/>.
		/// </summary>
		public bool RemoveFilter()
		{
			VerifyNotDisposed();
			return RemoveChange(Db4oListChanges.Filter);
		}

		/// <summary>
		/// Gets predicate operating on item ids.
		/// </summary>
		/// <param name="predicate">Predicate operating on item.</param>
		/// <returns>Id predicate.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null.</exception>
		protected Predicate<long> GetIdPredicate(Predicate<T> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return delegate(long id)
				   {
					   T item = GetItem(id, true);
					   if (item == null)
					   {
						   return false;
					   }
					   else
					   {
						   return predicate(item);
					   }
				   };
		}

		#endregion

		#region Equality

		/// <summary>
		/// Compares items for equality.
		/// </summary>
		/// <remarks>See <see cref="Db4oList<T>.Item"/>.</remarks>
		/// <value>Default: <see cref="ComparisonHelper{}.DefaultEqualityComparison"/>.</value>
		public EqualityComparison<T> Equaler
		{
			get
			{
				VerifyNotDisposed();
				return _equaler;
			}
			set
			{
				VerifyNotDisposed();
				_equaler = value;
			}
		}

		#endregion

		#region Sorting

		/// <summary>
		/// Sorter applied by calling <see cref="Sort"/> or query.
		/// </summary>
		/// <value>If null, items are not sorted.</value>
		public Comparison<T> Sorter
		{
			get
			{
				VerifyNotDisposed();
				return _sorter;
			}
		}

		/// <summary>
		/// If true, sorting is done using <see cref="Database"/> by <see cref="Db4objects.Db4o.Query.IQuery"/>ing with current <see cref="Filter"/>.
		/// If false, it is done in memory.
		/// See <see cref="Sort"/>.
		/// </summary>
		/// <value>Default: true.</value>
		public bool SortingInDatabase
		{
			get
			{
				VerifyNotDisposed();
				return _sortingInDatabase;
			}
			set
			{
				VerifyNotDisposed();
				_sortingInDatabase = value;
			}
		}

		public bool IsSorted
		{
			get
			{
				VerifyNotDisposed();
				return _sortCalled;
			}
		}

		/// <summary>
		/// Sorts items using specified <paramref name="sorter"/> (only when <see cref="Count"/> &gt; 1).
		/// </summary>
		/// <remarks><see cref="Sorter"/> reflects used sorter after sorting is done.</remarks>
		/// <param name="sorter">Sorter. If null, <see cref="ComparisonHelper{}.DefaultComparison"/> is used.</param>
		public void Sort(Comparison<T> sorter)
		{
			VerifyNotDisposed();
			if (Count <= 1)
			{
				return;
			}
			if (sorter == null)
			{
				sorter = ComparisonHelper<T>.DefaultComparison;
			}
			if (SortingInDatabase)
			{
				Predicate<T> initFilter = _initFilter;
				Comparison<T> initSorter = _initSorter;
				bool filterCalled = _filterCalled;
				Query(Filter, sorter, false);
				_initFilter = initFilter;
				_initSorter = initSorter;
				_filterCalled = filterCalled;
			}
			else
			{
				_itemIds.Sort(GetIdSorter(sorter));
			}
			_sortCalled = true;
			_sorter = sorter;
		}

		/// <summary>
		/// Calls <see cref="RemoveChange"/> with <see cref="Db4oListChanges.Sort"/> to remove any filtering done by calling <see cref="Sort"/>.
		/// </summary>
		public bool RemoveSort()
		{
			VerifyNotDisposed();
			return RemoveChange(Db4oListChanges.Sort);
		}

		/// <summary>
		/// Gets sorter operating on item ids.
		/// </summary>
		/// <param name="sorter">Sorter operating on item.</param>
		/// <returns>Id sorter.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="sorter"/> is null.</exception>
		protected Comparison<long> GetIdSorter(Comparison<T> sorter)
		{
			if (sorter == null)
			{
				throw new ArgumentNullException("sorter");
			}
			return delegate(long id1, long id2)
				   {
					   T item1 = GetItem(id1, true);
					   T item2 = GetItem(id2, true);
					   if (item1 == null && item2 != null)
					   {
						   return -1;
					   }
					   else if (item1 != null && item2 == null)
					   {
						   return 1;
					   }
					   else if (item1 == null && item2 == null)
					   {
						   return 0;
					   }
					   else
					   {
						   return sorter(item1, item2);
					   }
				   };
		}

		#endregion

		#region IList<T> Members

		/// <summary>
		/// See <see cref="IList<T>.IndexOf"/>.
		/// </summary>
		public int IndexOf(T item)
		{
			VerifyNotDisposed();
			int index = -1;
			//try
			//{
				//ValidateItem(item);
				long id = GetItemId(item);
				if (id > 0)
				{
					index = ItemIds.IndexOf(id);
				}
			//}
			//catch (ArgumentException) {}
			return index;
		}

		/// <summary>
		/// See <see cref="IList<T>.Insert"/>.
		/// </summary>
		/// <remarks>Item is also stored to <see cref="Database"/>.</remarks>
		public void Insert(int index, T item)
		{
			VerifyNotDisposed();
			ItemIds.Insert(index, 0);
			try
			{
				this[index] = item;
			}
			catch (ArgumentException)
			{
				ItemIds.RemoveAt(index);
				throw;
			}
		}

		/// <summary>
		/// See <see cref="IList<T>.RemoveAt"/>.
		/// </summary>
		/// <remarks>Item is also deleted from <see cref="Database"/>.</remarks>
		public void RemoveAt(int index)
		{
			VerifyNotDisposed();
			long id = ItemIds[index];
			ItemIds.RemoveAt(index);
			DeleteItem(id, true);
		}

		/// <summary>
		/// See <see cref="IList<T>.Item"/>.
		/// List items indexer.
		/// </summary>
		/// <remarks>
		/// set: Item is also stored to <see cref="Database"/> when it is not already stored there or it is different than already stored one (got by calling <see cref="IExtObjectContainer.PeekPersisted"/>), while the comparing is done using <see cref="Equaler"/>. If <see cref="Equaler"/> is null, item is always stored.
		/// If an item previously stored at specified <paramref name="index"/> exists only ones in this list, it is deleted from <see cref="Database"/> before setting new one.
		/// </remarks>
		/// <param name="index">Index of list item.</param>
		/// <returns>Item.</returns>
		public T this[int index]
		{
			get
			{
				VerifyNotDisposed();
				long id = ItemIds[index];
				T item = GetItem(id, true);
				return item;
			}
			set
			{
				VerifyNotDisposed();
				ValidateItem(value);
				long oldId = ItemIds[index];
				bool isStored = Database.IsStored(value);
				if (!isStored)
				{
					StoreItem(value, string.Empty);
				}
				long newId = GetItemId(value);
				if (isStored)
				{
					bool storeAgain = Equaler == null;
					if (!storeAgain)
					{
						T oldValue = (T) Database.PeekPersisted(value, PeekPersistedActivationDepth, false);
						storeAgain = !Equaler(oldValue, value);
					}
					if (storeAgain)
					{
						StoreItem(value, string.Empty);
					}
				}
				if (oldId != newId)
				{
					DeleteItem(oldId, true);
					ItemIds[index] = newId;
				}
				if (StoreItemOnPropertyChanged)
				{
					_readCache.Enqueue(newId);
					RegisterItemPropertyChangedHandler(value, true);
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// See <see cref="ICollection<T>.Add"/>.
		/// </summary>
		/// <remarks>Calls <see cref="Insert"/>.</remarks>
		public void Add(T item)
		{
			VerifyNotDisposed();
			Insert(ItemIds.Count, item);
		}

		/// <summary>
		/// See <see cref="ICollection<T>.Clear"/>.
		/// </summary>
		/// <remarks>Items are also deleted from <see cref="Database"/>.</remarks>
		public void Clear()
		{
			VerifyNotDisposed();
			for (int i = 0; i < ItemIds.Count; i++)
			{
				DeleteItem(ItemIds[i], false);
			}
			ItemIds.Clear();
		}

		/// <summary>
		/// See <see cref="ICollection<T>.Contains"/>.
		/// </summary>
		/// <remarks>Calls <see cref="IndexOf"/>.</remarks>
		public bool Contains(T item)
		{
			VerifyNotDisposed();
			return IndexOf(item) >= 0;
		}

		/// <summary>
		/// See <see cref="ICollection<T>.CopyTo"/>.
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex)
		{
			VerifyNotDisposed();
			if (Count != 0)
			{
				CopyTo(0, array, arrayIndex, Count);
			}
		}

		/// <summary>
		/// See <see cref="ICollection<T>.Count"/>.
		/// </summary>
		public int Count
		{
			get
			{
				VerifyNotDisposed();
				return ItemIds.Count;
			}
		}

		/// <summary>
		/// See <see cref="ICollection<T>.IsReadOnly"/>.
		/// </summary>
		/// <value>false.</value>
		public bool IsReadOnly
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		/// <summary>
		/// See <see cref="ICollection<T>.Remove"/>.
		/// </summary>
		/// <remarks>Calls <see cref="RemoveAt"/>.</remarks>
		public bool Remove(T item)
		{
			VerifyNotDisposed();
			int index = IndexOf(item);
			if (index >= 0)
			{
				RemoveAt(index);
				return true;
			}
			return false;
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Typed enumerator.
		/// See <see cref="IEnumerable<T>"/>.
		/// </summary>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			int count = ItemIds.Count;
			for (int i = 0; i < count; i++)
			{
				yield return this[i];
			}
		}

		#endregion

		#region IEnumerable Members

		#region Enumerator

		public struct Enumerator : IEnumerator<T>
		{
			Db4oList<T> _collection;
			int _index;
			bool _isDisposed;

			public Enumerator(Db4oList<T> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException();
				}
				_collection = collection;
				_index = -1;
				_isDisposed = false;
			}

			private void CheckValidIndex(int validMinimum)
			{
				if (_index < validMinimum || _index >= _collection.Count)
				{
					throw new InvalidOperationException();
				}
			}

			private void CheckCollectionUnchanged()
			{
				if (!_collection._isEnumerating)
				{
					throw new InvalidOperationException();
				}
			}

			private void VerifyNotDisposed()
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("Db4oList.Enumerator");
				}
			}

			#region IEnumerator<T> Members

			public T Current
			{
				get
				{
					VerifyNotDisposed();
					CheckValidIndex(0);
					CheckCollectionUnchanged();
					return ((IList<T>) _collection)[_index];
				}
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				_collection = null;
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get
				{
					VerifyNotDisposed();
					return Current;
				}
			}

			public bool MoveNext()
			{
				VerifyNotDisposed();
				CheckValidIndex(-1);
				CheckCollectionUnchanged();
				return (++_index < _collection.Count);
			}

			public void Reset()
			{
				VerifyNotDisposed();
				CheckCollectionUnchanged();
				_index = -1;
			}

			#endregion
		}

		private bool _isEnumerating;

		#endregion

		public Enumerator GetEnumerator()
		{
			VerifyNotDisposed();
			_isEnumerating = true;
			return new Enumerator(this);
		}

		/// <summary>
		/// Enumerator.
		/// See <see cref="IEnumerable"/>.
		/// </summary>
		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Gets object identifiers from <see cref="Database"/>.
		/// </summary>
		/// <param name="items">Item collection.</param>
		/// <returns>
		/// Ids or null, if <paramref name="items"/> is not collection from <see cref="Database"/> query (IObjectSet or native query list).
		/// </returns>
		protected virtual IEnumerable<long> GetIds(IEnumerable items)
		{
			IEnumerable<long> ids = null;
			// results from db4o SODA/QBE query
			if (items is IObjectSet)
			{
				IObjectSet objectSet = items as IObjectSet;
				ids = objectSet.Ext().GetIDs();
			}
					// results from db4o native query
			else if (items is GenericObjectSetFacade<T>)
			{
				GenericObjectSetFacade<T> genericObjectSetFacade = items as GenericObjectSetFacade<T>;
				ids = genericObjectSetFacade._delegate.GetIDs();
			}
			return ids;
		}

		/// <summary>
		/// Gets db4o id of item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Id &gt;= 0, if <paramref name="item"/> exists in <see cref="Database"/>, otherwise -1.</returns>
		protected long GetItemId(T item)
		{
			//ValidateItem(item);
			return Database.GetID(item);
		}

		/// <summary>
		/// Gets item specified by its id from <see cref="Database"/> and optionally activates it.
		/// </summary>
		/// <param name="id">Item id.</param>
		/// <param name="activate">true, to activate item by calling <see cref="ActivateItem"/>.</param>
		/// <returns>Item.</returns>
		/// <exception cref="InvalidOperationException">
		/// <paramref name="activate"/> is true and object with specified <paramref name="id"/> does not exist in <see cref="Database"/>.
		/// </exception>
		protected T GetItem(long id, bool activate)
		{
			T item;
			if (activate)
			{
				item = ActivateItem(id);
				if (item == null)
				{
					throw new InvalidOperationException("Object with Id = " + id + " does not exists in database " +
														Database + ".");
				}
			}
			else
			{
				item = (T) Database.GetByID(id);
			}
			return item;
		}

		/// <summary>
		/// Validates item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="item"/> does not satisfy <see cref="Filter"/> condition.</exception>
		protected virtual void ValidateItem(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!Filter(item))
			{
				throw new ArgumentOutOfRangeException("item", item, "Does not satisfy Filter condition.");
			}
		}

		/// <summary>
		/// Activates item.
		/// </summary>
		/// <remarks>
		/// Calls <see cref="DoActivateItem"/> if item is not activated.
		/// Calls <see cref="DeactivateExcessItems"/> if read cache is enabled.
		/// </remarks>
		/// <param name="id">Id of item.</param>
		/// <returns>Activated item or null if <paramref name="id"/> is not id of stored object.</returns>
		protected T ActivateItem(long id)
		{
			T item = (T) Database.GetByID(id);
			if (item != null)
			{
				if (!Database.IsActive(item))
				{
					if (ReadCacheSize > 0)
					{
						DeactivateExcessItems(1);
						_readCache.Enqueue(id);
					}
					DoActivateItem(item);
				}
				if (StoreItemOnPropertyChanged)
				{
					RegisterItemPropertyChangedHandler(item, true);
				}
			}
			return item;
		}

		/// <summary>
		/// Activates item.
		/// </summary>
		/// <remarks>
		/// If item is not activated by calling <see cref="OnActivating"/>, it is activated using <see cref="IExtObjectContainer.Activate"/> with <see cref="ActivationDepth"/>.
		/// </remarks>
		/// <param name="item">Item.</param>
		protected virtual void DoActivateItem(T item)
		{
			if (!OnActivating(item))
			{
				Database.Activate(item, ActivationDepth);
			}
		}

		/// <summary>
		/// Deactivates unnecessary items in read cache.
		/// </summary>
		/// <remarks>Calls <see cref="DoDeactivateItem"/> to deactivate an item.</remarks>
		/// <param name="extraFreeSize">Extra number of items to make free in read cache.</param>
		protected void DeactivateExcessItems(int extraFreeSize)
		{
			if (ReadCacheSize == 0)
			{
				return;
			}
			int deactivationCount = _readCache.Count - ReadCacheSize +
									Math.Min(ReadCacheSize, extraFreeSize);
			long id;
			T item;
			for (int i = 0; i < deactivationCount; i++)
			{
				id = _readCache.Dequeue();
				item = GetItem(id, false);
				if (item != null)
				{
					if (StoreItemOnPropertyChanged)
					{
						RegisterItemPropertyChangedHandler(item, false);
					}
					if (Database.IsActive(item))
					{
						DoDeactivateItem(item);
					}
				}
			}
		}

		/// <summary>
		/// Deactivates item.
		/// </summary>
		/// <remarks>
		/// If item is not deactivated by calling <see cref="OnDeactivating"/>, it is deactivated using <see cref="IExtObjectContainer.Deactivate"/> with <see cref="ActivationDepth"/>.
		/// </remarks>
		/// <param name="item">Item.</param>
		protected virtual void DoDeactivateItem(T item)
		{
			if (!OnDeactivating(item))
			{
				Database.Deactivate(item, ActivationDepth);
			}
		}

		/// <summary>
		/// Stores (adds or updates) item in <see cref="Database"/>.
		/// </summary>
		/// <remarks>Calls <see cref="DoStoreItem"/> and <see cref="CommitFullCache"/>.</remarks>
		/// <param name="item">Item.</param>
		protected void StoreItem(T item, string propertyName)
		{
			DoStoreItem(item, propertyName);
			CommitFullCache();
		}

		/// <summary>
		/// Stores (adds or updates) item in <see cref="Database"/>.
		/// </summary>
		/// <remarks>
		/// If item is not stored by calling <see cref="OnStoring"/>, it is stored by calling <see cref="Db4objects.Db4o.Ext.IExtObjectContainer.Set(object, int)"/>.
		/// </remarks>
		/// <param name="item">Item.</param>
		protected virtual void DoStoreItem(T item, string propertyName)
		{
			if (!OnStoring(item, propertyName))
			{
				SetDatabaseLastModified();
				Database.Set(item, SetActivationDepth);
			}
		}

		/// <summary>
		/// Deletes item from <see cref="Database"/>.
		/// </summary>
		/// <remarks>Calls <see cref="DoDeleteItem"/> and <see cref="CommitFullCache"/>.</remarks>
		/// <param name="item">Item (which is not activated).</param>
		protected void DeleteItem(T item)
		{
			long id = GetItemId(item);
			DoDeleteItem(item);
			CommitFullCache();
			_deleteCache.Enqueue(id);
		}

		/// <summary>
		/// Deletes item from <see cref="Database"/>.
		/// </summary>
		/// <remarks>
		/// If item is not deleted by calling <see cref="OnDeleting"/>, it is deleted by calling <see cref="IExtObjectContainer.Delete"/>.
		/// </remarks>
		/// <param name="item">Item (which is not activated).</param>
		protected virtual void DoDeleteItem(T item)
		{
			if (!OnDeleting(item))
			{
				SetDatabaseLastModified();
				Database.Delete(item);
				RegisterItemPropertyChangedHandler(item, false);
			}
		}

		/// <summary>
		/// Deletes item specified by its id from <see cref="Database"/>.
		/// </summary>
		/// <remarks>Calls <see cref="Db4oList<T>.DeleteItem(T)"/>.</remarks>
		/// <param name="id">Item id.</param>
		/// <param name="onlyIfLast">Whether to delete item only if no same item (object specified by id) is in the list.</param>
		protected void DeleteItem(long id, bool onlyIfLast)
		{
			if (id <= 0)
			{
				return;
			}
			if (onlyIfLast && ItemIds.Contains(id))
			{
				return;
			}
			T item = GetItem(id, false);
			if (item != null)
			{
				DeleteItem(item);
			}
		}

		/// <summary>
		/// Increases current write cache size by 1 and calls <see cref="Commit"/> if the cache is full.
		/// </summary>
		/// <remarks>Must be called after any database change to commit changes if write cache is full.</remarks>
		protected void CommitFullCache()
		{
			if (_writeCacheCurrentSize < int.MaxValue)
			{
				_writeCacheCurrentSize++;
			}
			if (WriteCacheSize > 0 && _writeCacheCurrentSize >= WriteCacheSize)
			{
				Commit();
			}
		}

		/// <summary>
		/// Checks, whether <paramref name="changes"/> contains <paramref name="change"/>.
		/// </summary>
		/// <param name="changes">Changes to check in.</param>
		/// <param name="change">Change to check for.</param>
		/// <returns>true, if <paramref name="changes"/> contains <paramref name="change"/>.</returns>
		protected static bool IsChange(Db4oListChanges changes, Db4oListChanges change)
		{
			return (changes & change) > 0;
		}

		/// <summary>
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/> event handler used to store changed items to database.
		/// Calls <see cref="StoreItem"/> for changed item.
		/// </summary>
		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			StoreItem((T) sender, e.PropertyName);
		}

		/// <summary>
		/// See <see cref="ItemIds"/>.
		/// </summary>
		private List<long> _itemIds = new List<long>();

		/// <summary>
		/// See <see cref="Database"/>.
		/// </summary>
		private IExtObjectContainer _database;

		/// <summary>
		/// See <see cref="ReadCacheSize"/>.
		/// </summary>
		private int _readCacheSize;

		/// <summary>
		/// Read cache.
		/// Hold activated items` ids.
		/// See <see cref="ReadCacheSize"/>.
		/// </summary>
		private Queue<long> _readCache = new Queue<long>();

		/// <summary>
		/// Delete cache.
		/// Hold deleted items` ids which are added back to this list on <see cref="Rollback"/>.
		/// Cache is cleared on <see cref="Commit"/>.
		/// </summary>
		private Queue<long> _deleteCache = new Queue<long>();

		/// <summary>
		/// See <see cref="WriteCacheSize"/>.
		/// </summary>
		private int _writeCacheSize = 100;

		/// <summary>
		/// Current size of write cache.
		/// See <see cref="WriteCacheSize"/>.
		/// </summary>
		private int _writeCacheCurrentSize;

		/// <summary>
		/// See <see cref="ActivationDepth"/>.
		/// </summary>
		private int _activationDepth = 5;

		/// <summary>
		/// See <see cref="SetActivationDepth"/>.
		/// </summary>
		private int _setActivationDepth = 5;

		/// <summary>
		/// See <see cref="PeekPersistedActivationDepth"/>.
		/// </summary>
		private int _peekPersistedActivationDepth = 5;

		/// <summary>
		/// See <see cref="Filter"/>.
		/// </summary>
		private Predicate<T> _filter = ComparisonHelper<T>.DefaultPredicate;

		/// <summary>
		/// Initial filter used when removing filter.
		/// See <see cref="Filter"/> and <see cref="RemoveFilter"/>.
		/// </summary>
		private Predicate<T> _initFilter;

		/// <summary>
		/// Used in <see cref="RemoveFilter"/>.
		/// See <see cref="Filter"/>.
		/// </summary>
		private bool _filterCalled;

		/// <summary>
		/// See <see cref="DeleteItemsWhileFiltering"/>.
		/// </summary>
		private bool _deleteItemsWhileFiltering;

		/// <summary>
		/// See <see cref="FilteringInDatabase"/>.
		/// </summary>
		private bool _filteringInDatabase = true;

		/// <summary>
		/// See <see cref="Equaler"/>.
		/// </summary>
		private EqualityComparison<T> _equaler = ComparisonHelper<T>.DefaultEqualityComparison;

		/// <summary>
		/// See <see cref="Sorter"/>.
		/// </summary>
		private Comparison<T> _sorter;

		/// <summary>
		/// Initial sorter used when removing sorting.
		/// See <see cref="Sorter"/> and <see cref="RemoveSort"/>.
		/// </summary>
		private Comparison<T> _initSorter;

		/// <summary>
		/// Used in <see cref="RemoveSort"/>.
		/// See <see cref="Sorter"/>.
		/// </summary>
		private bool _sortCalled;

		/// <summary>
		/// See <see cref="SortingInDatabase"/>.
		/// </summary>
		private bool _sortingInDatabase = true;

		/// <summary>
		/// See <see cref="StoreItemOnPropertyChanged"/>.
		/// </summary>
		private bool _storeItemOnPropertyChanged = true;

		#region IDisposable Members

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_database != null && _database.IsClosed())
					{
						throw new InvalidOperationException(
								"Database should not be disposed until after Db4oList is disposed.");
					}

					int count = _readCache.Count;
					long id;
					T item;
					for (int i = 0; i < count; ++i)
					{
						id = _readCache.Dequeue();
						item = GetItem(id, false);
						if (item != null)
						{
							if (StoreItemOnPropertyChanged)
							{
								RegisterItemPropertyChangedHandler(item, false);
							}
						}

					}
				}
				_disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Db4oList");
			}
		}

#if DEBUG
		~Db4oList()
		{
			if (!_disposed)
			{
				throw new InvalidOperationException("Disposed not explicitly called");
			}
		}
#endif

		#endregion
	}

	/// <summary>
	/// <see cref="Db4oList`1"/> event arguments.
	/// </summary>
	/// <typeparam name="T">List item type.</typeparam>
	public class Db4oListEventArgs<T> : CancelEventArgs
	{
		/// <summary>
		/// Constructor with <see cref="CancelEventArgs.Cancel"/> false and <see cref="Item"/> null.
		/// </summary>
		public Db4oListEventArgs() {}

		/// <summary>
		/// Constructor with <see cref="CancelEventArgs.Cancel"/> false and <see cref="Item"/> set to <paramref name="item"/>.
		/// </summary>
		/// <param name="item">Item.</param>
		public Db4oListEventArgs(T item)
		{
			_item = item;
		}

		public Db4oListEventArgs(T item, string propertyName) : this(item)
		{
			_propertyName = propertyName;
		}

		/// <summary>
		/// Item which the event is about.
		/// </summary>
		public T Item
		{
			get { return _item; }
		}

		/// <summary>
		/// See <see cref="Item"/>.
		/// </summary>
		private T _item;

		private string _propertyName;

		public string PropertyName
		{
			get { return _propertyName; }
		}
	}

	/// <summary>
	/// <see cref="Db4oList`1"/> list changes.
	/// </summary>
	[Flags]
	public enum Db4oListChanges
	{
		/// <summary>
		/// Filtering.
		/// </summary>
		Filter = 1,
		/// <summary>
		/// Sorting.
		/// </summary>
		Sort = 2
	}
}
using System.ComponentModel;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public abstract class IRecordListBaseTest<T> where T : class, new()
	{
		protected IRecordList<T> _recordList;
		protected string _changedFieldName;

		private bool _listChanged;
		private bool _listContentsChanged;
		private ListChangedEventArgs _listChangedEventArgs;

		protected void ResetListChanged()
		{
			_listChanged = false;
			_listChangedEventArgs = null;
		}

		public void _bindingList_ListChanged(object sender, ListChangedEventArgs e)
		{
			_listChanged = true;
			_listChangedEventArgs = e;
		}

		private void _recordList_ContentOfItemInListChanged(object sender, ListChangedEventArgs e)
		{
			_listContentsChanged = true;
			_listChangedEventArgs = e;
		}

		[SetUp]
		public virtual void SetUp()
		{
			_recordList.ListChanged += _bindingList_ListChanged;
			_recordList.ContentOfItemInListChanged += _recordList_ContentOfItemInListChanged;
			_listChanged = false;
			_listContentsChanged = false;
			_listChangedEventArgs = null;
		}

		[Test]
		public virtual void ListChangedOnReplace()
		{
			Assert.IsFalse(_listChanged);
			if (_recordList.SupportsChangeNotification)
			{
				if (_recordList.Count == 0)
				{
					_recordList.AddNew();
					_listChanged = false;
					_listContentsChanged = false;
					_listChangedEventArgs = null;
				}
				_recordList[0] = new T();
				Assert.IsTrue(_listChanged);
				Assert.IsFalse(_listContentsChanged);
				Assert.AreEqual(ListChangedType.ItemChanged, _listChangedEventArgs.ListChangedType);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
			}
		}

		[Test]
		public virtual void ListContentChangedOnChange()
		{
			Assert.IsFalse(_listChanged);
			if (_recordList.SupportsChangeNotification)
			{
				if (_recordList.Count == 0)
				{
					_recordList.AddNew();
					_listChanged = false;
					_listContentsChanged = false;
					_listChangedEventArgs = null;
				}
				Change(_recordList[0]);
				Assert.IsTrue(_listContentsChanged);
				Assert.IsFalse(_listChanged);
				Assert.AreEqual(ListChangedType.ItemChanged, _listChangedEventArgs.ListChangedType);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
			}
		}

		protected abstract void Change(T item);
	}
}
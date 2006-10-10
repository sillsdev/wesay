using NUnit.Framework;
using System.ComponentModel;

namespace WeSay.Data.Tests
{
	public abstract class IRecordListBaseTest<T> where T: class, new()
	{
		protected IRecordList<T> _recordList;
		protected string _changedFieldName;

		private bool _listChanged;
		private ListChangedEventArgs _listChangedEventArgs;

		protected void ResetListChanged()
		{
			this._listChanged = false;
			this._listChangedEventArgs = null;
		}

		public void _bindingList_ListChanged(object sender, ListChangedEventArgs e)
		{
			_listChanged = true;
			_listChangedEventArgs = e;
		}

		[SetUp]
		public virtual void SetUp()
		{
			this._recordList.ListChanged += new ListChangedEventHandler(_bindingList_ListChanged);
			_listChanged = false;
			_listChangedEventArgs = null;
		}

		[Test]
		public virtual void ListChangedOnChange()
		{
			Assert.IsFalse(_listChanged);
			if (_recordList.SupportsChangeNotification)
			{
				if (_recordList.Count == 0)
				{
					_recordList.AddNew();
					_listChanged = false;
					_listChangedEventArgs = null;
				}
				Change(_recordList[0]);
				Assert.IsTrue(_listChanged);
				Assert.AreEqual(ListChangedType.ItemChanged, _listChangedEventArgs.ListChangedType);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
			}
		}
		protected abstract void Change(T item);
	}
}

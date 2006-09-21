using System;
using System.Collections.Generic;
using System.Text;
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
		private PropertyDescriptor _property;


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

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(T));
			this._property = pdc.Find(_changedFieldName, false);
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
				Change((T)_recordList[0]);
				Assert.IsTrue(_listChanged);
				Assert.AreEqual(ListChangedType.ItemChanged, _listChangedEventArgs.ListChangedType);
				Assert.AreEqual(_property, _listChangedEventArgs.PropertyDescriptor);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
				Assert.AreEqual(0, _listChangedEventArgs.OldIndex);
			}
		}
		protected abstract void Change(T item);
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data.Tests
{
	public class ChildTestItem
	{
		private int _storedInt;
		private string _storedString;
		private DateTime _storedDateTime;
		private ChildTestItem _testItem;
		public ChildTestItem() {}

		public ChildTestItem(string s, int i, DateTime d)
		{
			_storedInt = i;
			_storedString = s;
			_storedDateTime = d;
		}

		public ChildTestItem Child
		{
			get { return _testItem; }
			set { _testItem = value; }
		}

		public int Depth
		{
			get
			{
				int depth = 1;
				ChildTestItem item = Child;
				while (item != null)
				{
					++depth;
					item = item.Child;
				}
				return depth;
			}
		}

		public int StoredInt
		{
			get { return _storedInt; }
			set { _storedInt = value; }
		}

		public string StoredString
		{
			get { return _storedString; }
			set { _storedString = value; }
		}

		public DateTime StoredDateTime
		{
			get { return _storedDateTime; }
			set { _storedDateTime = value; }
		}
	}

	public class TestItem: INotifyPropertyChanged
	{
		private int _storedInt;
		private string _storedString;
		private DateTime _storedDateTime;
		private ChildTestItem _childTestItem;
		private int _onActivateDepth;
		private List<string> _storedList;

		private List<ChildTestItem> _childItemList;

		public List<ChildTestItem> ChildItemList
		{
			get { return _childItemList; }
			set
			{
				_childItemList = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Children"));
			}
		}

		public int OnActivateDepth
		{
			get { return _onActivateDepth; }
		}

		public ChildTestItem Child
		{
			get { return _childTestItem; }
			set
			{
				_childTestItem = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Child"));
			}
		}


		public int Depth
		{
			get
			{
				int depth = 1;
				ChildTestItem item = Child;
				while (item != null)
				{
					++depth;
					item = item.Child;
				}
				return depth;
			}
		}

		public TestItem()
		{
			_storedDateTime = PreciseDateTime.UtcNow;
			_childTestItem = new ChildTestItem();
		}

		public TestItem(string s, int i, DateTime d)
		{
			_storedString = s;
			_storedInt = i;
			_storedDateTime = d;
		}

		public override string ToString()
		{
			return StoredInt + ". " + StoredString + " " + StoredDateTime;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			TestItem item = obj as TestItem;
			if (item == null)
			{
				return false;
			}

			return Equals(item);
		}

		public bool Equals(TestItem item)
		{
			if (item == null)
			{
				return false;
			}

			return (_storedInt == item._storedInt) && (_storedString == item._storedString) &&
				   (_storedDateTime == item._storedDateTime);
		}

		public int StoredInt
		{
			get { return _storedInt; }
			set
			{
				if (_storedInt != value)
				{
					_storedInt = value;
					OnPropertyChanged(new PropertyChangedEventArgs("StoredInt"));
				}
			}
		}

		public string StoredString
		{
			get { return _storedString; }
			set
			{
				if (_storedString != value)
				{
					_storedString = value;
					OnPropertyChanged(new PropertyChangedEventArgs("StoredString"));
				}
			}
		}

		public DateTime StoredDateTime
		{
			get { return _storedDateTime; }
			set
			{
				if (_storedDateTime != value)
				{
					_storedDateTime = value;
					OnPropertyChanged(new PropertyChangedEventArgs("StoredDateTime"));
				}
			}
		}

		public List<string> StoredList
		{
			get { return _storedList; }
			set
			{
				_storedList = value;
				OnPropertyChanged(new PropertyChangedEventArgs("StoredList"));
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		#endregion
	}
}
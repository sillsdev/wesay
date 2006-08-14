using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WeSay.Data.Tests
{
	public class TestItem : INotifyPropertyChanged
	{
		int _storedInt;
		string _storedString;
		DateTime _storedDateTime;

		public TestItem()
		{
		}

		public TestItem(string s, int i, DateTime d)
		{
			_storedString = s;
			_storedInt = i;
			_storedDateTime = d;
		}

		public override string ToString()
		{
			return StoredInt.ToString() + ". " + StoredString + " " + StoredDateTime.ToString();
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

			return (_storedInt == item._storedInt) &&
				(_storedString == item._storedString) &&
				(_storedDateTime == item._storedDateTime);
		}

		public override int GetHashCode()
		{
			return _storedInt ^ _storedString.GetHashCode() ^ _storedDateTime.GetHashCode();
		}

		public int StoredInt
		{
			get
			{
				return this._storedInt;
			}
			set
			{
				if (this._storedInt != value)
				{
					this._storedInt = value;
					OnPropertyChanged(new PropertyChangedEventArgs("StoredInt"));
				}
			}
		}

		public string StoredString
		{
			get
			{
				return this._storedString;
			}
			set
			{
				if (this._storedString != value)
				{
					this._storedString = value;
					OnPropertyChanged(new PropertyChangedEventArgs("StoredString"));
				}
			}
		}

		public DateTime StoredDateTime
		{
			get
			{
				return this._storedDateTime;
			}
			set
			{
				if (this._storedDateTime != value)
				{
					this._storedDateTime = value;
					OnPropertyChanged(new PropertyChangedEventArgs("StoredDateTime"));
				}
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, e);
			}

		}

		#endregion
	}

}

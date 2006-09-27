using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data.Tests
{

	public class SimpleIntTestClass : INotifyPropertyChanged, IComparable, IComparable<SimpleIntTestClass>
	{
		int _i;
		public SimpleIntTestClass()
		{
		}

		public SimpleIntTestClass(int i)
		{
			this._i = i;
		}

		public int I
		{
			get
			{
				return _i;
			}
			set
			{
				_i = value;
				OnPropertyChanged(new PropertyChangedEventArgs("I"));
			}
		}
		public override string ToString()
		{
			return I.ToString();
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

		#region IComparable Members

		int IComparable.CompareTo(object obj)
		{
			return CompareTo((SimpleIntTestClass)obj);
		}

		#endregion

		#region IComparable<SimpleTestClass> Members

		public int CompareTo(SimpleIntTestClass other)
		{
			if (other == null)
			{
				return 1;
			}
			return (other.I - I);
		}

		#endregion
	}
}

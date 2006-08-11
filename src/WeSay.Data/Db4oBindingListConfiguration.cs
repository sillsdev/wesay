using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public class Db4oBindingListConfiguration<T>
	{
		Db4oDataSource _dataSource;
		Predicate<T> _filter;
		IComparer<T> _sort;

		public Predicate<T> Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				_filter = value;
			}
		}
		public IComparer<T> Sort
		{
			get
			{
				return _sort;
			}
			set
			{
				_sort = value;
			}
		}

		public Db4oDataSource DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
			}
		}

		public Db4oBindingListConfiguration(Db4oDataSource dataSource)
		{
			_dataSource = dataSource;
		}
	}

}

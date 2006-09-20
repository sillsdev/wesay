using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data.Tests
{
	public class SimpleIntFilter : IFilter<SimpleIntTestClass>
	{
		int _lowerBound;
		int _upperBound;
		public SimpleIntFilter(int lowerBound, int upperBound)
		{
			_lowerBound = lowerBound;
			_upperBound = upperBound;
		}
		#region IFilter<SimpleIntTestClass> Members

		public Predicate<SimpleIntTestClass> Inquire
		{
			get
			{
				return delegate(SimpleIntTestClass simpleIntTest)
				{
					return (_lowerBound <= simpleIntTest.I && simpleIntTest.I <= _upperBound);
				};
			}
		}


		public string Key
		{
			get
			{
				return this.ToString() + _lowerBound.ToString() + "-" + _upperBound.ToString();
			}
		}

		#endregion
	}
}

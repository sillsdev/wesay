
using System.Collections;
using System.Collections.Generic;

namespace WeSay.Foundation
{
	public interface IDisplayStringAdaptor
	{
		string GetDisplayLabel(object item);
	}

	public class ToStringAutoCompleteAdaptor : IDisplayStringAdaptor
	{

		public string GetDisplayLabel(object item)
		{
			return item.ToString();
		}
	}

	/// <summary>
	/// This is used to convert from the IEnuerable<string> that the cache give us
	/// to the IEnumerable<object> that AutoComplete needs.
	/// </summary>
	public class StringToObjectEnumerableWrapper : IEnumerable<object>
	{
		private readonly IEnumerable<string> _stringCollection;

		public StringToObjectEnumerableWrapper(IEnumerable<string> stringCollection)
		{
			_stringCollection = stringCollection;
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			foreach (string s in _stringCollection)
			{
				yield return s;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}

	}


	public interface IChoice
	{
		string Label
		{
			get;
		}
		string Key
		{
			get;
		}
	}
}

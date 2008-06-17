using System.Collections.Generic;

namespace WeSay.Data
{
	public sealed class RecordTokenComparer<T>: IComparer<RecordToken<T>>
	{
		private readonly IComparer<string> _keySorter;

		public RecordTokenComparer(IComparer<string> keySorter)
		{
			_keySorter = keySorter;
		}

		#region IComparer<RecordToken> Members

		public int Compare(RecordToken<T> x, RecordToken<T> y)
		{
			int result = _keySorter.Compare(x.DisplayString, y.DisplayString);
			if (result == 0)
			{
				result = x.Id.CompareTo(y.Id);
			}
			return result;
		}

		#endregion
	} ;
}
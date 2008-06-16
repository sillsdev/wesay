using System.Collections.Generic;

namespace WeSay.Data
{
	public class RecordTokenComparer<T> : IComparer<RecordToken<T>>
	{
		private readonly IComparer<string> _keySorter;
		private bool _ignoreId;

		public RecordTokenComparer(IComparer<string> keySorter)
		{
			_ignoreId = false;
			_keySorter = keySorter;
		}

		public bool IgnoreId
		{
			get { return this._ignoreId; }
			set { this._ignoreId = value; }
		}

		#region IComparer<RecordToken> Members

		public int Compare(RecordToken<T> x, RecordToken<T> y)
		{
			int result = _keySorter.Compare(x.DisplayString, y.DisplayString);
			if (result == 0 && !IgnoreId)
			{
				result = x.Id.CompareTo(y.Id);
			}
			return result;
		}

		#endregion
	} ;

}

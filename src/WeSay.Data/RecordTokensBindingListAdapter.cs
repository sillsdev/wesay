using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	class RecordTokensBindingListAdapter<T> : BindingList<RecordToken<T>>
	{
		public RecordTokensBindingListAdapter()
		{}

		public RecordTokensBindingListAdapter(IList<RecordToken<T>> list):base(list)
		{
		}

		protected override bool SupportsSortingCore
		{
			get
			{
				return false;
			}
		}
		protected override bool SupportsSearchingCore
		{
			get
			{
				return false;
			}
		}
	}
}

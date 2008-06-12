using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WeSay.Data
{
	class RecordTokensBindingListAdapter<T> : BindingList<RecordToken>
	{
		public RecordTokensBindingListAdapter()
		{}

		public RecordTokensBindingListAdapter(IList<RecordToken> list):base(list)
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

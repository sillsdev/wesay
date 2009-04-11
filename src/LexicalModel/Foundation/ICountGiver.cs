using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Foundation
{
	public interface ICountGiver
	{
		int Count
		{ get;
		}
	}

	public class NullCountGiver : ICountGiver
	{
		public int Count
		{
			get { return 0; }
		}
	}
}

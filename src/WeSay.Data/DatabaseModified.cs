using System;
using System.Diagnostics;

namespace WeSay.Data
{
	public sealed class DatabaseModified
	{
		private DateTime _lastModified;

		public DateTime LastModified
		{
			get
			{
				//workaround db4o 6 bug
				if (_lastModified.Kind != DateTimeKind.Utc)
				{
					LastModified = new DateTime(_lastModified.Ticks, DateTimeKind.Utc);
				}
				return _lastModified;
			}
			set
			{
				Debug.Assert(value.Kind == DateTimeKind.Utc);
				_lastModified = value;
			}
		}
	}
}
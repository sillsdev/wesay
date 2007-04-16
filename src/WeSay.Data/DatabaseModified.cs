using System;

namespace WeSay.Data
{
	public class DatabaseModified
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
				return this._lastModified;
			}
			set
			{
				System.Diagnostics.Debug.Assert(value.Kind == DateTimeKind.Utc);
				this._lastModified = value;
			}
		}
	}
}

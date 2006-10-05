using System;

namespace WeSay.Data
{
	public class DatabaseModified
	{
		private DateTime _lastModified;

		public DateTime LastModified
		{
			get { return this._lastModified; }
			set { this._lastModified = value; }
		}
	}
}

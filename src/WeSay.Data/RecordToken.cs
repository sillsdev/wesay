namespace WeSay.Data
{
	public class RecordToken
	{
		public RecordToken() {}
		public RecordToken(string s, long id)
		{
			_displayString = s;
			_id = id;
		}
		private string _displayString;
		private long _id;

		public string DisplayString
		{
			get { return this._displayString; }
			set { this._displayString = value; }
		}

		public long Id
		{
			get { return this._id; }
			set { this._id = value; }
		}
		public override string ToString()
		{
			return DisplayString;
		}
	}
}

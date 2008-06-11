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
		private readonly string _displayString;
		private readonly long _id;

		public string DisplayString
		{
			get { return this._displayString; }
		}

		public long Id
		{
			get { return this._id; }
		}
		public override string ToString()
		{
			return DisplayString;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			RecordToken other = obj as RecordToken;
			if (other == null)
			{
				return false;
			}
			return (Id.Equals(other.Id)
				 && DisplayString.Equals(other.DisplayString));
		}

		public static bool operator == (RecordToken rt1, RecordToken rt2)
		{
			if (ReferenceEquals(rt1, rt2))
			{
				return true;
			}

			if ((object)rt1 == null || (object)rt2 == null)
			{
				return false;
			}

			return (rt1.Id == rt2.Id && rt1.DisplayString == rt2.DisplayString);
		}

		public static bool operator !=(RecordToken rt1, RecordToken rt2)
		{
			return !(rt1 == rt2);
		}
		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ DisplayString.GetHashCode();
		}
	}
}

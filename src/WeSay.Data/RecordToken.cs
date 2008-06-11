using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	public class RecordToken: IEquatable<RecordToken>
	{
		public RecordToken() {}
		public RecordToken(string s, RepositoryId id)
		{
			_displayString = s;
			_id = id;
		}
		private readonly string _displayString;
		private readonly RepositoryId _id;

		public string DisplayString
		{
			get { return this._displayString; }
		}

		public RepositoryId Id
		{
			get { return this._id; }
		}
		public override string ToString()
		{
			return DisplayString;
		}

		public static int FindFirstWithDisplayString(List<RecordToken> recordTokens, string displayString)
		{
			return recordTokens.FindIndex(delegate (RecordToken r)
							  { return r.DisplayString == displayString; });
			// todo This could be a binary search. We need our own search so that
			// we find the first element. The .net BinarySearch finds any, not the first.
		}

		public static bool operator !=(RecordToken recordToken1, RecordToken recordToken2)
		{
			return !Equals(recordToken1, recordToken2);
		}

		public static bool operator ==(RecordToken recordToken1, RecordToken recordToken2)
		{
			return Equals(recordToken1, recordToken2);
		}

		public bool Equals(RecordToken recordToken)
		{
			if (recordToken == null)
			{
				return false;
			}
			return Equals(_displayString, recordToken._displayString) && Equals(_id, recordToken._id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as RecordToken);
		}

		public override int GetHashCode()
		{
			return _displayString.GetHashCode() + 29 * _id.GetHashCode();
		}
	}
}

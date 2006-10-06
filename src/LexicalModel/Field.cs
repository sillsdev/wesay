using System;

namespace WeSay.LexicalModel
{
	public class Field
	{
		private string _fieldName;
		private string[] _writingSystems;
		public string FieldName
		{
			get
			{
				return _fieldName;
			}
		}

		public string[] WritingSystems
		{
			get
			{
				return _writingSystems;
			}
		}

		public Field(string FieldName, params string[] writingSystems)
		{
			if (FieldName == null)
			{
				throw new ArgumentNullException("FieldName");
			}
			if(writingSystems == null)
			{
				throw new ArgumentNullException("writingSystems");
			}
			int i = 0;
			foreach (string s in writingSystems)
			{
				i++;
				if(s==null)
				{
					throw new ArgumentNullException("writingSystem",
													"Writing System argument" + i.ToString() + "is null");
				}
			}
			_fieldName = FieldName;
			_writingSystems = writingSystems;
		}

		public bool HasWritingSystem(string writingSystemId)
		{
			return Array.Exists<string>(_writingSystems,
										delegate(string s)
										{
											return s == writingSystemId;
										});
		}
	}
}

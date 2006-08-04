using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexicalEntry
	{
		private MultiText _lexicalForm;
		private Guid _guid;

		public LexicalEntry()
		{
			_lexicalForm = new MultiText();
		}

		public MultiText LexicalForm
		{
			get
			{
				return _lexicalForm;
				//return StringOrEmpty(_lexicalForm);
			}
			//set
			//{
			//    //_lexicalForm = StringOrNull(value);
			//}
		}

		/// <summary>
		/// Used to track this entry across programs, for the purpose of merging and such.
		/// </summary>
		public Guid Guid
		{
			get { return _guid; }
			set { _guid = value; }
		}
	}
}

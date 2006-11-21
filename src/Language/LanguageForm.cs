using System;

namespace WeSay.Language
{
	/// <summary>
	/// A LanguageForm is a unicode string plus the id of its writing system
	/// </summary>
	public class LanguageForm
	{
		private string _writingSystemId;
		private string _form;

		/// <summary>
		/// See the comment on MultiText._parent for information on
		/// this field.
		/// </summary>
		private MultiText _parent;

		public LanguageForm(string writingSystemId, string form, MultiText parent)
		{
			if (parent == null)
			{
				throw new ArgumentException("Cannot be null", "parent");
			}
			_parent = parent;
			_writingSystemId = writingSystemId;
			_form =  form;
		}

		public string WritingSystemId
		{
			get { return _writingSystemId; }
		}

		public string Form
		{
			get { return _form; }
			set { _form = value; }
		}

		/// <summary>
		/// See the comment on MultiText._parent for information on
		/// this field.
		/// </summary>
		public MultiText Parent
		{
			get { return _parent; }
		}
	}
}
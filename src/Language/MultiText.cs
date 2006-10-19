using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace WeSay.Language
{
	public class LanguageForm
	{
		private string _writingSystemId;
		private string _form;

		public LanguageForm(string writingSystemId, string form)
		{
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
	}

	/// <summary>
	/// MultiText holds an array of strings, indexed by writing system ID.
	/// These are simple, single language Unicode strings.
	/// </summary>
	public /*sealed*/ class MultiText : INotifyPropertyChanged, IEnumerable
	{
		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private LanguageForm[] _forms;

		public MultiText()
		{
			_forms = new LanguageForm[0] ;
		}

		public string this[string writingSystemId]
		{
			get
			{
				return GetAlternative(writingSystemId);
			}
			set
			{
				SetAlternative(writingSystemId, value);
			}
		}
		public LanguageForm Find(string writingSystemId)
		{
			foreach(LanguageForm f in _forms)
			{
				if(f.WritingSystemId == writingSystemId)
					return f;
			}
			return null;
		}

		public string GetAlternative(string writingSystemId)
		{
			LanguageForm alt = Find(writingSystemId);
			if (null == alt)
				return string.Empty;

			string form = alt.Form;
			if (form == null)
			{
				return string.Empty;
			}
			else
				return form;
		}

		//hack
		public string GetFirstAlternative()
		{
			if (Count > 0)
				return _forms[0].Form;
			else
				return string.Empty;
		}

		public bool Empty
		{
			get
			{
				return Count == 0;
			}
		}

		public int Count
		{
			get
			{
				return _forms.Length;
			}
		}

		public void SetAlternative(string writingSystemId, string form)
		{
		   Debug.Assert(writingSystemId != null && writingSystemId.Length > 0, "The writing system id was empty.");
		   Debug.Assert(writingSystemId.Trim() == writingSystemId, "The writing system id had leading or trailing whitespace");

		   //enhance: check to see if there has actually been a change

		   LanguageForm alt = Find(writingSystemId);
		   if (form == null || form.Length == 0) // we don't use space to store empty strings.
		   {
			   if (alt != null)
			   {
				   RemoveLanguageForm(alt);
			   }
		   }
		   else
		   {
			   if (alt != null)
			   {
				   alt.Form = form;
			   }
			   else
			   {
				   AddLanguageForm(new LanguageForm(writingSystemId, form));
			   }

		   }

		   NotifyPropertyChanged(writingSystemId);
		}

		private void RemoveLanguageForm(LanguageForm languageForm)
		{
			Debug.Assert(_forms.Length > 0);
			LanguageForm[] forms = new LanguageForm[_forms.Length - 1];
			for (int i = 0,j=0; i < forms.Length; i++,j++)
			{
				if (_forms[j] == languageForm)
				{
					j++;
				}
				forms[i] = _forms[j];
			}
			_forms = forms;
		}

		private void AddLanguageForm(LanguageForm languageForm)
		{
			LanguageForm[] forms = new LanguageForm[_forms.Length + 1];
			for (int i = 0; i < _forms.Length; i++)
			{
				forms[i] = _forms[i];
			}
			forms[_forms.Length] = languageForm;
			_forms = forms;
		}

		private void NotifyPropertyChanged(string writingSystemId)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(writingSystemId));
			}

		}

		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return _forms.GetEnumerator();
		}
		#endregion
	  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
		  //  set { _writingSystemId = value; }
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
	public class MultiText : INotifyPropertyChanged
	{
		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		//protected System.Collections.Generic.Dictionary<string, string> _forms;
//        protected System.Collections.Generic.List<LanguageForm> _forms;
		protected System.Collections.ArrayList _forms;

		public MultiText()
		{
		   // _forms = new Dictionary<string, string>();
		 //   _forms = new List<LanguageForm>(1);
			 _forms = new ArrayList(1);
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
				return "";

			string form = alt.Form;
			if (form == null)
			{
				return "";
			}
			else
				return form;
		}

		public void SetAlternative(string writingSystemId, string form)
		{
		   Debug.Assert(writingSystemId != null && writingSystemId.Length > 0, "The writing system id was empty.");
		   Debug.Assert(writingSystemId.Trim() == writingSystemId, "The writing system id had leading or trailing whitespace");

		   //enhance: check to see if there has actually been a change

		   LanguageForm alt = Find(writingSystemId);
		   if (form == null || form == "") // we don't use space to store empty strings.
		   {
			   if (alt != null)
			   {
				   _forms.Remove(alt);
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
				   _forms.Add(new LanguageForm(writingSystemId, form));
			   }

		   }

		   NotifyPropertyChanged(writingSystemId);
		}

		private void NotifyPropertyChanged(string writingSystemId)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(writingSystemId));
			}

		}
	}
}

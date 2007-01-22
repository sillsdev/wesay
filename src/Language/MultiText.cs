using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using WeSay.Foundation;

namespace WeSay.Language
{
	/* ----------- ABOUT SUBCLASSES OF MULTITEXT ------------
	* Subclasses of MultiText are used in many (most? all?)
	* fields in WeSay exist to in order to make a
	* signature for fields that we want to search on quickly.
	* This is in influenced by the version of db4o we are using (5.5)
	* as our persistence mechanism, which cannot do fast queries
	* which span collections.
	* For example, consider the following class definition.
	* If we want to load all lexeme-forms into a
	* datastructure for as-you-type approximate matching, we
	* can just pull up all objects of this type from the database.
		public class LexicalFormMultiText : MultiText
		{
		}
	*/

	/// <summary>
	/// MultiText holds an array of LanguageForms, indexed by writing system ID.
	/// </summary>
	public class MultiText : WeSay.Foundation.IParentable, INotifyPropertyChanged, IEnumerable
	{
		/// <summary>
		/// We have this pesky "backreference" solely to enable fast
		/// searching in our current version of db4o (5.5), which
		/// can find strings fast, but can't be queried for the owner
		/// quickly, if there is an intervening collection.  Since
		/// each string in WeSay is part of a collection of writing
		/// system alternatives, that means we can't quickly get
		/// an answer, for example, to the question Get all
		/// the Entries that contain a senes which matches the gloss "cat".
		///
		/// Using this field, we can do a query asking for all
		/// the LanguageForms matching "cat".
		/// This can all be done in a single, fast query.
		///  In code, we can then follow the
		/// LanguageForm._parent up to the multitext, then this _parent
		/// up to it's owner, etc., on up the hierarchy to get the Entries.
		///
		/// Subclasses should provide a property which set the proper class.
		///
		/// 23 Jan 07, note: starting to switch to using these for notifying parent of changes, too.
		/// </summary>
		protected WeSayDataObject _parent;

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private LanguageForm[] _forms;

		public MultiText() : this(null)
		{
		}


		public MultiText(WeSayDataObject parent)
		{
			_parent = parent; //ok for this to be null
			_forms = new LanguageForm[0];
		}

		static public MultiText Create(Dictionary<string,string> forms)
		{
			MultiText m = new MultiText();
			if (forms != null && forms.Keys != null)
			{
				foreach (string key in forms.Keys)
				{
					LanguageForm f = m.Find(key);
					if (f != null)
					{
						f.Form = forms[key];
					}
					else
					{
						m.SetAlternative(key, forms[key]);
					}
				}
			}
			return m;
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

		/// <summary>
		/// Subclasses should provide a "Parent" property which set the proper class.
		/// </summary>
		public object ParentAsObject
		{
			get { return _parent; }
		}

		#region IParentable Members

		public WeSayDataObject Parent
		{
			set
			{
				_parent = value;
			}
		}

		#endregion

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
				   AddLanguageForm(new LanguageForm(writingSystemId, form, this));
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

			//actually copy the contents, as we must now be the parent
			forms[_forms.Length] = new LanguageForm(languageForm.WritingSystemId, languageForm.Form, this);
			_forms = forms;
		}

		private void NotifyPropertyChanged(string writingSystemId)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(writingSystemId));
			}

			//TODO:
			//        /// 23 Jan 07, note: starting to switch to using these for notifying parent of changes, too.
			//tell our parent
			//this._parent.NotifyPropertyChanged("option");//todo


		}

		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return _forms.GetEnumerator();
		}
		#endregion

		public override string ToString()
		{
			return GetFirstAlternative();
		}

		public void MergeIn(MultiText incoming)
		{
			foreach (LanguageForm  form in incoming)
			{
				LanguageForm f = this.Find(form.WritingSystemId);
				if (f != null)
				{
					f.Form = form.Form;
				}
				else
				{
					this.AddLanguageForm(form); //this actually copies the meat of the form
				}
			}
		}



	}
}

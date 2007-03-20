using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using WeSay.Foundation;

namespace WeSay.Language
{
	/*
	 * Important: This class purposly hides the implementation of MultiText as a collection of LanguageForms.
	 *
	 * Thus it
	 * 1) only refers to "alternatives" in public methods and
	 * 2) has some otherwise ugly methods for getting at the innards (e.g. annotations) of them.
	 */

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
	//NO: we haven't been able to do a reasonalbly compact xml representation except with custom deserializer
	//[ReflectorType("multiText")]
	[XmlInclude(typeof(LanguageForm))]
	public class MultiText : WeSay.Foundation.IParentable, INotifyPropertyChanged//, IEnumerable
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

			public void Add(Object objectFromSerializer)
			{


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

		[XmlArrayItem(typeof(LanguageForm), ElementName = "tobedetermined")]
		public string this[string writingSystemId]
		{
			get
			{
				return GetExactAlternative(writingSystemId);
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

		/// <summary>
		/// Throws exception if alternative does not exist.
		/// </summary>
		/// <param name="writingSystemId"></param>
		/// <returns></returns>
//        public string GetExactAlternative(string writingSystemId)
//        {
//            if (!Contains(writingSystemId))
//            {
//                throw new ArgumentOutOfRangeException("Use Contains() to first check if the multitext has a language form for this writing system.");
//            }
//
//            return GetBestAlternative(writingSystemId, false, null);
//        }

		public bool ContainsAlternative(string writingSystemId)
		{
			return (Find(writingSystemId) != null);
		}

		public bool GetAnnotationOfAlternativeIsStarred(string id)
		{
			LanguageForm alt = Find(id);
			if (alt == null)
			{
				return false;
			}
			return alt.IsStarred;
		}

		public void SetAnnotationOfAlternativeIsStarred(string id, bool isStarred)
		{
			LanguageForm alt = Find(id);
			if (isStarred)
			{
				if (alt == null)
				{
					AddLanguageForm(new LanguageForm(id, String.Empty, this));
					alt = Find(id);
					Debug.Assert(alt != null);
				}
				alt.IsStarred = true;
			}
			else
			{
				if (alt != null)
				{
					if (alt.Form == String.Empty)   //non-starred and empty? Nuke it.
					{
						RemoveLanguageForm(alt);
					}
					else
					{
						alt.IsStarred = false;
					}
				}
				else
				{
					//nothing to do.  Missing altertive == not starred.
				}
			}
			NotifyPropertyChanged(id);
		}

		/// <summary>
		/// Get exact alternative or String.Empty
		/// </summary>
		/// <param name="writingSystemId"></param>
		/// <returns></returns>
		public string GetExactAlternative(string writingSystemId)
		{
			return GetAlternative(writingSystemId, false, null);
		}

		/// <summary>
		/// Gives the string of the requested id if it exists, else the 'first'(?) one that does exist, else Empty String
		/// </summary>
		/// <returns></returns>
		public string GetBestAlternative(string writingSystemId)
		{
			return GetAlternative(writingSystemId, true, string.Empty);
		}

		public string GetBestAlternative(string writingSystemId, string notFirstChoiceSuffix)
		{
			return GetAlternative(writingSystemId, true, notFirstChoiceSuffix);
		}

		/// <summary>
		/// Get a string out
		/// </summary>
		/// <returns>the string of the requested id if it exists,
		/// else the 'first'(?) one that does exist + the suffix,
		/// else the given suffix </returns>
		private string GetAlternative(string writingSystemId, bool doShowSomethingElseIfMissing, string notFirstChoiceSuffix)
		{
			LanguageForm alt = Find(writingSystemId);
			if (null == alt)
			{
				if (doShowSomethingElseIfMissing)
				{
					return GetFirstAlternative() + notFirstChoiceSuffix; ;
				}
				else
				{
					return string.Empty;
				}
			}
			string form = alt.Form;
			if (form == null || (form.Trim().Length == 0))
			{
				if (doShowSomethingElseIfMissing)
				{
					return GetFirstAlternative() + notFirstChoiceSuffix;
				}
				else
				{
					return string.Empty;
				}
			}
			else
			{
				return form;
			}
		}

		public string GetFirstAlternative()
		{
			foreach (LanguageForm form in this.Forms)
			{
				if (form.Form.Trim().Length>0)
				{
					return form.Form;
				}
			}
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

		/// <summary>
		/// just for deserialization
		/// </summary>
		[XmlElement(typeof(LanguageForm), ElementName="form")]
		public LanguageForm[] Forms
		{
			get
			{
				return _forms;
			}
			set
			{
				_forms = value;
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

		public void MergeInWithAppend(MultiText incoming, string separator)
		{
			foreach (LanguageForm form in incoming)
			{
				LanguageForm f = this.Find(form.WritingSystemId);
				if (f != null)
				{
					f.Form += separator + form.Form;
				}
				else
				{
					this.AddLanguageForm(form); //this actually copies the meat of the form
				}
			}
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

	#region NetReflector
	public class MultiTextSerializorFactory : ISerialiserFactory
	{
		public IXmlMemberSerialiser Create(ReflectorMember member, ReflectorPropertyAttribute attribute)
		{
			return new MultiTextSerialiser(member, attribute);
		}
	}

	internal class MultiTextSerialiser : XmlMemberSerialiser
	{
		public MultiTextSerialiser(ReflectorMember member, ReflectorPropertyAttribute attribute)
			: base(member, attribute)
		{
		}

		public override object Read(XmlNode node, NetReflectorTypeTable table)
		{
			MultiText text = new MultiText();
			foreach (XmlNode form in node.SelectNodes("form"))
			{
					text[form.Attributes["ws"].Value] = form.InnerText;
			}
			return text;
		}

	}
	#endregion


}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using com.db4o;
using WeSay.Language;


namespace WeSay.LexicalModel
{
	public class LexEntry : WeSayDataObject
	{
		private MultiText _lexicalForm;
		private Guid _guid;
		private WeSay.Data.InMemoryBindingList<LexSense> _senses;
		private DateTime _creationDate;
		private DateTime _modifiedDate;

		public LexEntry()
		{
			Init(Guid.NewGuid());
		}

		public LexEntry(Guid guid)
		{
			Init(guid);
		}

		private void Init(Guid guid)
		{
			_guid = guid;
			this._lexicalForm = new MultiText();
			this._senses = new WeSay.Data.InMemoryBindingList<LexSense>();
			this._creationDate = DateTime.Now;
			this._modifiedDate = _creationDate;

			WireUpEvents();
		}


		public override string ToString()
		{
			//hack
			if(_lexicalForm !=null)
				return _lexicalForm.GetFirstAlternative();
			else
				return "";
		}


		public string ToRtf()
		{

			string rtf = @"{\rtf1\ansi\fs28 ";
			  if(_lexicalForm !=null){
				  rtf += @"\b ";
				  foreach(LanguageForm l in _lexicalForm){
					  rtf += l.Form + " ";
				  }
				  rtf += @"\b0 ";
			  }

			  foreach(LexSense sense in _senses) {
				  rtf += @"\i ";
				  if (sense.Gloss != null)
				  {
					  foreach (LanguageForm l in sense.Gloss)
					  {
						  rtf += l.Form + " ";
					  }
				  }
				  rtf += @" \i0 ";

				  foreach (LexExampleSentence exampleSentence in sense.ExampleSentences){
					  if(exampleSentence.Sentence != null)
					  {
						  foreach (LanguageForm l in exampleSentence.Sentence)
						  {
							  rtf += l.Form + " ";
						  }
					  }
				  }
			  }

			rtf += @"\par}";
			return Utf16ToRtfAnsi(rtf);
		}

		private string Utf16ToRtfAnsi(string inString)
		{
			string outString= String.Empty;
			foreach (char c in inString){
				if (c > 128)
				{
					outString += @"\u" + Convert.ToInt16(c).ToString() + "?";
				}
				else
				{
					outString += c;
				}
			}
			return outString;
		}

	   protected override void WireUpEvents()
	   {
		   base.WireUpEvents();
		   WireUpChild(_lexicalForm);
		   WireUpList(_senses,"senses");
	   }


		public override void SomethingWasModified()
		{
			_modifiedDate = DateTime.Now;
		}

		public MultiText LexicalForm
		{
			get   {  return _lexicalForm;       }
		}

		public DateTime CreationDate
		{
			get   {  return _creationDate;  }
		}

		public DateTime ModifiedDate
		{
			get { return _modifiedDate; }
		}

		public IBindingList Senses
		{
			get   {  return _senses;       }
		}

		/// <summary>
		/// Used to track this entry across programs, for the purpose of merging and such.
		/// </summary>
		public Guid Guid
		{
			get { return _guid; }
			set
			{
				if (_guid  != value)
				{
					_guid  = value;
					NotifyPropertyChanged("GUID");
				}
			}
		}
	}
}

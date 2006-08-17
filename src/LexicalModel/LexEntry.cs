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
		private BindingList<LexSense> _senses;
		private DateTime _creationDate;
		private DateTime _modifiedDate;

		public LexEntry()
		{
			_lexicalForm = new MultiText();
			_senses = new BindingList<LexSense>();
			_creationDate = DateTime.Now;
			_modifiedDate = _creationDate;

			WireUpEvents();
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

		public BindingList<LexSense> Senses
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

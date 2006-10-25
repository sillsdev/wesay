using System;
using System.ComponentModel;
using WeSay.Language;


namespace WeSay.LexicalModel
{
	sealed public class LexEntry : WeSayDataObject
	{
		private LexicalFormMultiText _lexicalForm;
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
			this._lexicalForm = new LexicalFormMultiText();
			this._senses = new WeSay.Data.InMemoryBindingList<LexSense>();
			this._creationDate = DateTime.Now;
			this._modifiedDate = _creationDate;

			WireUpEvents();
		}


		public override string ToString()
		{
			//hack
			if (_lexicalForm != null)
				return _lexicalForm.GetFirstAlternative();
			else
				return "";
		}

		protected override void WireUpEvents()
		{
			base.WireUpEvents();
			WireUpChild(_lexicalForm);
			WireUpList(_senses, "senses");
		}


		public override void SomethingWasModified(string PropertyModified)
		{
			_modifiedDate = DateTime.Now;
			if (PropertyModified != "senses")
			{
				RemoveEmptySenses();
			}
			base.SomethingWasModified(PropertyModified);
		}

		public MultiText LexicalForm
		{
			get
			{
				return _lexicalForm;
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return _creationDate;
			}
		}

		public DateTime ModifiedDate
		{
			get
			{
				return _modifiedDate;
			}
		}

		public IBindingList Senses
		{
			get
			{
				return _senses;
			}
		}

		/// <summary>
		/// Used to track this entry across programs, for the purpose of merging and such.
		/// </summary>
		public Guid Guid
		{
			get
			{
				return _guid;
			}
			set
			{
				if (_guid != value)
				{
					_guid = value;
					NotifyPropertyChanged("GUID");
				}
			}
		}

		public override bool Empty
		{
			get
			{
				return Senses.Count == 0 && LexicalForm.Empty;
			}
		}

		public void RemoveEmptySenses()
		{
			// remove any senses that are empty
			int count = this._senses.Count;

			for (int i = count - 1; i >= 0; i--)
			{
				if (this._senses[i].Empty)
				{
					this._senses.RemoveAt(i);
				}
			}
			if(count != this._senses.Count)
			{
				OnEmptyObjectsRemoved();
			}
		}
	}
}

using System;
using System.ComponentModel;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel
{


	/// <summary>
	/// A Lexical Entry is what makes up our lexicon/dictionary.  In
	/// some languages/dictionaries, these will be indistinguishable from "words".
	/// In others, words are made up of lexical entries.
	/// </summary>
	sealed public class LexEntry : WeSayDataObject
	{
		private LexicalFormMultiText _lexicalForm;
		private Guid _guid;
		private WeSay.Data.InMemoryBindingList<LexSense> _senses;
		private DateTime _creationTime;
		private DateTime _modificationTime;

		public LexEntry(): base(null)
		{
			Init(Guid.NewGuid());
		}

		public LexEntry(Guid guid): base(null)
		{
			Init(guid);
		}

		private void Init(Guid guid)
		{
			if (guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
			}
			else
			{
				_guid = guid;
			}
			this._lexicalForm = new LexicalFormMultiText(this);
			this._senses = new WeSay.Data.InMemoryBindingList<LexSense>();
			this.CreationTime = DateTime.UtcNow;
			this.ModificationTime = CreationTime;

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
			//workaround db4o 6 bug
			if (_creationTime.Kind != DateTimeKind.Utc)
			{
				this.CreationTime = new DateTime(_creationTime.Ticks, DateTimeKind.Utc);
			}
			if (_modificationTime.Kind != DateTimeKind.Utc)
			{
				this.ModificationTime = new DateTime(_modificationTime.Ticks, DateTimeKind.Utc);
			}

			System.Diagnostics.Debug.Assert(this.CreationTime.Kind == DateTimeKind.Utc);
			System.Diagnostics.Debug.Assert(this.ModificationTime.Kind == DateTimeKind.Utc);
			base.WireUpEvents();
			WireUpChild(_lexicalForm);
			WireUpList(_senses, "senses");
		}


		public override void SomethingWasModified(string PropertyModified)
		{
			ModificationTime = DateTime.UtcNow;
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
#if DEBUG
				//Why have this requirement, when the helper isn't needed in a non-db-test context?
				//ONly because I don't know how else to make sure it is initialized when
				// it *is* needed.
				if (Db4oLexModelHelper.Singleton == null)
				{
					throw new ApplicationException("This class should not be used without initializing Db4oLexModelHelper.");
				}
#endif

				return _lexicalForm;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return _creationTime;
			}
			set
			{
				System.Diagnostics.Debug.Assert(value.Kind == DateTimeKind.Utc);
				_creationTime = value;
			}
		}

		public DateTime ModificationTime
		{
			get
			{
				return _modificationTime;
			}
			set
			{
				System.Diagnostics.Debug.Assert(value.Kind == DateTimeKind.Utc);
				_modificationTime = value;
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

	/// <summary>
	/// See comment in MultiText.cs for an explanation of this class.
	/// </summary>
	public class LexicalFormMultiText : MultiText
	{
		public LexicalFormMultiText(LexEntry parent)
			: base(parent)
		{
		}

		///// <summary>
		///// TODO: needed for GetClosestLexicalForms(IRecordList<LexicalFormMultiText> lexicalForms...)
		///// </summary>
		//public LexicalFormMultiText()
		//    : base(null)
		//{
		//}

		public new LexEntry Parent
		{
			get { return _parent as LexEntry; }
		}
	}
}

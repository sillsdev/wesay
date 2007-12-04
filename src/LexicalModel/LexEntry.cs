using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WeSay.Data;
using WeSay.Foundation;
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
		/// <summary>
		/// This use for keeping track of the item when importing an then exporting again,
		/// like for merging. The user doesn't edit this, and if it is null that's fine,
		/// the exporter will make up a reasonable one.
		/// </summary>
		private string _id;
		private InMemoryBindingList<LexSense> _senses;
		private DateTime _creationTime;
		private DateTime _modificationTime;
		private bool _isBeingDeleted;

		new public class WellKnownProperties : WeSayDataObject.WellKnownProperties
		{
			static public string Citation = "citation";
			static public bool Contains(string fieldName)
			{
				List<string> list = new List<string>(new string[] { Citation });
				return list.Contains(fieldName);
			}
		} ;

		public LexEntry(): base(null)
		{
			Init(null, Guid.NewGuid());
		}

		public LexEntry(string id, Guid guid): base(null)
		{
			Init(id, guid);
		}

		private void Init(string id,Guid guid)
		{
			_id = id;
			if (_id != null)
			{
				_id = _id.Trim().Normalize(NormalizationForm.FormD);
			}
			if (guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
			}
			else
			{
				_guid = guid;
			}
			this._lexicalForm = new LexicalFormMultiText(this);
			this._senses = new InMemoryBindingList<LexSense>();
			CreationTime = DateTime.UtcNow;
			ModificationTime = CreationTime;

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
				CreationTime = new DateTime(_creationTime.Ticks, DateTimeKind.Utc);
			}
			if (_modificationTime.Kind != DateTimeKind.Utc)
			{
				ModificationTime = new DateTime(_modificationTime.Ticks, DateTimeKind.Utc);
			}

			System.Diagnostics.Debug.Assert(CreationTime.Kind == DateTimeKind.Utc);
			System.Diagnostics.Debug.Assert(ModificationTime.Kind == DateTimeKind.Utc);
			base.WireUpEvents();
			WireUpChild(_lexicalForm);
			WireUpList(_senses, "senses");
		}


		public override void SomethingWasModified(string propertyModified)
		{
			base.SomethingWasModified(propertyModified);
			ModificationTime = DateTime.UtcNow;
			//too soon to make id: this method is called after first keystroke
			//  GetOrCreateId(false);
		}

		public string GetOrCreateId(bool doCreateEvenIfNoLexemeForm)
		{
			if (String.IsNullOrEmpty(_id))
			{
				//review: I think we could rapidly search the db for exact id matches,
				//so we could come up with a smaller id... but this has the nice
				//property of being somewhat readable and unique even across merges
				if (!String.IsNullOrEmpty(this._lexicalForm.GetFirstAlternative()))
				{
					_id = this._lexicalForm.GetFirstAlternative().Trim().Normalize(NormalizationForm.FormD)+"_"+this.Guid;
					this.NotifyPropertyChanged("id");
				}
				else if (doCreateEvenIfNoLexemeForm)
				{
					_id = "Id'dPrematurely_"+this.Guid;
					this.NotifyPropertyChanged("id");
				}
			}

			return _id;
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>The signature here is MultiText rather than LexicalFormMultiText because we want
		/// to hide this (hopefully temporary) performance implementation detail. </remarks>
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

		public override bool IsEmpty
		{
			get
			{
				return Senses.Count == 0 &&
					   LexicalForm.Empty &&
					   !HasProperties;
			}
		}

		/// <summary>
		/// This use for keeping track of the item when importing an then exporting again,
		/// like for merging. Also used for relations (e.g. superentry). we purposefully
		/// delay making one of these (if we aren't contructed with one) in order to give
		/// time to get a LexemeForm to make the id more readable.
		/// </summary>
		public string Id
		{
			get
			{
				return GetOrCreateId(false);
			}
		}

		public override void CleanUpAfterEditting()
		{
			Debug.Assert(!IsBeingDeleted);
			base.CleanUpAfterEditting();
			foreach (LexSense sense in _senses)
			{
				sense.CleanUpAfterEditting();
			}
			CleanUpEmptyObjects();
		}

		public override void CleanUpEmptyObjects()
		{
			Debug.Assert(!IsBeingDeleted);
			Palaso.Reporting.Logger.WriteMinorEvent("LexEntry CleanUpEmptyObjects()");
			base.CleanUpEmptyObjects();


			for (int i = 0; i < this._senses.Count; i++)
			{
				_senses[i].CleanUpEmptyObjects();
			}

			// remove any senses that are empty
			int count = this._senses.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				if (this._senses[i].IsEmptyForPurposesOfDeletion)
				{
				   this._senses.RemoveAt(i);
				}
			}
			if(count != this._senses.Count)
			{
				Palaso.Reporting.Logger.WriteMinorEvent("Empty sense removed");
				OnEmptyObjectsRemoved();
			}
		}

		public LexSense GetOrCreateSenseWithGloss(MultiText gloss)
		{
			foreach (LexSense sense in Senses)
			{
				if (gloss.HasFormWithSameContent(sense.Gloss))
				{
					return sense;
				}
			}
			LexSense newSense = (LexSense)Senses.AddNew();
			newSense.Gloss.MergeIn(gloss);
			return newSense;
		}

		public string GetToolTipText()
		{
			string s = "";
			foreach (LexSense sense in Senses)
			{
				s += sense.Gloss.GetFirstAlternative() + ", ";
			}
			if (s == "")
			{
				return StringCatalog.Get("~No Senses");
			}
			return s.Substring(0, s.Length - 2);// chop off the trailing separator
		}


		/// <summary>
		/// checks if it looks like the user has added info. this is used when changing spelling
		/// in a word-gathering task
		/// </summary>
		public bool IsEmptyExceptForLexemeFormForPurposesOfDeletion
		{
			get
			{
				if(LexicalForm.Count > 1)
				{
					return false;
				}
				foreach (LexSense sense in _senses)
				{
					if(!sense.IsEmptyForPurposesOfDeletion )
						return false;
				}
				if (HasPropertiesForPurposesOfDeletion)
					return false;

				return true;
			}
		}

		/// <summary>
		/// this is used to prevent things like cleanup of an object that is being deleted, which
		/// can lead to update notifications that get the dispossed entry back in the db, or in some cache
		/// </summary>
		public bool IsBeingDeleted
		{
			get { return _isBeingDeleted; }
			set { _isBeingDeleted = value; }
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

		public new LexEntry Parent
		{
			get { return base.Parent as LexEntry; }
		}
	}
}

using System;
using System.Collections.Generic;
using LiftIO;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel
{
	/// <summary>
	/// This class is called by the LiftParser, as it encounters each element of a lift file.
	/// There is at least one other ILexiconMerger, used in FLEX.
	///
	/// NB: this doesn't yet merge (dec 2006). Just blindly adds.
	/// </summary>
	public class LiftMerger : ILexiconMerger<WeSay.Foundation.WeSayDataObject, LexEntry,LexSense,LexExampleSentence>, IDisposable
	{
		private Db4oDataSource _dataSource;
	   // private WeSay.Data.Db4oRecordList<LexEntry> _entries;
		private IList<String> _expectedOptionTraits;
		private IList<string> _expectedOptionCollectionTraits;
//        private ProgressState _progressState = new NullProgressState();

		public LiftMerger(Db4oDataSource dataSource)
		{
			_dataSource = dataSource;
//            _entries = new WeSay.Data.Db4oRecordList<LexEntry>(_dataSource);
//            _entries.WriteCacheSize = 0;
			_expectedOptionTraits = new List<string>();
			_expectedOptionCollectionTraits = new List<string>();
		}

		public LexEntry GetOrMakeEntry(Extensible eInfo)
		{
			//Guid guid = GetGuidOrEmptyFromIdString(eInfo.Id);
			LexEntry entry = null;
			if (eInfo.Guid != Guid.Empty)
			{
				entry = Db4oLexQueryHelper.FindObjectFromGuid<LexEntry>(_dataSource, eInfo.Guid);

				if (CanSafelyPruneMerge(eInfo, entry))
				{
					return null; // no merging needed
				}
			}

			if (entry == null)
			{
				entry = new LexEntry(eInfo.Id, eInfo.Guid);
			}

			if (eInfo.CreationTime != default(DateTime))
			{
				entry.CreationTime = eInfo.CreationTime;
			}

			if (eInfo.ModificationTime != default(DateTime))
			{
				entry.ModificationTime = eInfo.ModificationTime;
			}
			return entry;
		}

		#region ILexiconMerger<WeSayDataObject,LexEntry,LexSense,LexExampleSentence> Members

		public LexEntry EntryWasDeleted(Extensible info, DateTime dateDeleted)
		{
			throw new NotImplementedException();
		}

		#endregion

		private static bool CanSafelyPruneMerge(Extensible eInfo, LexEntry entry)
		{
			return entry != null
				&& entry.ModificationTime == eInfo.ModificationTime
				&& entry.ModificationTime.Kind != DateTimeKind.Unspecified
				 && eInfo.ModificationTime.Kind != DateTimeKind.Unspecified;
		}

		private Guid GetGuidOrEmptyFromIdString(string id)
		{
			try
			{
				return new Guid(id);
			}
			catch (Exception)
			{
				//enchance: log this, we're throwing away the id they had
				return Guid.Empty;
			}
		}

		public LexSense GetOrMakeSense(LexEntry entry, Extensible eInfo)
		{
			//nb, has no guid or dates
			LexSense s= new LexSense(entry);
			entry.Senses.Add(s);

			return s;
		}

		public LexExampleSentence GetOrMakeExample(LexSense sense, Extensible eInfo)
		{
			LexExampleSentence ex = new LexExampleSentence(sense);
			sense.ExampleSentences.Add(ex);
			return ex;
		}

		public void MergeInLexemeForm(LexEntry entry, LiftMultiText forms)
		{
			MergeIn(entry.LexicalForm, forms);
		}



		public void MergeInGloss(LexSense sense, LiftMultiText forms)
		{
		   sense.Gloss.MergeInWithAppend(MultiText.Create(forms), "; ");
			foreach (Trait trait in forms.Traits )
			{
				if (trait.Name == "flag")
				{
					sense.Gloss.SetAnnotationOfAlternativeIsStarred(trait.Language, trait.Value == "1");
				}
			}
		}

		public void MergeInExampleForm(LexExampleSentence example, LiftMultiText forms)//, string optionalSource)
		{
			MergeIn(example.Sentence, forms);
		}

		public void MergeInTranslationForm(LexExampleSentence example, LiftMultiText forms)
		{
			MergeIn(example.Translation, forms);
		}

		public void MergeInDefinition(LexSense sense, LiftMultiText contents)
		{
			AddOrAppendMultiTextProperty(sense, contents, LexSense.WellKnownProperties.Note);
		}

		/// <summary>
		/// Handle LIFT's "note" entity
		/// </summary>
		public void MergeInNote(WeSayDataObject extensible, string type, LiftMultiText contents)
		{
			if (type != null && type != string.Empty)
			{
				List<String> keys = new List<string>(contents.Count);
				foreach (KeyValuePair<string, string> pair in contents)
				{
					keys.Add(pair.Key);
				}
				foreach (string s in keys)
				{
					contents.Prepend(s, "(" + type + ") ");
				}
			}
			AddOrAppendMultiTextProperty(extensible, contents, WeSayDataObject.WellKnownProperties.Note);
		}

		public void MergeInGrammaticalInfo(LexSense sense, string val)
		{
			OptionRef o = sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			o.Value = val;
		}

		private static void AddOrAppendMultiTextProperty(WeSayDataObject dataObject, LiftMultiText contents, string propertyName)
		{
			MultiText mt = dataObject.GetOrCreateProperty<MultiText>(propertyName);
			mt.MergeInWithAppend(MultiText.Create(contents), "; ");
			//dataObject.GetOrCreateProperty<string>(propertyName) mt));
		}

		private static void AddMultiTextProperty(WeSayDataObject dataObject, LiftMultiText contents, string propertyName)
		{
			dataObject.Properties.Add(
				new KeyValuePair<string, object>(propertyName,
												 MultiText.Create(contents)));
		}

		/// <summary>
		/// Handle LIFT's "field" entity which can be found on any subclass of "extensible"
		/// </summary>
		public void MergeInField(WeSayDataObject extensible, string tagAttribute, DateTime dateCreated,
								 DateTime dateModified, LiftMultiText contents)
		{
			MultiText t = MultiText.Create(contents);
			extensible.Properties.Add(new KeyValuePair<string, object>(tagAttribute, t));
		}

		/// <summary>
		/// Handle LIFT's "trait" entity,
		/// which can be found on any subclass of "extensible", on any "field", and as
		/// a subclass of "annotation".
		/// </summary>
		public void MergeInTrait(WeSayDataObject extensible, string name, string valueAttribute)
		{
				if (name != null && ExpectedOptionTraits.Contains(name))
				{
					OptionRef o = extensible.GetOrCreateProperty<OptionRef>(name);
					o.Value = valueAttribute;
				}
				else if (name != null && ExpectedOptionCollectionTraits.Contains(name))
				{
					OptionRefCollection c = extensible.GetOrCreateProperty<OptionRefCollection>(name);
					c.Keys.Add(valueAttribute);
				}
				else
				{
					//"log skipping..."
				}
		}

		public IList<string> ExpectedOptionTraits
		{
			get
			{
				return _expectedOptionTraits;
			}
		}

		public IList<string> ExpectedOptionCollectionTraits
		{
			get
			{
				return _expectedOptionCollectionTraits;
			}
		}

//        public ProgressState ProgressState
//        {
//            set
//            {
//                _progressState = value;
//            }
//        }


		private static void MergeIn(MultiText multiText, LiftMultiText forms)
		{

			multiText.MergeIn(MultiText.Create(forms));
		}

		 public void Dispose()
		{
			 //_entries.Dispose();
		}

		#region ILexiconMerger<WeSayDataObject,LexEntry,LexSense,LexExampleSentence> Members


		public void FinishEntry(LexEntry entry)
		{
			_dataSource.Data.Set(entry);
		}

		#endregion
	}
}

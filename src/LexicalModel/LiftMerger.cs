using System;
using System.Collections.Generic;
using LiftIO;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;

namespace WeSay.LexicalModel
{
	/// <summary>
	/// This class is called by the LiftParser, as it encounters each element of a lift file.
	/// There is at least one other ILexiconMerger, used in FLEX.
	///
	/// NB: this doesn't yet merge (dec 2006). Just blindly adds.
	/// </summary>
	public class LiftMerger : ILexiconMerger<WeSayDataObject, LexEntry,LexSense,LexExampleSentence>, IDisposable
	{
		private Db4oDataSource _dataSource;
		private IRecordList<LexEntry> _entries;
		private IList<String> _expectedOptionTraits;
		private IList<string> _expectedOptionCollectionTraits;

		/// <summary>
		/// related to homograph number calculation
		/// </summary>
		private IHistoricalEntryCountProvider _historicalEntryCountProvider;

		public LiftMerger(Db4oDataSource dataSource,  IRecordList<LexEntry> entries)
		{
			_dataSource = dataSource;
			_entries = entries;
			_expectedOptionTraits = new List<string>();
			_expectedOptionCollectionTraits = new List<string>();
			_historicalEntryCountProvider = HistoricalEntryCountProviderForDb4o.GetOrMakeFromDatabase(dataSource);
		}

		public LexEntry GetOrMakeEntry(Extensible eInfo)
		{
			LexEntry entry = null;
#if merging
	 This really slows us down to a crawl if the incoming lift has guids, yet
	 we aren't really merging, so its a waste of time.

			If or when we do need to handle merging, this can probably be sped up by getting
			the entire list of guids all in one go and then checking

			if (eInfo.Guid != Guid.Empty)
			{
				entry = Db4oLexQueryHelper.FindObjectFromGuid<LexEntry>(_dataSource, eInfo.Guid);

				if (CanSafelyPruneMerge(eInfo, entry))
				{
					return null; // no merging needed
				}
			}
#endif
			if (entry == null)
			{
				if (eInfo.CreationTime == default(DateTime))
				{
					eInfo.CreationTime = DateTime.UtcNow;
				}

				if (eInfo.ModificationTime == default(DateTime))
				{
					eInfo.ModificationTime = DateTime.UtcNow;
				}


				entry = new LexEntry(eInfo, _historicalEntryCountProvider);
			}

			entry.ModifiedTimeIsLocked = true; //while we build it up
			return entry;
		}

		#region ILexiconMerger<WeSayDataObject,LexEntry,LexSense,LexExampleSentence> Members

		public void  EntryWasDeleted(Extensible info, DateTime dateDeleted)
		{
			//there isn't anything we need to do; we just don't import it
			// since we always update file in place, the info will still stay in the lift file
			// even though we don't import it.
		}

		#endregion

#if merging
		private static bool CanSafelyPruneMerge(Extensible eInfo, LexEntry entry)
		{
			return entry != null
				&& entry.ModificationTime == eInfo.ModificationTime
				&& entry.ModificationTime.Kind != DateTimeKind.Unspecified
				 && eInfo.ModificationTime.Kind != DateTimeKind.Unspecified;
		}
#endif

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

		public void MergeInCitationForm(LexEntry entry, LiftMultiText contents)
		{
			AddOrAppendMultiTextProperty(entry, contents, LexEntry.WellKnownProperties.Citation);
		}

		public void MergeInGloss(LexSense sense, LiftMultiText forms)
		{
		   sense.Gloss.MergeInWithAppend(MultiText.Create(forms), "; ");
		   AddTraitsToMultiText(forms, sense.Gloss);
		}

		private static void AddTraitsToMultiText(LiftMultiText forms,  MultiText text)
		{
			foreach (Trait trait in forms.Traits )
			{
				if (trait.Name == "flag")
				{
					text.SetAnnotationOfAlternativeIsStarred(trait.LanguageHint, int.Parse(trait.Value) > 0);
				}
				else
				{
					//log dropped
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

		public void MergeInSource(LexExampleSentence example, string source)
		{
			OptionRef o = example.GetOrCreateProperty<OptionRef>(LexExampleSentence.WellKnownProperties.Source);
			o.Value = source;
		}

		public void MergeInDefinition(LexSense sense, LiftMultiText contents)
		{
			AddOrAppendMultiTextProperty(sense, contents, LexSense.WellKnownProperties.Definition);
		}

		public void MergeInPicture(LexSense sense, string href, LiftMultiText caption)
		{
			//nb 1:  we're limiting ourselves to one picture per sense, here:
			//nb 2: the name and case must match the fieldName
			PictureRef pict = sense.GetOrCreateProperty<PictureRef>("Picture");
			pict.Value = href;
			if (caption != null)
			{
				pict.Caption = MultiText.Create(caption);
			}
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

		public void MergeInGrammaticalInfo(LexSense sense, string val, List<Trait> traits)
		{
			OptionRef o = sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			o.Value = val;
			if (traits != null)
			{
				foreach (Trait trait in traits)
				{
					if (trait.Name == "flag" && int.Parse(trait.Value) > 0)
					{
						o.IsStarred = true;
					}
					else
					{
						//log skipping
					}
				}
			}
		}

		private static void AddOrAppendMultiTextProperty(WeSayDataObject dataObject, LiftMultiText contents, string propertyName)
		{
			MultiText mt = dataObject.GetOrCreateProperty<MultiText>(propertyName);
			mt.MergeInWithAppend(MultiText.Create(contents), "; ");
			//dataObject.GetOrCreateProperty<string>(propertyName) mt));
		}

/*
		private static void AddMultiTextProperty(WeSayDataObject dataObject, LiftMultiText contents, string propertyName)
		{
			dataObject.Properties.Add(
				new KeyValuePair<string, object>(propertyName,
												 MultiText.Create(contents)));
		}
*/

		/// <summary>
		/// Handle LIFT's "field" entity which can be found on any subclass of "extensible"
		/// </summary>
		public void MergeInField(WeSayDataObject extensible, string tagAttribute, DateTime dateCreated,
								 DateTime dateModified, LiftMultiText contents)
		{
			MultiText t = MultiText.Create(contents);
			extensible.Properties.Add(new KeyValuePair<string, object>(tagAttribute, t));

			/* good idea, but it doesn't work.  It doesn't cause an update to the lift xml
			 * if(extensible.GetType() == typeof(LexSense) && tagAttribute=="note")
			{
				//goose it into wanting to write this back out, in the corrected form (<note> instead of <field>)
				extensible.SomethingWasModified("note");
			}
			*/
		}

		/// <summary>
		/// Handle LIFT's "trait" entity,
		/// which can be found on any subclass of "extensible", on any "field", and as
		/// a subclass of "annotation".
		/// </summary>
		public void MergeInTrait(WeSayDataObject extensible, Trait trait)
		{
			if(String.IsNullOrEmpty(trait.Name))
			{
				//"log skipping..."
				return;
			}
			if (ExpectedOptionTraits.Contains(trait.Name))
			{
				OptionRef o = extensible.GetOrCreateProperty<OptionRef>(trait.Name);
				o.Value = trait.Value;
			}
			else if(trait.Name.StartsWith("flag_"))
			{
				extensible.SetFlag(trait.Name);
			}
				// if it is unknown assume it is a collection.
			else //if (ExpectedOptionCollectionTraits.Contains(trait.Name))
			{
				OptionRefCollection c = extensible.GetOrCreateProperty<OptionRefCollection>(trait.Name);
				c.Add(trait.Value);
			}
			//else
			//{
			//    //"log skipping..."
			//}
		}

		public void MergeInRelation(WeSayDataObject extensible, string relationFieldId, string targetId)
		{
			if (String.IsNullOrEmpty(relationFieldId))
			{
				return; //"log skipping..."
			}

			//the "field name" of a relation equals the name of the relation type
			LexRelationCollection collection=  extensible.GetOrCreateProperty<LexRelationCollection>(relationFieldId);
			LexRelation relation = new LexRelation(relationFieldId, targetId, extensible);
			collection.Relations.Add(relation);
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
			AddTraitsToMultiText(forms, multiText);
		}

		 public void Dispose()
		{
			 //_entries.Dispose();
		}

		#region ILexiconMerger<WeSayDataObject,LexEntry,LexSense,LexExampleSentence> Members


		public void FinishEntry(LexEntry entry)
		{
			entry.GetOrCreateId(false);
			entry.ModifiedTimeIsLocked = false;
			if (!_entries.Contains(entry))
			{
				_entries.Add(entry);
			}
		}

		#endregion
	}
}

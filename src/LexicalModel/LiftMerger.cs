using System;
using System.Collections.Generic;
using LiftIO.Parsing;
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
		private readonly LexEntryRepository _lexEntryRepository;
		private readonly IList<String> _expectedOptionTraits;
		private readonly IList<string> _expectedOptionCollectionTraits;

		public LiftMerger(LexEntryRepository lexEntryRepository)
		{
			_lexEntryRepository = lexEntryRepository;
			_expectedOptionTraits = new List<string>();
			_expectedOptionCollectionTraits = new List<string>();
		}

		public LexEntry GetOrMakeEntry(Extensible eInfo, int order)
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


				entry = _lexEntryRepository.CreateItem(eInfo);
			}


			entry.ModifiedTimeIsLocked = true; //while we build it up
			entry.OrderForRoundTripping = order;
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

		public LexSense GetOrMakeSense(LexEntry entry, Extensible eInfo, string rawXml)
		{
			//nb, has no guid or dates
			LexSense s= new LexSense(entry);
			s.Id = eInfo.Id;
			entry.Senses.Add(s);

			return s;
		}

		public LexSense GetOrMakeSubsense(LexSense sense, Extensible info, string rawXml)
		{
			sense.GetOrCreateProperty<EmbeddedXmlCollection>("subSense").Values.Add(rawXml);

			return null;
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
			AddOrAppendMultiTextProperty(entry, contents, LexEntry.WellKnownProperties.Citation, null);
		}

		public WeSayDataObject MergeInPronunciation(LexEntry entry, LiftMultiText contents, string rawXml)
		{
			entry.GetOrCreateProperty<EmbeddedXmlCollection>("pronunciation").Values.Add(rawXml);
			return null;
		}

		public WeSayDataObject MergeInVariant(LexEntry entry, LiftMultiText contents, string rawXml)
		{
			entry.GetOrCreateProperty<EmbeddedXmlCollection>("variant").Values.Add(rawXml);
			return null;
		}

		public void MergeInGloss(LexSense sense, LiftMultiText forms)
		{
		   sense.Gloss.MergeInWithAppend(MultiText.Create(forms.AsSimpleStrings), "; ");
		   AddAnnotationsToMultiText(forms, sense.Gloss);
		}

		private static void AddAnnotationsToMultiText(LiftMultiText forms,  MultiText text)
		{
			foreach (Annotation annotation in forms.Annotations)
			{
				if (annotation.Name == "flag")
				{
					text.SetAnnotationOfAlternativeIsStarred(annotation.LanguageHint, int.Parse(annotation.Value) > 0);
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

		public void MergeInTranslationForm(LexExampleSentence example, string type, LiftMultiText forms, string rawXml)
		{
			bool alreadyHaveAPrimaryTranslation = example.Translation != null && !string.IsNullOrEmpty(example.Translation.GetFirstAlternative());
			bool typeIsCompatibleWithWeSayPrimaryTranslation = string.IsNullOrEmpty(type) || type == "free";
			if(!alreadyHaveAPrimaryTranslation && typeIsCompatibleWithWeSayPrimaryTranslation)
			{
				MergeIn(example.Translation, forms);
				example.TranslationType = type;
			}
			else
			{
				example.GetOrCreateProperty<EmbeddedXmlCollection>("translation").Values.Add(rawXml);
			}
		}

		public void MergeInSource(LexExampleSentence example, string source)
		{
			OptionRef o = example.GetOrCreateProperty<OptionRef>(LexExampleSentence.WellKnownProperties.Source);
			o.Value = source;
		}

		public void MergeInDefinition(LexSense sense, LiftMultiText contents)
		{
			AddOrAppendMultiTextProperty(sense, contents, LexSense.WellKnownProperties.Definition,null);
		}

		public void MergeInPicture(LexSense sense, string href, LiftMultiText caption)
		{
			//nb 1:  we're limiting ourselves to one picture per sense, here:
			//nb 2: the name and case must match the fieldName
			PictureRef pict = sense.GetOrCreateProperty<PictureRef>("Picture");
			pict.Value = href;
			if (caption != null)
			{
				pict.Caption = MultiText.Create(caption.AsSimpleStrings);
			}
		}

		/// <summary>
		/// Handle LIFT's "note" entity
		/// </summary>
		public void MergeInNote(WeSayDataObject extensible, string type, LiftMultiText contents)
		{
			List<String> writingSystemAlternatives = new List<string>(contents.Count);
			foreach (KeyValuePair<string, string> pair in contents.AsSimpleStrings)
			{
				writingSystemAlternatives.Add(pair.Key);
			}

			if (!string.IsNullOrEmpty(type))
			{
				List<String> keys = new List<string>(contents.Count);
				foreach (KeyValuePair<string, string> pair in contents.AsSimpleStrings)
				{
					keys.Add(pair.Key);
				}
				foreach (string s in keys)
				{
					contents.Prepend(s, "(" + type + ") ");
				}
			}

			AddOrAppendMultiTextProperty(extensible, contents, WeSayDataObject.WellKnownProperties.Note, " || ");
		}



		public WeSayDataObject GetOrMakeParentReversal(WeSayDataObject parent, LiftMultiText contents, string type)
		{
			return null; // we'll get what we need from the rawxml of MergeInReversal
		}

		public WeSayDataObject MergeInReversal(LexSense sense, WeSayDataObject parent, LiftMultiText contents,
											   string type, string rawXml)
		{
			sense.GetOrCreateProperty<EmbeddedXmlCollection>("reversal").Values.Add(rawXml);
			return null;
		}

		public WeSayDataObject MergeInEtymology(LexEntry entry, string source, LiftMultiText form, LiftMultiText gloss, string rawXml)
		{
			entry.GetOrCreateProperty<EmbeddedXmlCollection>("etymology").Values.Add(rawXml);

			return null;
		}

		public void ProcessRangeElement(string range, string id, string guid, string parent, LiftMultiText description,
										LiftMultiText label, LiftMultiText abbrev)
		{

		}

		public void ProcessFieldDefinition(string tag, LiftMultiText description)
		{

		}

		public void MergeInGrammaticalInfo(WeSayDataObject senseOrReversal, string val, List<Trait> traits)
		{
			LexSense sense = senseOrReversal as LexSense;
			if (sense == null)
			{
				return; //todo: preserve grammatical info on reversal, when we hand reversals
			}

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

		private static void AddOrAppendMultiTextProperty(WeSayDataObject dataObject, LiftMultiText contents, string propertyName, string noticeToPrependIfNotEmpty)
		{
			MultiText mt = dataObject.GetOrCreateProperty<MultiText>(propertyName);
			mt.MergeInWithAppend(MultiText.Create(contents.AsSimpleStrings), string.IsNullOrEmpty(noticeToPrependIfNotEmpty) ? "; " : noticeToPrependIfNotEmpty);
			AddAnnotationsToMultiText(contents, mt);

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
		public void MergeInField(WeSayDataObject extensible, string typeAttribute, DateTime dateCreated,
								 DateTime dateModified, LiftMultiText contents, List<Trait> traits)
		{
			MultiText t = MultiText.Create(contents.AsSimpleStrings);

			//enchance: instead of KeyValuePair, make a LiftField class, so we can either keep the
			// other field stuff as xml (in order to round-trip it) or model it.

			extensible.Properties.Add(new KeyValuePair<string, object>(typeAttribute, t));
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

		public void MergeInRelation(WeSayDataObject extensible, string relationFieldId, string targetId, string rawXml)
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

			multiText.MergeIn(MultiText.Create(forms.AsSimpleStrings));
			AddAnnotationsToMultiText(forms, multiText);
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
			_lexEntryRepository.SaveItem(entry);
		}

		#endregion
	}
}

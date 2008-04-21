using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using LiftIO.Validation;
using Palaso.Annotations;
using Palaso.Reporting;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.Project
{
	public class LiftExporter
	{
		public const string LiftDateTimeFormat = "yyyy-MM-ddThh:mm:ssZ";
		private XmlWriter _writer;
		private Dictionary<string, int> _allIdsExportedSoFar;
		private ViewTemplate _viewTemplate;
		private IHomographCalculator _homographCalculator;
		private IFindEntries _entryFinder;

		[Flags]
		public enum Options
		{
			NormalLift = 0,
			DereferenceRelations = 1,
			DetermineHeadword = 1
		} ;

		private Options _options= Options.NormalLift;
		private string _headWordWritingSystemId;


		//   private Dictionary<string, string> _fieldToRangeSetPairs;
		protected LiftExporter()
		{
			_allIdsExportedSoFar = new Dictionary<string, int>();
		}

		public LiftExporter(/*Dictionary<string, string> fieldToOptionListName, */string path): this()
		{
			//   _fieldToRangeSetPairs = fieldToOptionListName;
			_writer = XmlWriter.Create(path, PrepareSettings(false));
			Start();
		}

		/// <summary>
		/// for automated testing
		/// </summary>`
		public LiftExporter(/*Dictionary<string, string> fieldToOptionListName,*/ StringBuilder builder, bool produceFragmentOnly)
			:this()
		{
			_writer = XmlWriter.Create(builder, PrepareSettings(produceFragmentOnly));
			if (!produceFragmentOnly)
			{
				Start();
			}
		}

		public void SetUpForPresentationLiftExport(ViewTemplate template, IHomographCalculator homographCalculator, IFindEntries entryFinder)
		{
			_homographCalculator = homographCalculator;
			ExportOptions = LiftExporter.Options.DereferenceRelations | Options.DetermineHeadword;
			_entryFinder = entryFinder;
			Template = template;
		}

//        public Dictionary<string, string> FieldToRangeSetPairs
//        {
//            get
//            {
//                return _fieldToRangeSetPairs;
//            }
//            set
//            {
//                _fieldToRangeSetPairs = value;
//            }
//        }

		private static XmlWriterSettings PrepareSettings(bool produceFragmentOnly)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			if (produceFragmentOnly)
			{
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.Indent = false;//helps with tests that just do a string compare
			}
			else
			{
				settings.Indent = true;
			}
			// this will give you a bom, which messes up princexml :settings.Encoding = Encoding.UTF8;
			Encoding utf8NoBom = new UTF8Encoding(false);
			settings.Encoding = utf8NoBom;
			settings.CloseOutput = true;
			return settings;
		}

		private void Start()
		{

			_writer.WriteStartDocument();
			_writer.WriteStartElement("lift");
			_writer.WriteAttributeString("version", Validator.LiftVersion);
			_writer.WriteAttributeString("producer",
										 ProducerString);
			// _writer.WriteAttributeString("xmlns", "flex", null, "http://fieldworks.sil.org");
		}

		public static string ProducerString
		{
			get
			{
				return "WeSay " +
					   Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		/// <summary>
		/// Set this if you want the output filtered to the visible fields and the writing system orders respected
		/// </summary>
		public ViewTemplate Template
		{
			get { return _viewTemplate; }
			set { _viewTemplate = value; }
		}

		public IHomographCalculator HomographCalculator
		{
			get { return _homographCalculator; }
			set { _homographCalculator = value; }
		}

		public IFindEntries EntryFinder
		{
			get { return _entryFinder; }
			set { _entryFinder = value; }
		}

		public Options ExportOptions
		{
			get { return _options; }
			set { _options = value; }
		}

		public void End()
		{
			if (_writer.Settings.ConformanceLevel != ConformanceLevel.Fragment)
			{
				_writer.WriteEndElement();//lift
				_writer.WriteEndDocument();

			}
			_writer.Flush();
			_writer.Close();
		}

		public void Add(IList<LexEntry> entries, int startIndex, int howMany)
		{
			for (int i = startIndex; i < startIndex+howMany; i++)
			{
				Add(entries[i]);
			}
		}

		public void Add(IEnumerable<LexEntry> entries)
		{
			foreach (LexEntry entry in entries)
			{
				Add(entry);
			}
		}

		public void AddNoGeneric(IEnumerable entries)
		{
			foreach (LexEntry entry in entries)
			{
				Add(entry);
			}
		}

		public void Add(LexEntry entry)
		{
			List<string> propertiesAlreadyOutput=new List<string>();

			_writer.WriteStartElement("entry");
			_writer.WriteAttributeString("id", GetHumanReadableId(entry, _allIdsExportedSoFar));

			if (_homographCalculator != null)
			{
				int h = _homographCalculator.GetHomographNumber(entry);
				if (h > 0)
				{
					  _writer.WriteAttributeString("order", h.ToString());
				}
			}

			System.Diagnostics.Debug.Assert(entry.CreationTime.Kind == DateTimeKind.Utc);
			_writer.WriteAttributeString("dateCreated", entry.CreationTime.ToString(LiftDateTimeFormat));
			System.Diagnostics.Debug.Assert(entry.ModificationTime.Kind == DateTimeKind.Utc);
			_writer.WriteAttributeString("dateModified", entry.ModificationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("guid", entry.Guid.ToString());
			// _writer.WriteAttributeString("flex", "id", "http://fieldworks.sil.org", entry.Guid.ToString());
			WriteMultiWithWrapperIfNonEmpty(LexEntry.WellKnownProperties.LexicalUnit, "lexical-unit",entry.LexicalForm);

			if (0 != (_options & Options.DetermineHeadword))
			{
				WriteHeadWordField(entry,"headword");
			}
			WriteWellKnownCustomMultiText(entry, LexEntry.WellKnownProperties.Citation, propertiesAlreadyOutput);
			WriteWellKnownCustomMultiText(entry, LexEntry.WellKnownProperties.Note, propertiesAlreadyOutput);
			WriteCustomProperties(entry, propertiesAlreadyOutput);
			foreach (LexSense sense in entry.Senses)
			{
				Add(sense);
			}
			_writer.WriteEndElement();
		}

		/// <summary>
		/// nb: this is used both for the headword of an article, but also for the target of a relation.
		/// </summary>
		private void WriteHeadWordField(LexEntry entry, string outputFieldName)
		{
			if(Template == null)
			{
				throw new ArgumentException("Expected a non-null Template");
			}
			MultiText headword = new MultiText();
			Field fieldControllingHeadwordOutput = Template.GetField(LexEntry.WellKnownProperties.Citation);
			if(fieldControllingHeadwordOutput == null || !fieldControllingHeadwordOutput.Enabled )
			{
				fieldControllingHeadwordOutput = Template.GetField(LexEntry.WellKnownProperties.LexicalUnit);
				if (fieldControllingHeadwordOutput == null)
				{
					throw new ArgumentException("Expected to find LexicalUnit in the view Template");
				}
			}
			//                headword.SetAlternative(HeadWordWritingSystemId, entry.GetHeadWordForm(HeadWordWritingSystemId));

			foreach (string writingSystemId in fieldControllingHeadwordOutput.WritingSystemIds)
			{
				headword.SetAlternative(writingSystemId, entry.GetHeadWordForm(writingSystemId));
			}
			WriteMultiTextAsArtificialField(outputFieldName, headword);
		}


		/// <summary>
		/// use this for multitexts that were somehow constructed during export, with no corresponding single property
		/// </summary>
		private void WriteMultiTextAsArtificialField(string outputFieldName, MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				_writer.WriteStartElement("field");

				_writer.WriteAttributeString("type", outputFieldName);

				if (!MultiText.IsEmpty(text))
				{
					Add(text.Forms, true);
				}

				_writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Get a human readable identifier for this entry taking into account all the rest of the
		/// identifiers that this has seen
		/// </summary>
		/// <param name="entry">the entry to </param>
		/// <param name="idsAndCounts">the base ids that have been used so far and how many times</param>
		/// <remarks>This function alters the idsAndCounts and thus is not stable if the entry
		/// does not already have an id and the same idsAndCounts dictionary is provided.
		/// A second call to this function with the same entry that lacks an id and the same
		/// idsAndCounts will produce different results each time it runs
		/// </remarks>
		/// <returns>A base id composed with its count</returns>
		static public string GetHumanReadableId(LexEntry entry, Dictionary<string, int> idsAndCounts)
		{
			string id = entry.GetOrCreateId(true);
			/*         if (id == null || id.Length == 0)       // if the entry doesn't claim to have an id
			{
				id = entry.LexicalForm.GetFirstAlternative().Trim().Normalize(NormalizationForm.FormD); // use the first form as an id
				if (id == "")
				{
					id = "NoForm"; //review
				}
			}
			id = id.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
			//make this id unique
			int count;
			if (idsAndCounts.TryGetValue(id, out count))
			{
				++count;
				idsAndCounts.Remove(id);
				idsAndCounts.Add(id, count);
				id = string.Format("{0}_{1}", id, count);
			}
			else
			{
				idsAndCounts.Add(id, 1);
			}
			*/
			return id;
		}

		public void Add(LexSense sense)
		{
			List<string> propertiesAlreadyOutput = new List<string>();

			_writer.WriteStartElement("sense");
			_writer.WriteAttributeString("id", sense.GetOrCreateId());

			if (ShouldOutputProperty(LexSense.WellKnownProperties.PartOfSpeech))
			{
				WriteGrammi(sense);
				propertiesAlreadyOutput.Add(LexSense.WellKnownProperties.PartOfSpeech);
			}
			if (ShouldOutputProperty(LexSense.WellKnownProperties.Gloss))
			{
				WriteOneElementPerFormIfNonEmpty(LexSense.WellKnownProperties.Gloss, "gloss", sense.Gloss, ';');
				propertiesAlreadyOutput.Add(LexSense.WellKnownProperties.Gloss);
			}


			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
				Add(example);
			}
			WriteWellKnownCustomMultiText(sense, LexSense.WellKnownProperties.Definition, propertiesAlreadyOutput);
			WriteWellKnownCustomMultiText(sense, LexSense.WellKnownProperties.Note, propertiesAlreadyOutput);
		 //   WriteWellKnownUnimplementedProperty(sense, LexSense.WellKnownProperties.Note, propertiesAlreadyOutput);
			WriteCustomProperties(sense, propertiesAlreadyOutput);
			_writer.WriteEndElement();
		}

		private void WriteGrammi(LexSense sense)
		{
			if (!ShouldOutputProperty(LexSense.WellKnownProperties.PartOfSpeech))
				return;

			OptionRef pos = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);

			if (pos != null && !pos.IsEmpty)
			{
				_writer.WriteStartElement("grammatical-info");
				_writer.WriteAttributeString("value", pos.Value);
				WriteFlags(pos);
				_writer.WriteEndElement();
			}
			else
			{
				//review
				// I think this is right (Eric)
			}
		}

		private void WriteWellKnownCustomMultiText(WeSayDataObject item, string property, List<string> propertiesAlreadyOutput)
		{
			if (ShouldOutputProperty(property))
			{
				MultiText m = item.GetProperty<MultiText>(property);
				if (WriteMultiWithWrapperIfNonEmpty(property, property, m))
				{
					propertiesAlreadyOutput.Add(property);
				}
			}
		}

		private bool ShouldOutputProperty(string property)
		{
			if(Template == null)
				return true;
			Field f = Template.GetField(property);
			if(f==null)
				return false;
			return (f.Enabled);
		}

		/// <summary>
		/// this is used both when we're just exporting to lift, and dont' want to filter or order, and
		/// when we are writing presentation-ready lift, when we do want to filter and order
		/// </summary>
		/// <param name="text"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private LanguageForm[] GetOrderedAndFilteredForms(MultiText text, string propertyName)
		{
			if (Template == null)
			{
				return text.Forms;
			}
			Field f = Template.GetField(propertyName);
			if (f == null)
			{
				return text.Forms;
			}
			return text.GetOrderedAndFilteredForms(f.WritingSystemIds);
		}

		private void WriteCustomProperties(WeSayDataObject item, List<string> propertiesAlreadyOutput)
		{
			foreach (KeyValuePair<string, object> pair in item.Properties)
			{
				if (propertiesAlreadyOutput.Contains(pair.Key))
				{
					continue;
				}
				if(!ShouldOutputProperty(pair.Key))
				{
					continue;
				}
				if (pair.Value is EmbeddedXmlCollection)
				{
					WriteEmbeddedXmlCollection(pair.Value as EmbeddedXmlCollection);
					continue;
				}
				if (pair.Value is MultiText)
				{
					WriteCustomMultiTextField(pair.Key, pair.Value as MultiText);
					continue;
				}
				if (pair.Value is OptionRef)
				{
					WriteOptionRef(pair.Key, pair.Value as OptionRef);
					continue;
				}
				if (pair.Value is OptionRefCollection)
				{
					WriteOptionRefCollection(pair.Key, pair.Value as OptionRefCollection);
					continue;
				}
				if (pair.Value is LexRelationCollection)
				{
					WriteRelationCollection(pair.Key, pair.Value as LexRelationCollection);
					continue;
				}
				if (pair.Value is FlagState)
				{
					WriteFlagState(pair.Key, pair.Value as FlagState);
					continue;
				}
				if (pair.Value is PictureRef)
				{
					PictureRef pictureRef = pair.Value as PictureRef;
					WriteURLRef("picture", pictureRef.Value, pictureRef.Caption );
					continue;
				}
				throw new ApplicationException(
					string.Format("The LIFT exporter was surprised to find a property '{0}' of type: {1}", pair.Key,
								  pair.Value.GetType()));
			}
		}

		private void WriteEmbeddedXmlCollection(EmbeddedXmlCollection collection)
		{
			foreach (string rawXml in collection.Values)
			{
				_writer.WriteRaw(rawXml);
			}
		}

		private void WriteURLRef(string key, string href, MultiText caption)
		{
			if (!string.IsNullOrEmpty(href))
			{
				_writer.WriteStartElement(key);
				_writer.WriteAttributeString("href", href);
				WriteMultiWithWrapperIfNonEmpty(key, "label", caption);
				_writer.WriteEndElement();
			}
		}

		private void WriteFlagState(string key, FlagState state)
		{
			if (state.Value) //skip it if it's not set
			{
				_writer.WriteStartElement("trait");
				_writer.WriteAttributeString("name", key);
				_writer.WriteAttributeString("value", "set");//this attr required by lift schema, though we don't use it
				_writer.WriteEndElement();
			}
		}

		private void WriteRelationCollection(string key, LexRelationCollection collection)
		{
			if (!ShouldOutputProperty(key))
				return;

			foreach (LexRelation relation in collection.Relations)
			{
				_writer.WriteStartElement("relation");
				_writer.WriteAttributeString("type", relation.FieldId);
				_writer.WriteAttributeString("ref", relation.Key);
				if (0 != (ExportOptions & Options.DereferenceRelations))
				{
					Debug.Assert(_entryFinder != null, "An IEntryFinder must be provide if DereferenceRelations is on.");
					LexEntry target = _entryFinder.FindFirstEntryMatchingId(relation.Key);
					if (target != null)
					{
						WriteHeadWordField(target, "headword-of-target");
					}
				}
				_writer.WriteEndElement();


			}
		}

		private string HeadWordWritingSystemId
		{
			get
			{
				if (_headWordWritingSystemId == null)
				{
					Debug.Assert(_viewTemplate != null,"Should not be in here if not template was specified.");
					if (_viewTemplate.HeadwordWritingSytem == null)
						throw new ConfigurationException("Could not get a HeadwordWritingSytem from the ViewTemplate.");
					if (string.IsNullOrEmpty(_viewTemplate.HeadwordWritingSytem.Id))
						throw new ConfigurationException("HeadwordWritingSytem had an empty id.");
					//cache this
					_headWordWritingSystemId = _viewTemplate.HeadwordWritingSytem.Id;
				}
				return _headWordWritingSystemId;
			}
		}

		private void WriteOptionRefCollection(string traitName, OptionRefCollection collection)
		{
			if (!ShouldOutputProperty(traitName))
				return;
			foreach (string key in collection.Keys)
			{
				_writer.WriteStartElement("trait");
				_writer.WriteAttributeString("name", traitName);
				_writer.WriteAttributeString("value", key);//yes, the 'value' here is an option key
				// WriteRangeName(key);
				_writer.WriteEndElement();
			}
		}

		private void WriteCustomMultiTextField(string tag, MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				_writer.WriteStartElement("field");

				_writer.WriteAttributeString("type", tag);
				WriteMultiTextNoWrapper(tag, text);
				_writer.WriteEndElement();
			}
		}

		private void WriteOptionRef(string key, OptionRef optionRef)
		{
			if (optionRef.Value.Length > 0)
			{
				_writer.WriteStartElement("trait");
				_writer.WriteAttributeString("name", key);
				_writer.WriteAttributeString("value", optionRef.Value);
				//  WriteRangeName(key);
				_writer.WriteEndElement();
			}
		}

//        private void WriteRangeName(string key)
//        {
//            string rangeSet;
//            if (_fieldToRangeSetPairs.TryGetValue(key, out rangeSet))
//            {
//                _writer.WriteAttributeString("range", rangeSet);
//            }
//        }

		public void Add(LexExampleSentence example)
		{
			if (!ShouldOutputProperty(LexExampleSentence.WellKnownProperties.ExampleSentence))
				return;

			List<string> propertiesAlreadyOutput = new List<string>();
			_writer.WriteStartElement("example");

			OptionRef source;

			source = example.GetProperty<OptionRef>(LexExampleSentence.WellKnownProperties.Source);

			if (source != null && source.Value.Length > 0)
			{
				if (ShouldOutputProperty(LexExampleSentence.WellKnownProperties.Source))
				{
					_writer.WriteAttributeString("source", source.Value);
					propertiesAlreadyOutput.Add("source");
				}
			}

			WriteMultiTextNoWrapper(LexExampleSentence.WellKnownProperties.ExampleSentence, example.Sentence);
			WriteMultiWithWrapperIfNonEmpty(LexExampleSentence.WellKnownProperties.Translation, "translation", example.Translation);

			if (ShouldOutputProperty(LexExampleSentence.WellKnownProperties.ExampleSentence))
			{
				WriteWellKnownCustomMultiText(example, LexExampleSentence.WellKnownProperties.Note,
											  propertiesAlreadyOutput);
			}


			WriteCustomProperties(example, propertiesAlreadyOutput);
			_writer.WriteEndElement();
		}



		public void Add(string propertyName, MultiText text)
		{
			Add(GetOrderedAndFilteredForms(text, propertyName), false);
		}

		private void Add(LanguageForm[] forms, bool doMarkTheFirst)
		{
			foreach (LanguageForm form in forms)
			{
				_writer.WriteStartElement("form");
				_writer.WriteAttributeString("lang", form.WritingSystemId);
				if (doMarkTheFirst)
				{
					doMarkTheFirst = false;
					_writer.WriteAttributeString("first", "true");//useful for headword
				}
				_writer.WriteStartElement("text");
				_writer.WriteString(form.Form);
				_writer.WriteEndElement();

				WriteFlags(form);
				_writer.WriteEndElement();
			}
		}

		private void WriteFlags(IAnnotatable thing)
		{
			if (thing.IsStarred )
			{
				_writer.WriteStartElement("annotation");
				_writer.WriteAttributeString("name", "flag");
				_writer.WriteAttributeString("value", "1");
				_writer.WriteEndElement();
			}
		}


		private void WriteMultiTextNoWrapper(string propertyName, MultiText text)
		{

			if (!MultiText.IsEmpty(text))
			{
				Add(propertyName, text);
			}
		}

		private void WriteOneElementPerFormIfNonEmpty(string propertyName, string wrapperName, MultiText text, char delimeter)
		{
			if (!MultiText.IsEmpty(text))
			{
				foreach (LanguageForm alternative in GetOrderedAndFilteredForms(text, propertyName))
				{
						foreach (string part in alternative.Form.Split(new char[] { delimeter }))
						{
							string trimmed = part.Trim();
							if (part != string.Empty)
							{
								_writer.WriteStartElement(wrapperName);
								_writer.WriteAttributeString("lang", alternative.WritingSystemId);
								_writer.WriteStartElement("text");
								_writer.WriteString(trimmed);
								_writer.WriteEndElement();
								WriteFlags(alternative);
								_writer.WriteEndElement();
							}
						}
				}
			}
		}




		private bool WriteMultiWithWrapperIfNonEmpty(string propertyName, string wrapperName, MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				_writer.WriteStartElement(wrapperName);
				Add(propertyName, text);
				_writer.WriteEndElement();
				return true;
			}
			return false;
		}

		public void AddDeletedEntry(LexEntry entry)
		{
			_writer.WriteStartElement("entry");
			_writer.WriteAttributeString("id", GetHumanReadableId(entry, _allIdsExportedSoFar));
			_writer.WriteAttributeString("dateCreated", entry.CreationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("dateModified", entry.ModificationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("guid", entry.Guid.ToString());
			_writer.WriteAttributeString("dateDeleted", DateTime.UtcNow.ToString(LiftDateTimeFormat));

			_writer.WriteEndElement();
		}
	}


}
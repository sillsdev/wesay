using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using Palaso.Annotations;
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
			settings.Encoding = Encoding.UTF8;
			settings.CloseOutput = true;
			return settings;
		}

		private void Start()
		{

			_writer.WriteStartDocument();
			_writer.WriteStartElement("lift");
			_writer.WriteAttributeString("version", LiftIO.Validator.LiftVersion);
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
			System.Diagnostics.Debug.Assert(entry.CreationTime.Kind == DateTimeKind.Utc);
			_writer.WriteAttributeString("dateCreated", entry.CreationTime.ToString(LiftDateTimeFormat));
			System.Diagnostics.Debug.Assert(entry.ModificationTime.Kind == DateTimeKind.Utc);
			_writer.WriteAttributeString("dateModified", entry.ModificationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("guid", entry.Guid.ToString());
			// _writer.WriteAttributeString("flex", "id", "http://fieldworks.sil.org", entry.Guid.ToString());
			WriteMultiWithWrapperIfNonEmpty(LexEntry.WellKnownProperties.LexicalUnit, "lexical-unit",entry.LexicalForm);

			WriteWellKnownCustomMultiTextIfVisible(entry, LexEntry.WellKnownProperties.Citation, propertiesAlreadyOutput);
			WriteWellKnownCustomMultiTextIfVisible(entry, LexEntry.WellKnownProperties.Note, propertiesAlreadyOutput);
			WriteCustomPropertiesIfVisible(entry, propertiesAlreadyOutput);
			foreach (LexSense sense in entry.Senses)
			{
				Add(sense);
			}
			_writer.WriteEndElement();
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
			WriteWellKnownCustomMultiTextIfVisible(sense, LexSense.WellKnownProperties.Definition, propertiesAlreadyOutput);
			WriteWellKnownCustomMultiTextIfVisible(sense, LexSense.WellKnownProperties.Note, propertiesAlreadyOutput);
			WriteCustomPropertiesIfVisible(sense, propertiesAlreadyOutput);
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

		private void WriteWellKnownCustomMultiTextIfVisible(WeSayDataObject item, string property, List<string> propertiesAlreadyOutput)
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

		private void WriteCustomPropertiesIfVisible(WeSayDataObject item, List<string> propertiesAlreadyOutput)
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
				_writer.WriteAttributeString("name", relation.FieldId);
				_writer.WriteAttributeString("ref", relation.Key);
				_writer.WriteEndElement();
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

		private void WriteCustomMultiTextField(string key, MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				_writer.WriteStartElement("field");

				_writer.WriteAttributeString("tag", key);
				WriteMultiTextNoWrapper(key, text);
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
				WriteWellKnownCustomMultiTextIfVisible(example, LexExampleSentence.WellKnownProperties.Note,
											  propertiesAlreadyOutput);
			}


			WriteCustomPropertiesIfVisible(example, propertiesAlreadyOutput);
			_writer.WriteEndElement();
		}



		public void Add(string propertyName, MultiText text)
		{
			foreach (LanguageForm form in GetOrderedAndFilteredForms(text, propertyName))
			{
					_writer.WriteStartElement("form");
					_writer.WriteAttributeString("lang", form.WritingSystemId);

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
				_writer.WriteStartElement("trait");
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
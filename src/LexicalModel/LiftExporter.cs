using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LiftExporter
	{
		public const string LiftDateTimeFormat = "yyyy-MM-ddThh:mm:ssZ";
		private XmlWriter _writer;
		private Dictionary<string, int> _allIdsExportedSoFar;
		private Dictionary<string, string> _fieldToRangeSetPairs;

		public LiftExporter(Dictionary<string, string> fieldToOptionListName, string path)
		{
			_fieldToRangeSetPairs = fieldToOptionListName;
			_allIdsExportedSoFar = new Dictionary<string, int>();
		   _writer = XmlWriter.Create(path, PrepareSettings(false));
		   Start();
		}

		/// <summary>
		/// for automated testing
		/// </summary>
		public LiftExporter(Dictionary<string, string> fieldToOptionListName, StringBuilder builder, bool produceFragmentOnly)
		{
			_fieldToRangeSetPairs = fieldToOptionListName;

			_allIdsExportedSoFar = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
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
			WriteMultiWithWrapperIfNonEmpty( "lexical-unit",entry.LexicalForm);

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
			WriteGrammi(sense);
			propertiesAlreadyOutput.Add(LexSense.WellKnownProperties.PartOfSpeech);

			WriteOneElementPerFormIfNonEmpty("gloss", sense.Gloss, ';');
			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
				Add(example);
			}
			WriteWellKnownCustomMultiText(sense, LexSense.WellKnownProperties.Definition, propertiesAlreadyOutput);
			WriteWellKnownCustomMultiText(sense, LexSense.WellKnownProperties.Note, propertiesAlreadyOutput);
			WriteCustomProperties(sense, propertiesAlreadyOutput);
			_writer.WriteEndElement();
		}

		private void WriteGrammi(LexSense sense)
		{
			OptionRef pos = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);

			//For Dennis
 /*               OptionRef oldpos = sense.GetProperty<OptionRef>("PartOfSpeech");
				if (oldpos != null)
				{
					//move it
					sense.Properties.Remove(
						new KeyValuePair<string, object>("PartOfSpeech", oldpos));

					OptionRef existingPos = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
					if (existingPos == null)
					{
						sense.Properties.Add(
							new KeyValuePair<string, object>(LexSense.WellKnownProperties.PartOfSpeech, oldpos));
						pos = oldpos;
					}
				}
*/

			if (pos != null && pos.Value.Length > 0)
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
			MultiText m = item.GetProperty<MultiText>(property);
			if (WriteMultiWithWrapperIfNonEmpty(property, m))
			{
				propertiesAlreadyOutput.Add(property);
			}
		}

		private void WriteCustomProperties(WeSayDataObject item, List<string> propertiesAlreadyOutput)
		{
			foreach (KeyValuePair<string, object> pair in item.Properties)
			{
				if (propertiesAlreadyOutput.Contains(pair.Key))
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

				throw new ApplicationException(
					string.Format("The LIFT exporter was surprised to find a property '{0}' of type: {1}", pair.Key,
								  pair.Value.GetType()));
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
			foreach (LexRelation relation in collection.Relations)
			{
				_writer.WriteStartElement("relation");
				_writer.WriteAttributeString("name", relation.FieldId);
				_writer.WriteAttributeString("ref", relation.TargetId);
				_writer.WriteEndElement();
			}
		}

		private void WriteOptionRefCollection(string key, OptionRefCollection collection)
		{
			foreach (string value in collection)
			{
				_writer.WriteStartElement("trait");
				_writer.WriteAttributeString("name", key);
				_writer.WriteAttributeString("value", value);
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
				WriteMultiTextNoWrapper(text);
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
			List<string> propertiesAlreadyOutput = new List<string>();
			_writer.WriteStartElement("example");

			OptionRef source;
			//convert for dennis
/*            MultiText sourceAsMt = example.GetProperty<MultiText>(LexExampleSentence.WellKnownProperties.Source);
			if (!MultiText.IsEmpty(sourceAsMt))
			{
				example.Properties.Remove(new KeyValuePair<string, object>("source", sourceAsMt));

//                source = new OptionRef();
//
//               //take the old one out
//
//                example.Properties.Remove(new KeyValuePair<string, object>("source", sourceAsMt));
//
//                //put this one in
//                example.Properties.Add(
//                        new KeyValuePair<string, object>(LexExampleSentence.WellKnownProperties.Source, source));
//
//                source.Parent = example;
//                source.Value = sourceAsMt.GetFirstAlternative();
			}
 //           else
*/            {
				source = example.GetProperty<OptionRef>(LexExampleSentence.WellKnownProperties.Source);
			}
			if (source != null && source.Value.Length > 0)
			{
				_writer.WriteAttributeString("source", source.Value);
				propertiesAlreadyOutput.Add("source");
			}

			WriteMultiTextNoWrapper(example.Sentence);
			WriteMultiWithWrapperIfNonEmpty("translation", example.Translation);
			WriteWellKnownCustomMultiText(example, LexExampleSentence.WellKnownProperties.Note, propertiesAlreadyOutput);

			//for Dennis
			/*MultiText t = example.GetProperty<MultiText>("trans");
			if(t!=null)
			{
				Debug.Assert(!MultiText.IsEmpty( example.Translation));
				example.Properties.Remove(new KeyValuePair<string, object>("trans", t));
			}
			*/
			WriteCustomProperties(example, propertiesAlreadyOutput);
			_writer.WriteEndElement();
		}

		public void Add(MultiText text)
		{
			foreach (LanguageForm form in text)
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

		private void WriteFlags(Annotatable thing)
		{
			if (thing.IsStarred )
			{
				_writer.WriteStartElement("trait");
				_writer.WriteAttributeString("name", "flag");
				_writer.WriteAttributeString("value", "1");
				_writer.WriteEndElement();
			}
		}


		private void WriteMultiTextNoWrapper(MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				Add(text);
			}
		}

		private void WriteOneElementPerFormIfNonEmpty(string wrapperName, MultiText text, char delimeter)
		{
			if (!MultiText.IsEmpty(text))
			{
				foreach (LanguageForm form in text)
				{
					foreach (string part in form.Form.Split(new char[]{delimeter}))
					{
						string trimmed = part.Trim();
						if (part != string.Empty)
						{
							_writer.WriteStartElement(wrapperName);
							_writer.WriteAttributeString("lang", form.WritingSystemId);
							_writer.WriteStartElement("text");
							_writer.WriteString(trimmed);
							_writer.WriteEndElement();
							WriteFlags(form);
							_writer.WriteEndElement();

						}
					}
				}
			}
		}


		private bool WriteMultiWithWrapperIfNonEmpty(string wrapperName, MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				_writer.WriteStartElement(wrapperName);
				Add(text);
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

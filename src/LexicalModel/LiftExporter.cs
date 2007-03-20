using System;
using System.Collections;
using System.Collections.Generic;
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
		private Dictionary<string, int> _forms;
		static List<string> _reservedNames = new List<string>(new string[] { "PartOfSpeech" });
		private Dictionary<string, string> _fieldToRangeSetPairs;

		public LiftExporter(Dictionary<string, string> fieldToOptionListName, string path)
		{
			_fieldToRangeSetPairs = fieldToOptionListName;
			_forms = new Dictionary<string, int>();
		   _writer = XmlWriter.Create(path, PrepareSettings(false));
		   Start();
		}

		/// <summary>
		/// for automated testing
		/// </summary>
		public LiftExporter(Dictionary<string, string> fieldToOptionListName, StringBuilder builder, bool produceFragmentOnly)
		{
			_fieldToRangeSetPairs = fieldToOptionListName;

			_forms = new Dictionary<string, int>();
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
				settings.ConformanceLevel = ConformanceLevel.Fragment;

			settings.Encoding = Encoding.UTF8;
			settings.CloseOutput = true;
			return settings;
		}

		private void Start()
		{

			_writer.WriteStartDocument();
			_writer.WriteStartElement("lift");
			_writer.WriteAttributeString("producer", "WeSay.1Pt0Alpha");
			_writer.WriteAttributeString("xmlns", "flex", null, "http://fieldworks.sil.org");

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

		public void Add(IList<LexEntry> entries)
		{
			foreach (LexEntry entry in entries)
			{
				Add(entry);
			}
		}

		public void AddNoGeneric(IList entries)
		{
			foreach (LexEntry entry in entries)
			{
				Add(entry);
			}
		}

		public void Add(LexEntry entry)
		{
			_writer.WriteStartElement("entry");
			_writer.WriteAttributeString("id", MakeHumanReadableId(entry));
			_writer.WriteAttributeString("dateCreated", entry.CreationTime.ToString(LiftDateTimeFormat));
			System.Diagnostics.Debug.Assert(entry.CreationTime.Kind == DateTimeKind.Utc);
			 System.Diagnostics.Debug.Assert(entry.ModificationTime.Kind == DateTimeKind.Utc);
		   _writer.WriteAttributeString("dateModified", entry.ModificationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("flex", "id", "http: //fieldworks.sil.org", entry.Guid.ToString());
			WriteMultiWithWrapperIfNonEmpty( "lexical-unit",entry.LexicalForm);

			foreach(LexSense sense in entry.Senses)
			{
				Add(sense);
			}
			WriteCustomProperties(entry);
			_writer.WriteEndElement();
		}

		private string MakeHumanReadableId(LexEntry entry)
		{

			string form = entry.LexicalForm.GetFirstAlternative().Trim();
			if (form == "")
			{
				form = "NoForm"; //review
			}

			int count=0;
			if (_forms.TryGetValue(form, out count))
			{
				++count;
				_forms.Remove(form);
				_forms.Add(form, count);
				form = string.Format("{0}_{1}", form, count);
			}
			else
			{
				_forms.Add(form, 1);
			}

			return form;
		}

		public void Add(LexSense sense)
		{
			_writer.WriteStartElement("sense");
			WriteGrammi(sense);
			WriteMultiWithWrapperIfNonEmpty("gloss", sense.Gloss);
			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
				Add(example);
			}
			WriteCustomProperties(sense);
			_writer.WriteEndElement();
		}

		private void WriteGrammi(LexSense sense)
		{
			OptionRef pos = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			if (pos!=null)
			{
				_writer.WriteStartElement("grammatical-info");
				_writer.WriteAttributeString("value", ((OptionRef)pos).Value);
				//todo add flex:id
				_writer.WriteEndElement();
			}
			else
			{
				//review
			}
		}

		private void WriteCustomProperties(WeSayDataObject item)
		{
			foreach (KeyValuePair<string, object> pair in item.Properties)
			{
				if (_reservedNames.Contains(pair.Key))
				{
					continue;
				}
				if (pair.Value is MultiText)
				{
					WriteMultiWithWrapperIfNonEmpty(pair.Key, pair.Value as MultiText);
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
				throw new ApplicationException(
					string.Format("The LIFT exporter was surprised to find a property '{0}' of type: {1}", pair.Key,
								  pair.Value.GetType()));
			}
		}

		private void WriteOptionRefCollection(string key, OptionRefCollection collection)
		{
			foreach (string value in collection.Keys)
			{
				_writer.WriteStartElement("trait");
				_writer.WriteAttributeString("name", key);
				_writer.WriteAttributeString("value", value);
				WriteRangeName(key);
				_writer.WriteEndElement();
			}
		}

		private void WriteOptionRef(string key, OptionRef optionRef)
		{
			_writer.WriteStartElement("trait");
			_writer.WriteAttributeString("name", key);
			_writer.WriteAttributeString("value", optionRef.Value);
			string rangeSet;
			WriteRangeName(key);
			_writer.WriteEndElement();

			//todo add range name
		}

		private void WriteRangeName(string key)
		{
			string rangeSet;
			if (_fieldToRangeSetPairs.TryGetValue(key, out rangeSet))
			{
				_writer.WriteAttributeString("range", rangeSet);
			}
		}

		public void Add(LexExampleSentence example)
		{
			_writer.WriteStartElement("example");
			WriteMultiTextNoWrapper(example.Sentence);
			WriteMultiWithWrapperIfNonEmpty("translation", example.Translation);
			WriteCustomProperties(example);
			_writer.WriteEndElement();
		}

		public void Add(MultiText text)
		{
			foreach (LanguageForm form in text)
			{
				_writer.WriteStartElement("form");
				_writer.WriteAttributeString("lang", form.WritingSystemId);
				_writer.WriteString(form.Form);
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
		private void WriteMultiWithWrapperIfNonEmpty(string wrapperName, MultiText text)
		{
			if (!MultiText.IsEmpty(text))
			{
				_writer.WriteStartElement(wrapperName);
				Add(text);
				_writer.WriteEndElement();
			}
		}

		public void AddDeletedEntry(LexEntry entry)
		{
			_writer.WriteStartElement("entry");
			_writer.WriteAttributeString("id", MakeHumanReadableId(entry));
			_writer.WriteAttributeString("dateCreated", entry.CreationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("dateModified", entry.ModificationTime.ToString(LiftDateTimeFormat));
			_writer.WriteAttributeString("flex", "id", "http: //fieldworks.sil.org", entry.Guid.ToString());
			 _writer.WriteAttributeString("dateDeleted", DateTime.UtcNow.ToString(LiftDateTimeFormat));

			_writer.WriteEndElement();
		}
	}
}

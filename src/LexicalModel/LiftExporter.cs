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
			_writer.WriteAttributeString("flex", "id", "http://fieldworks.sil.org", entry.Guid.ToString());
			WriteForm(entry.LexicalForm);

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
			WriteFormInElement("gloss", sense.Gloss);
			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
				Add(example);
			}
			WriteCustomProperties(sense);
			_writer.WriteEndElement();
		}

		private void WriteGrammi(LexSense sense)
		{
			object pos;
			if (sense.Properties.TryGetValue("PartOfSpeech", out pos) && pos is OptionRef)
			{
				_writer.WriteStartElement("grammi");
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
					WriteFormInElement(pair.Key, pair.Value as MultiText);
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
			WriteFormInElement("source", example.Sentence);
			WriteFormInElement("trans", example.Translation);
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


		private void WriteForm(MultiText text)
		{
			if (text != null && text.Count > 0)
			{
				Add(text);
			}
		}

		private void WriteFormInElement(string name, MultiText text)
		{
			if (text != null && text.Count > 0)
			{
				_writer.WriteStartElement(name);
				Add(text);
				_writer.WriteEndElement();
			}
		}


	}
}

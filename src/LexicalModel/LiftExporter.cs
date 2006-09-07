using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LiftExporter
	{
		protected XmlWriter _writer;

		public LiftExporter(string path)
		{
			_writer = XmlWriter.Create(path, PrepareSettings(false));
		   Start();
		}

		/// <summary>
		/// for automated testing
		/// </summary>
		/// <param name="builder"></param>
		public LiftExporter(StringBuilder builder, bool produceFragmentOnly)
		{

			_writer = XmlWriter.Create(builder, PrepareSettings(produceFragmentOnly));
			if (!produceFragmentOnly)
			{
				Start();
			}
		}

		private XmlWriterSettings PrepareSettings(bool produceFragmentOnly)
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
		}

		public void End()
		{
			if (_writer.Settings.ConformanceLevel != ConformanceLevel.Fragment)
			{
				_writer.WriteEndElement();//lift
				_writer.WriteEndDocument();
			}
			_writer.Close();
		}

		public void Add(IList<LexEntry> entries)
		{
			foreach (LexEntry entry in entries)
			{
				Add(entry);
			}
		}

		public void Add(LexEntry entry)
		{
			_writer.WriteStartElement("entry");
			WriteForm(entry.LexicalForm);

			foreach(LexSense sense in entry.Senses)
			{
				Add(sense);
			}
		}

		public void Add(LexExampleSentence example)
		{
			_writer.WriteStartElement("example");
			WriteFormInElement("source", example.Sentence);
			WriteFormInElement("trans", example.Translation);
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

		public void Add(LexSense sense)
		{
			_writer.WriteStartElement("sense");
			WriteFormInElement("gloss", sense.Gloss);
			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
				Add(example);
			}
			_writer.WriteEndElement();
		}

		private void WriteForm(MultiText text)
		{
			if (text != null && text.Count > 0)
			{
				Add(text);
			}
			_writer.WriteEndElement();
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

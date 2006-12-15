using System;
using System.Collections.Generic;
using System.Xml;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public abstract class LiftImporter
	{
		private ProgressState _progressState = new NullProgressState();

		/// <summary>
		///
		/// </summary>
		public LiftImporter()
		{
		}

//        public LiftImporter()
//        {
//        }

		/// <summary>
		/// Optionally set this to a wiredup ProgressState to get UI feedback and user cancelling
		/// </summary>
		public ProgressState Progress
		{
			get
			{
				return _progressState;
			}
			set
			{
				_progressState = value;
			}
		}

		/// <summary>
		/// Pick the best importer based on the version info in the file
		/// </summary>
		 public static LiftImporter CreateCorrectImporter( XmlDocument doc)
		{
			string version = XmlUtils.GetAttributeValue(doc.SelectSingleNode("lift"), "producer", "");
			switch (version)
			{
				case "SIL.FLEx.V1Pt1":
					return new LiftImporterFlexVer1Pt1();
				case "WeSay.1Pt0Alpha":
					return new LiftImporterWeSay();
				default:
					return new LiftImporterWeSay();
			}
		}

		/// <summary>
		/// Picks the best importer and uses it.
		/// </summary>
		public static LiftImporter ReadFile(IList<LexEntry> entries, string path, ProgressState progressState)
		{
			XmlDocument doc =new XmlDocument();
			doc.Load(path);

			LiftImporter importer = CreateCorrectImporter(doc);
			if (progressState != null)
			{
				importer.Progress = progressState;
			}
			importer.ReadFile(doc, entries);
			return importer;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="entries">New items will be added to this list</param>
		public virtual void ReadFile(XmlDocument doc, IList<LexEntry> entries)
		{
			XmlNodeList entryNodes = doc.SelectNodes("./lift/entry");
			int count = 0;
			const int kInterval = 50;
			int nextProgressPoint = count + kInterval;
			_progressState.NumberOfSteps = entryNodes.Count;
			foreach (XmlNode node in entryNodes)
			{
				entries.Add(this.ReadEntry(node));
				count++;
				if (count >= nextProgressPoint)
				{
					_progressState.NumberOfStepsCompleted = count;
					nextProgressPoint = count + kInterval;
					if (_progressState.Cancel)
						break;
				}
			}
		}

		protected static string GetStringAttribute(XmlNode form, string attr)
		{
			return form.Attributes[attr].Value;
		}

		protected static string GetOptionalAttributeString(XmlNode xmlNode, string name)
		{
			XmlAttribute attr= xmlNode.Attributes[name];
			if (attr == null)
				return null;
			return attr.Value;
		}

		public void ReadMultiTextOrNull(XmlNode node, string query, MultiText text)
		{
			XmlNode element = node.SelectSingleNode(query);
			if (element != null)
			{
				ReadMultiText(element, text);
			}
		}

		/// <summary>
		/// this takes a text, rather than returning one just because the
		/// lexical model classes currently always create their MultiText fields during the constructor.
		/// </summary>
		public virtual void ReadMultiText(XmlNode node, MultiText text)
		{
			foreach (XmlNode form in node.SelectNodes("form"))
			{
				text.SetAlternative(GetStringAttribute(form, "lang"), form.InnerText);
			}
		}

		public LexExampleSentence ReadExample(XmlNode xmlNode)
		{
			LexExampleSentence example = new LexExampleSentence();
			ReadMultiTextOrNull(xmlNode, "source", example.Sentence);
			//NB: will only read in one translation
			ReadMultiTextOrNull(xmlNode, "trans", example.Translation);
			return example;
		}

		public LexSense ReadSense(XmlNode xmlNode)
		{
			LexSense sense = new LexSense();
			ReadMultiTextOrNull(xmlNode, "gloss", sense.Gloss);
			foreach (XmlNode n in xmlNode.SelectNodes("example"))
			{
				sense.ExampleSentences.Add(ReadExample(n));
			}
			return sense;
		}

		public LexEntry ReadEntry(XmlNode xmlNode)
		{
			LexEntry entry=null;
			string id = GetOptionalAttributeString(xmlNode, "id");
			if (id != null)
			{
				try
				{
					entry = new LexEntry(new Guid(id));
				}
				catch (FormatException e)
				{
					//enchance: log this, we're throwing away the id they had
				}
				catch (OverflowException e)
				{
					//enchance: log this, we're throwing away the id they had
				}
			}

			if(entry==null)
			{
				entry = new LexEntry();
			}
			ReadMultiText(xmlNode, entry.LexicalForm);

			foreach (XmlNode n in xmlNode.SelectNodes("sense"))
			{
				entry.Senses.Add(ReadSense(n));
			}

			return entry;
		}
	}

	public class ProgressEventArgs : EventArgs
	{
		private int _progress;
		private bool _cancel = false;
		public ProgressEventArgs(int progress)
		{
			_progress = progress;
		}

		public int Progress
		{
			get { return _progress; }
		}

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
}
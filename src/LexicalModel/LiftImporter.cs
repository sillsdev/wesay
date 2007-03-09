using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public abstract class LiftImporter
	{
		private ProgressState _progressState = new NullProgressState();
		private IList<String> _expectedOptionTraits;
		private IList<string> _expectedOptionCollectionTraits;
		private string _queryForCustomFields;

		/// <summary>
		///
		/// </summary>
		public LiftImporter()
		{
			_expectedOptionTraits = new List<string>();
			_expectedOptionCollectionTraits = new List<string>();
			MakeQueryForCustomFields();
		}

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

		public IList<string> ExpectedOptionTraits
		{
			get
			{
				return _expectedOptionTraits;
			}
//            set
//            {
//                _expectedOptionTraits = value;
//            }
		}

		public IList<string> ExpectedOptionCollectionTraits
		{
			get
			{
				return _expectedOptionCollectionTraits;
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
				case "SIL.FLEx.V1Pt2": // we'll try to get this version set to the correct number as the time approaches
					return new LiftImporterFlexVer1Pt2();
				 case "SIL.FLEx.V1Pt1": // this was actually released (without informing us in time) as 1.0.1
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
				entries.Add(ReadEntry(node));
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

		/// <summary>
		/// Get an obligatory attribute value.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The required attribute to find.</param>
		/// <returns>The value of the attribute.</returns>
		/// <exception cref="ApplicationException">
		/// Thrown when the value is not found in the node.
		/// </exception>
		public static string GetManditoryAttributeValue(XmlNode node, string attrName)
		{
			string retval = XmlUtils.GetOptionalAttributeValue(node, attrName, null);
			if (retval == null)
			{
				throw new ApplicationException("The attribute'"
					+ attrName
					+ "' is mandatory, but was missing. "
					+ node.OuterXml);
			}
			return retval;
		}



		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XmlNode node, string attrName, string defaultString)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[attrName];
				if (xa != null)
					return xa.Value;
			}
			return defaultString;
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
				text.SetAlternative(GetManditoryAttributeValue(form, "lang"), form.InnerText);
			}
		}

		public LexExampleSentence ReadExample(XmlNode xmlNode)
		{
			LexExampleSentence example = new LexExampleSentence();
			ReadTraits(example, xmlNode);
			ReadCustomFields(example, xmlNode);
			ReadMultiTextOrNull(xmlNode, "source", example.Sentence);
			//NB: will only read in one translation
			ReadMultiTextOrNull(xmlNode, "trans", example.Translation);
			return example;
		}

		public LexSense ReadSense(XmlNode node)
		{
			LexSense sense = new LexSense();
			ReadTraits(sense, node);
			ReadCustomFields(sense, node);
			ReadMultiTextOrNull(node, "gloss", sense.Gloss);

			ReadGrammi(sense, node);


			foreach (XmlNode n in node.SelectNodes("example"))
			{
				sense.ExampleSentences.Add(ReadExample(n));
			}
			return sense;
		}

		private void ReadTraits(WeSayDataObject lexObject, XmlNode node)
		{
			foreach (XmlNode traitNode in node.SelectNodes("trait"))
			{
				//nb: name is not in the Dec 2006 version of lift
				string name = GetOptionalAttributeValue(traitNode, "name",null);
				if (name != null && ExpectedOptionTraits.Contains(name))
				{
					OptionRef o = lexObject.GetOrCreateProperty<OptionRef>(name);
					o.Value = GetManditoryAttributeValue(traitNode, "value");
				}
				else if (name != null && ExpectedOptionCollectionTraits.Contains(name))
				{
					OptionRefCollection c = lexObject.GetOrCreateProperty<OptionRefCollection>(name);
					c.Keys.Add(GetManditoryAttributeValue(traitNode, "value"));
				}
			   else
				{
					//"log skipping..."
				}
			}
		}

		private void ReadCustomFields(WeSayDataObject lexObject, XmlNode node)
		{
			foreach (XmlNode customNode in node.SelectNodes(_queryForCustomFields))
			{
				MultiText mt = new MultiText();
				ReadMultiText(customNode, mt);
				lexObject.Properties.Add(new KeyValuePair<string, object>(customNode.Name, mt));
			}
		}

		private void MakeQueryForCustomFields()
		{
			string[] knownElementNames =
				new string[] {"trait", "lex", "sense", "subentry", "gloss", "grammi", "example", "subsense"};

			StringBuilder builder = new StringBuilder();
			builder.Append("*[not (");
			foreach (string s in knownElementNames)
			{
				builder.AppendFormat("self::{0} or ", s);
			}
			builder.Append("false)]");
			_queryForCustomFields = builder.ToString();
		}

		protected virtual void ReadGrammi(LexSense sense, XmlNode senseNode)
		{
			XmlNode grammi = senseNode.SelectSingleNode("grammi");
			if (grammi == null)
			{
				return;
			}
			OptionRef o = sense.GetOrCreateProperty<OptionRef>("PartOfSpeech");
			o.Value = GetManditoryAttributeValue(grammi, "value");
		}

		public LexEntry ReadEntry(XmlNode xmlNode)
		{
			LexEntry entry=null;
			string id = GetOptionalAttributeValue(xmlNode, "id", null);
			if (id != null)
			{
				try
				{
					entry = new LexEntry(new Guid(id));
				}
				catch (FormatException)
				{
					//enchance: log this, we're throwing away the id they had
				}
				catch (OverflowException)
				{
					//enchance: log this, we're throwing away the id they had
				}
			}

			if(entry==null)
			{
				entry = new LexEntry();
			}
			ReadTraits(entry, xmlNode);
			ReadCustomFields(entry, xmlNode);
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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace LiftIO
{
	public class LiftParser<TEntry, TSense, TExample>
		where TEntry : class
		where TSense: class
		where TExample : class
	{

		private ILexiconMerger<TEntry, TSense, TExample> _merger;
		protected string wsAttributeLabel = "lang";

		private bool _cancelNow = false;
		private string _defaultLangId="??";


		public LiftParser(ILexiconMerger<TEntry, TSense, TExample> merger)
		{
			_merger = merger;
		}


		/// <summary>
		///
		/// </summary>
		public virtual void ReadFile(XmlDocument doc)
		{
			XmlNodeList entryNodes = doc.SelectNodes("./lift/entry");
			int count = 0;
			const int kInterval = 50;
			int nextProgressPoint = count + kInterval;
			this.ProgressTotalSteps = entryNodes.Count;
			foreach (XmlNode node in entryNodes)
			{
				this.ReadEntry(node);
				count++;
				if (count >= nextProgressPoint)
				{
					this.ProgressStepsCompleted = count;
					nextProgressPoint = count + kInterval;
					if (_cancelNow)
						break;
				}
			}
		}

		public TEntry ReadEntry(XmlNode node)
		{
			TEntry entry = _merger.GetOrMakeEntry(GetIdInfo(node));
			if (entry != null)//not been pruned
			{
				_merger.MergeInLexemeForm(entry, ProcessMultiText(node, "lex"));
				foreach (XmlNode n in node.SelectNodes("sense"))
				{
					ReadSense(n, entry);
				}
			}
			return entry;
		}

		protected virtual IdentifyingInfo GetIdInfo(XmlNode node)
		{
			IdentifyingInfo info = new IdentifyingInfo();
			info.id= GetOptionalAttributeString(node, "id");
			info.creationTime = GetOptionalDate(node, "dateCreated");
			info.modificationTime = GetOptionalDate(node, "dateModified");
			return info;
		}

		public TSense ReadSense(XmlNode node, TEntry entry)
		{
			TSense sense = _merger.GetOrMergeSense(entry, GetIdInfo(node));
			if (sense != null)//not been pruned
			{
				_merger.MergeInGloss(sense, ProcessMultiText(node, "gloss"));

				foreach (XmlNode n in node.SelectNodes("example"))
				{
					ReadExample(n, sense);
				}
			}
			return sense;
		}

		private TExample ReadExample(XmlNode node, TSense sense)
		{
			TExample example = _merger.GetOrMergeExample(sense, GetIdInfo(node));
			if (example != null)//not been pruned
			{
				_merger.MergeInExampleForm(example, ProcessMultiText(node, null));
				//NB: only one translation supported in LIFT at the moment
				_merger.MergeInTranslationForm(example, ProcessMultiText(node, "translation"));
			}
			return example;
		}

		protected StringDictionary ProcessMultiText(XmlNode node, string fieldName)
		{
			return ReadMultiText(node, fieldName);
		}


		protected virtual StringDictionary ReadLexemeForm(XmlNode node)
		{
			//TODO in real lift it is wrapped!!!!!!
			return ReadMultiText(node);
		}


		protected static string GetStringAttribute(XmlNode form, string attr)
		{
			return form.Attributes[attr].Value;
		}

		protected static string GetOptionalAttributeString(XmlNode xmlNode, string name)
		{
			XmlAttribute attr = xmlNode.Attributes[name];
			if (attr == null)
				return null;
			return attr.Value;
		}


		/// <summary>
		/// careful, can't return null, so give MinValue
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected static DateTime GetOptionalDate(XmlNode xmlNode, string name)
		{
			XmlAttribute attr = xmlNode.Attributes[name];
			if (attr == null)
				return DateTime.MinValue;

			/* if the incoming data lacks a time, we'll have a kind of 'unspecified', else utc */
			return DateTime.Parse(attr.Value);
		}

		protected StringDictionary ReadMultiText(XmlNode node, string query)
		{
			XmlNode element=null;
			if (query == null)
			{
				element = node;
			}
			else
			{
				element = node.SelectSingleNode(query);
			}

			if (element != null)
			{
				return ReadMultiText(element);
			}
			return new StringDictionary();
		}

		/// <summary>
		/// this takes a text, rather than returning one just because the
		/// lexical model classes currently always create their MultiText fields during the constructor.
		/// </summary>
		protected  StringDictionary ReadMultiText(XmlNode node)
		{
			StringDictionary text = new StringDictionary();
			foreach (XmlNode form in node.SelectNodes("form"))
			{
				try
				{
					text[GetStringAttribute(form, wsAttributeLabel)] = form.InnerText;
				}
				catch (Exception e)
				{
					NotifyError(e);
				}
			}
			if (text.Count == 0)
			{
				if (node.InnerText != null && node.InnerText.Trim() != string.Empty)
				{
					text[_defaultLangId] = node.InnerText;
				}
				else
				{
					return null;
				}
			}

			return text;
		}

//        public LexExampleSentence ReadExample(XmlNode xmlNode)
//        {
//            LexExampleSentence example = new LexExampleSentence();
//            ReadMultiText(xmlNode, "source", example.Sentence);
//            //NB: will only read in one translation
//            ReadMultiText(xmlNode, "trans", example.Translation);
//            return example;
//        }
//



		#region Progress

		public class StepsArgs : EventArgs
		{
			public int _steps;
		}

		public class ErrorArgs : EventArgs
		{
			public Exception _exception;
		}

		private int ProgressStepsCompleted
		{
			set
			{
				if (SetStepsCompleted != null)
				{
					ProgressEventArgs e = new ProgressEventArgs(value);
					SetStepsCompleted.Invoke(this, e);
					_cancelNow = e.Cancel;
				}
			}
		}

		private int ProgressTotalSteps
		{
			set
			{
				if (SetTotalNumberSteps != null)
				{
					StepsArgs e = new StepsArgs();
					e._steps = value;
					SetTotalNumberSteps.Invoke(this, e);
				}
			}
		}

	   private void NotifyError(Exception error)
		{
			if (ParsingError != null)
			{
				ErrorArgs e = new ErrorArgs();
				e._exception = error;
				ParsingError.Invoke(this, e);
			}
		}
		public event EventHandler<ErrorArgs> ParsingError;
		 public event EventHandler<StepsArgs> SetTotalNumberSteps;
	   public event EventHandler<ProgressEventArgs> SetStepsCompleted;

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

#endregion

	}
}

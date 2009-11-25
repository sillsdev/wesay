using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Palaso.DictionaryServices.Lift;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Model;
using Palaso.Lift.Options;
using Palaso.Text;
using Palaso.UiBindings;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;

#if MONO
using Palaso.Linq;
#endif

namespace WeSay.Project
{
	public class PLiftExporter: LiftWriter
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly LexEntryRepository _lexEntryRepository;
		private readonly IList<string> _headwordWritingSystemIds;

		public PLiftExporter(StringBuilder builder,
							 bool produceFragmentOnly,
							 LexEntryRepository lexEntryRepository,
							 ViewTemplate viewTemplate): base(builder, produceFragmentOnly)
		{
			_lexEntryRepository = lexEntryRepository;
			_viewTemplate = viewTemplate;
			_headwordWritingSystemIds = _viewTemplate.GetHeadwordWritingSystemIds();
		}

		public PLiftExporter(string path,
							 LexEntryRepository lexEntryRepository,
							 ViewTemplate viewTemplate): base(path)
		{
			_lexEntryRepository = lexEntryRepository;
			_viewTemplate = viewTemplate;
			_headwordWritingSystemIds = _viewTemplate.GetHeadwordWritingSystemIds();
		}

		private Options _options = Options.DereferenceRelations | Options.DereferenceOptions |
								   Options.DetermineHeadword;


		[Flags]
		public enum Options
		{
			NormalLift = 0,
			DereferenceRelations = 1,
			DereferenceOptions = 2,
			DetermineHeadword = 4
		} ;

		/// <summary>
		/// Set this if you want the output filtered to the visible fields and the writing system orders respected
		/// </summary>
		public ViewTemplate Template
		{
			get { return _viewTemplate; }
		}

		public Options ExportOptions
		{
			get { return _options; }
			set { _options = value; }
		}

		public override void Add(LexEntry entry)
		{
			WritingSystem headWordWritingSystem = _viewTemplate.HeadwordWritingSystems[0];
			int h = _lexEntryRepository.GetHomographNumber(entry, headWordWritingSystem);
			Add(entry, h);
		}

		private void WriteDisplayNameFieldForOption(IValueHolder<string> optionRef, string fieldName)
		{
			OptionsList list = WeSayWordsProject.Project.GetOptionsList(fieldName);
			if (list != null)
			{
				Option posOption = list.GetOptionFromKey(optionRef.Value);
				if (posOption == null)
				{
					return;
				}
				if (posOption.Name == null)
				{
					return;
				}

				LanguageForm[] labelForms =
//                        posOption.Name.GetOrderedAndFilteredForms(
					   posOption.Abbreviation.GetOrderedAndFilteredForms(
								_viewTemplate.GetField(fieldName).WritingSystemIds);

				if (labelForms != null && labelForms.Length > 0)
				{
					Writer.WriteStartElement("field");
					Writer.WriteAttributeString("type",
												fieldName == "POS" ? "grammatical-info" : fieldName);
					Add(labelForms, false);
					Writer.WriteEndElement();
				}
			}
		}

		/// <summary>
		/// nb: this is used both for the headword of an article, but also for the target of a relation.
		/// </summary>
		private void WriteHeadWordField(LexEntry entry, string outputFieldName)
		{
			//                headword.SetAlternative(HeadWordWritingSystemId, entry.GetHeadWordForm(HeadWordWritingSystemId));

			var headword = new MultiText();
			foreach (string writingSystemId in _headwordWritingSystemIds)
			{
				headword.SetAlternative(writingSystemId, entry.GetHeadWordForm(writingSystemId));
			}
			WriteMultiTextAsArtificialField(outputFieldName, headword);
		}


		/// <summary>
		/// use this for multitexts that were somehow constructed during export, with no corresponding single property
		/// </summary>
		private void WriteMultiTextAsArtificialField(string outputFieldName, MultiTextBase text)
		{
			if (!MultiTextBase.IsEmpty(text))
			{
				Writer.WriteStartElement("field");

				Writer.WriteAttributeString("type", outputFieldName);

				if (!MultiTextBase.IsEmpty(text))
				{
					var textWritingSystems = _viewTemplate.WritingSystems.GetActualTextWritingSystems();
					var ids = from ws in textWritingSystems select ws.Id;
					Add(text.Forms.Where(f=>ids.Contains(f.WritingSystemId) ), true);
				}

				Writer.WriteEndElement();
			}
		}

		/// <summary>
		/// for plift, we take any audio paths found in the multitext and turn them into traits.
		/// </summary>
		protected override void WriteFormsThatNeedToBeTheirOwnFields(MultiText text, string name)
		{
			foreach(var path in GetAudioForms(text, _viewTemplate.WritingSystems))
			{
				Writer.WriteStartElement("trait");

			   //nb: <media> not allowed by 0.12 schema, so we're just using trait[name='audio' value='...']
				Writer.WriteAttributeString("name", "audio");
				Writer.WriteAttributeString("value", string.Format("..{0}audio{0}" + path.Form, System.IO.Path.DirectorySeparatorChar));
				Writer.WriteEndElement();
			}
		}

		public static IList<LanguageForm> GetAudioForms(MultiText field, WritingSystemCollection writingSytems)
		{
			var x = field.Forms.Where(f => writingSytems[f.WritingSystemId].IsAudio);
			return new List<LanguageForm>(x);
		}

		protected override void WriteRelationTarget(LexRelation relation)
		{
			if (0 == (ExportOptions & Options.DereferenceRelations))
			{
				return;
			}

			string key = relation.Key;
			if(string.IsNullOrEmpty(key))
			{
				return;
			}

			LexEntry target = _lexEntryRepository.GetLexEntryWithMatchingId(key);
			if (target != null)
			{
				WriteHeadWordField(target, "headword-of-target");
			}
		}
		protected override string GetOutputRelationName(LexRelation relation)
		{
			var s= relation.FieldId.Replace("confer", "cf");//hack. Other names are left as-is.
			s = s.Replace("BaseForm", "see");//hack... not sure what we want here
			return s;
		}

		protected override void WritePosCore(OptionRef pos)
		{
			if (0 != (_options & Options.DereferenceOptions))
			{
				WriteDisplayNameFieldForOption(pos, LexSense.WellKnownProperties.PartOfSpeech);
			}
			else
			{
				base.WritePosCore(pos);
			}
		}

		/// <summary>
		/// add a pronunciation if we have an audio writing system alternative on the lexical unit
		/// </summary>
		 protected override void InsertPronunciationIfNeeded(LexEntry entry, List<string> propertiesAlreadyOutput)
		{
//            if(!_viewTemplate.WritingSystems.Any(p=>p.Value.IsAudio))
//                return;
//
			var paths = GetAudioForms(entry.LexicalForm, _viewTemplate.WritingSystems);
			if (paths.Count == 0)
				return;
			Writer.WriteStartElement("pronunciation");

			paths.ForEach(path =>
							  {
								  Writer.WriteStartElement("media");
								  Writer.WriteAttributeString("href", string.Format("..{0}audio{0}"+path.Form, System.IO.Path.DirectorySeparatorChar));
								  Writer.WriteEndElement();
							  });

			Writer.WriteEndElement();
		}

		protected override void WriteHeadword(LexEntry entry)
		{
			if (0 != (_options & Options.DetermineHeadword))
			{
				WriteHeadWordField(entry, "headword");
			}
		}

		protected override void WriteOptionRef(string key, OptionRef optionRef)
		{
			if (optionRef.Value.Length > 0)
			{
				if (0 != (ExportOptions & Options.DereferenceOptions))
				{
					WriteDisplayNameFieldForOption(optionRef, key);
				}
				else
				{
					base.WriteOptionRef(key, optionRef);
				}
			}
		}

		protected override bool ShouldOutputProperty(string property)
		{
			Field f = Template.GetField(property);
			if (f == null)
			{
				return false;
			}
			return (f.Enabled);
		}

		protected override LanguageForm[] GetOrderedAndFilteredForms(MultiTextBase text,
																	 string propertyName)
		{
			Field f = Template.GetField(propertyName);
			if (f == null)
			{
				return text.Forms;
			}
			return text.GetOrderedAndFilteredForms(f.GetTextOnlyWritingSystemIds(_viewTemplate.WritingSystems));
		}
	}
}

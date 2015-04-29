using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SIL.Linq;
using Palaso.DictionaryServices.Lift;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Options;
using SIL.Text;
using SIL.UiBindings;
using SIL.WritingSystems;
using SIL.Extensions;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project
{
	public class PLiftExporter: LiftWriter
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly LexEntryRepository _lexEntryRepository;
		private readonly IEnumerable<string> _headwordWritingSystemIds;
		private string _path;

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
							 ViewTemplate viewTemplate)
			: base(path, LiftWriter.ByteOrderStyle.BOM)
		{
			_path = path;
			_disposed = true; // In case we throw in the constructor
			_lexEntryRepository = lexEntryRepository;
			_viewTemplate = viewTemplate;
			_headwordWritingSystemIds = new List<string>(_viewTemplate.GetHeadwordWritingSystemIds());
			_disposed = false;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public const Options DefaultOptions = Options.DereferenceRelations | Options.DereferenceOptions |
								   Options.DetermineHeadword;

		private Options _options = DefaultOptions;



		[Flags]
		public enum Options
		{
			NormalLift = 0,
			DereferenceRelations = 1,
			DereferenceOptions = 2,
			DetermineHeadword = 4,
			ExportPartOfSpeechAsGrammaticalInfoElement = 8 //this just means export it as normal lift, rather than making it look like a custom fied.
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
			WritingSystemDefinition headWordWritingSystem = _viewTemplate.HeadwordWritingSystems[0];
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
					WriteLanguageFormsInWrapper(labelForms, "form", false);
					Writer.WriteEndElement();
				}
			}
		}


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
					var textWritingSystems = _viewTemplate.WritingSystems.TextWritingSystems();
					var ids = from ws in textWritingSystems select ws.Id;
					WriteLanguageFormsInWrapper(text.Forms.Where(f => ids.Contains(f.WritingSystemId)), "form", true);
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

		public static IList<LanguageForm> GetAudioForms(MultiText field, IWritingSystemRepository writingSytems)
		{
			var x = field.Forms.Where(f => writingSytems.Get(f.WritingSystemId).IsVoice);
			return new List<LanguageForm>(x);
		}

		protected override bool EntryDoesExist(string id)
		{
			return null!= _lexEntryRepository.GetLexEntryWithMatchingId(id);
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
				WriteHeadWordFieldForRelation(target, "headword-of-target");
			}
		}
		private void WriteHeadWordFieldForRelation(LexEntry entry, string outputFieldName)
		{
			//                headword.SetAlternative(HeadWordWritingSystemId, entry.GetHeadWordForm(HeadWordWritingSystemId));

			var headword = new MultiText();
			foreach (string writingSystemId in _headwordWritingSystemIds)
			{
				var headWordForm = entry.GetHeadWordForm(writingSystemId);
				if(!string.IsNullOrEmpty(headWordForm))
				{
					headword.SetAlternative(writingSystemId, headWordForm);
					break;//we only want the first non-empty one
				}
			}
			WriteMultiTextAsArtificialField(outputFieldName, headword);
		}

		protected override string GetOutputRelationName(LexRelation relation)
		{
			//Enhance: add "printed-dictionary-label" to fielddefns, so that people have control over this from wesay config.
			var s= relation.FieldId.Replace("confer", "see");
			s = s.Replace("BaseForm", "from");
			return s;
		}

		protected override void WritePosCore(OptionRef pos)
		{
			if ((0 == (_options & Options.ExportPartOfSpeechAsGrammaticalInfoElement)) && (0 != (_options & Options.DereferenceOptions)))
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
			IList<LanguageForm> paths = GetAudioForms(entry.LexicalForm, _viewTemplate.WritingSystems);
			if (paths.Count == 0)
				return;
			Writer.WriteStartElement("pronunciation");

			SIL.Linq.Enumerable.ForEach(paths, path =>
							  {
								  Writer.WriteStartElement("media");
								  Writer.WriteAttributeString("href", string.Format("..{0}audio{0}"+path.Form, System.IO.Path.DirectorySeparatorChar));
								  Writer.WriteEndElement();
							  });
			/*
			paths.ForEach(path =>
							  {
								  Writer.WriteStartElement("media");
								  Writer.WriteAttributeString("href", string.Format("..{0}audio{0}"+path.Form, System.IO.Path.DirectorySeparatorChar));
								  Writer.WriteEndElement();
							  });
*/
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

		/// <summary>
		/// This is to help Lexique Pro (or anyone else reading it) get the right urls to pictures, when the lift is down in Export.
		/// </summary>
		/// <param name="pictureRef"></param>
		protected override void WriteIllustrationElement(PictureRef pictureRef)
		{
			//base does this:             WriteURLRef("illustration", pictureRef.Value, pictureRef.Caption);
			//base.WriteIllustrationElement(pictureRef);

			string url = pictureRef.Value;
			if (url == null) // Fixes WS-34480 PLIFT-exporting actions don't work after going in Browse & Edit if image is missing
				return;
			if (_path != null)  //it's null during some tests
			{
				var dirWeAreWritingTo = Path.GetDirectoryName(_path);

				string parentDir = Directory.GetParent(dirWeAreWritingTo).FullName;
				if (!File.Exists(Path.Combine(dirWeAreWritingTo, url))) //something needs fixing
				{
					string upOne = Path.Combine(parentDir, url);
					if (File.Exists(upOne))
					{
						url = "..".CombineForPath(url);
					}
					else
					{
						string addPicturesDir = parentDir.CombineForPath("pictures", url);
						if (File.Exists(addPicturesDir))
						{
							url = "..".CombineForPath("pictures", url);
						}
					}
				}
			}
			WriteURLRef("illustration", url, pictureRef.Caption);
		}

		protected override LanguageForm[] GetOrderedAndFilteredForms(MultiTextBase text,
																	 string propertyName)
		{
			Field f = Template.GetField(propertyName);
			if (f == null)
			{
				return text.Forms;
			}
			return text.GetOrderedAndFilteredForms(_viewTemplate.WritingSystems.FilterForTextIetfLanguageTags(f.WritingSystemIds));
		}
	}
}

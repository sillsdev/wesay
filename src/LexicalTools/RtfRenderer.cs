using SIL.DictionaryServices.Model;
using SIL.Lift;
using SIL.Lift.Options;
using SIL.Text;
using SIL.WritingSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public static class RtfRenderer
	{
		public static string HeadWordWritingSystemId;

		public static string ToRtf(LexEntry entry,
								   CurrentItemEventArgs currentItem,
								   LexEntryRepository lexEntryRepository)
		{
			if (lexEntryRepository == null)
			{
				throw new ArgumentNullException("lexEntryRepository");
			}
			if (entry == null)
			{
				return string.Empty;
			}

			var rtf = new StringBuilder();
			rtf.Append(@"{\rtf1\ansi\uc1\fs28 ");
			rtf.Append(MakeFontTable());
			RenderHeadword(entry, rtf, lexEntryRepository);

			int senseNumber = 1;
			foreach (LexSense sense in entry.Senses)
			{
				RenderSense(entry, sense, senseNumber, currentItem, rtf);

				++senseNumber;
			}
			Field glossField = WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(LexSense.WellKnownProperties.Gloss);
			if (glossField.IsMeaningField)
			{
				rtf.Append(RenderGhostedField(null,
				LexSense.WellKnownProperties.Gloss,
										  currentItem,
										  entry.Senses.Count + 1));
			}
			else
			{
				rtf.Append(RenderGhostedField(null,
				LexSense.WellKnownProperties.Definition,
										  currentItem,
										  entry.Senses.Count + 1));
			}

			rtf.Append(@"\par}");
			return Utf16ToRtfAnsi(rtf.ToString());
		}

		private static void RenderSense(LexEntry entry, LexSense sense, int senseNumber, CurrentItemEventArgs currentItem, StringBuilder rtf)
		{
			//rtf.Append(SwitchToWritingSystem(WritingSystems.AnalysisWritingSystemDefault.Id));
			Field glossField = WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(LexSense.WellKnownProperties.Gloss);
			if (entry.Senses.Count > 1 ||
				(currentItem != null &&
				 ((glossField.IsMeaningField && currentItem.PropertyName == LexSense.WellKnownProperties.Gloss) ||
					(!glossField.IsMeaningField && currentItem.PropertyName == LexSense.WellKnownProperties.Definition))
				 ))
			{
				rtf.Append(" " + senseNumber);
			}

			RenderPartOfSpeech(sense, currentItem, rtf);

			// Render the Gloss/Definition (meaning) field
			if (glossField.IsMeaningField)
			{
				rtf.Append(" " + RenderField(sense.Gloss, currentItem, 0, glossField));
			}
			else
			{
				Field dfnField = WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(LexSense.WellKnownProperties.Definition);
				rtf.Append(" " + RenderField(sense.Definition, currentItem, 0, dfnField));

			}

			RenderExampleSentences(currentItem, rtf, sense);

			rtf.Append(RenderGhostedField(sense, "Sentence", currentItem, null));
			rtf.Append(RenderGhostedField(sense, "Translation", currentItem, null));
		}

		private static void RenderExampleSentences(CurrentItemEventArgs currentItem, StringBuilder rtf, LexSense sense)
		{
			foreach (LexExampleSentence exampleSentence in sense.ExampleSentences)
			{
				rtf.Append(@" \i ");
				rtf.Append(RenderField(exampleSentence.Sentence, currentItem));
				rtf.Append(@"\i0 ");
				rtf.Append(RenderField(exampleSentence.Translation, currentItem));
			}
		}

		private static void RenderPartOfSpeech(LexSense sense, CurrentItemEventArgs currentItem, StringBuilder rtf)
		{
			OptionRef posRef = sense.GetProperty<OptionRef>(
				LexSense.WellKnownProperties.PartOfSpeech
			);
			if (posRef == null)
			{
				return;
			}

			OptionsList list = WeSayWordsProject.Project.GetOptionsList(
				LexSense.WellKnownProperties.PartOfSpeech
			);
			if (list == null)
			{
				return;
			}

			Option posOption = list.GetOptionFromKey(posRef.Value);
			if (posOption == null)
			{
				return;
			}

			Field posField = WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(
				LexSense.WellKnownProperties.PartOfSpeech
			);
			if (posField == null)
			{
				return;
			}
			rtf.Append(@" \i ");
			rtf.Append(RenderField(posOption.Name, currentItem, 0, posField));
			rtf.Append(@"\i0 ");
		}

		private static void RenderHeadword(LexEntry entry,
										   StringBuilder rtf,
										   LexEntryRepository lexEntryRepository)
		{
			rtf.Append(@"{\b ");
			LanguageForm headword = entry.GetHeadWord(HeadWordWritingSystemId);
			if (null != headword)
			{
				// rtf.Append(RenderField(headword, currentItem, 2, null));

				rtf.Append(SwitchToWritingSystem(headword.WritingSystemId, 2));
				rtf.Append(headword.Form);
				//   rtf.Append(" ");

				int homographNumber = lexEntryRepository.GetHomographNumber(
					entry,
					WeSayWordsProject.Project.DefaultViewTemplate.HeadwordWritingSystems[0]
				);
				if (homographNumber > 0)
				{
					rtf.Append(@"{\sub " + homographNumber + "}");
				}
			}
			else
			{
				rtf.Append("??? ");
			}
			rtf.Append("}");
		}

		private static string MakeFontTable()
		{
			var rtf = new StringBuilder(@"{\fonttbl");
			int i = 0;
			foreach (var ws in WritingSystems.AllWritingSystems)
			{
				rtf.Append(@"\f" + i + @"\fnil\fcharset0" + " " + WritingSystemInfo.CreateFont(ws).FontFamily.Name +
						   ";");
				i++;
			}
			rtf.Append("}");
			return rtf.ToString();
		}

		private static IWritingSystemRepository WritingSystems
		{
			get { return BasilProject.Project.WritingSystems; }
		}

		private static int GetFontNumber(WritingSystemDefinition writingSystem)
		{
			int i = 0;
			foreach (var ws in WritingSystems.AllWritingSystems)
			{
				if (ws == writingSystem)
				{
					break;
				}
				i++;
			}
			return i;
		}

		private static string RenderField(MultiText text, CurrentItemEventArgs currentItem)
		{
			return RenderField(text, currentItem, 0, null);
		}

		private static string RenderField(MultiText text,
										  CurrentItemEventArgs currentItem,
										  int sizeBoost,
										  Field field)
		{
			var rtfBuilder = new StringBuilder();
			if (text != null)
			{
				if (text.Count == 0 && currentItem != null && text == currentItem.DataTarget)
				{
					rtfBuilder.Append(RenderBlankPosition());
				}

				if (field == null) // show them all
				{
					foreach (string id in WritingSystems.FilterForTextLanguageTags(text.Forms.Select(f => f.WritingSystemId)))
					{

						var form = text.Forms.First(f => f.WritingSystemId == id);
						RenderForm(text, currentItem, rtfBuilder, form, sizeBoost);
					}
				}
				else // show all forms turned on in the field
				{
					foreach (string id in field.WritingSystemIds.Intersect(text.Forms.Select(f => f.WritingSystemId)))
					{
						var form = text.Forms.First(f => f.WritingSystemId == id);
						RenderForm(text, currentItem, rtfBuilder, form, sizeBoost);
					}
				}
			}
			return rtfBuilder.ToString();
		}

		//public static IList<LanguageForm> GetActualTextForms(MultiText text, IWritingSystemRepository writingSytems)
		//{
		//    var x = text.Forms.Where(f => !writingSytems.Get(f.WritingSystemId).IsVoice);
		//    return new List<LanguageForm>(x);
		//}

		private static void RenderForm(MultiText text,
									   CurrentItemEventArgs currentItem,
									   StringBuilder rtfBuilder,
									   LanguageForm form,
									   int sizeBoost)
		{
			if (IsCurrentField(text, form, currentItem))
			{
				rtfBuilder.Append(@"\ul");
			}
			rtfBuilder.Append(SwitchToWritingSystem(form.WritingSystemId, sizeBoost));
			rtfBuilder.Append(form.Form); // + " ");
			if (IsCurrentField(text, form, currentItem))
			{
				//rtfBuilder.Append(" ");
				//rtfBuilder.Append(Convert.ToChar(160));
				rtfBuilder.Append(@"\ulnone ");
				//rtfBuilder.Append(Convert.ToChar(160));
			}
			rtfBuilder.Append(" ");
		}

		private static string RenderGhostedField(PalasoDataObject parent,
												string property,
												 CurrentItemEventArgs currentItem,
												 int? number)
		{
			string rtf = string.Empty;
			if (currentItem != null && property == currentItem.PropertyName && parent == currentItem.Parent)
			{
				//REVIEW: is a ws switch needed for a blank? rtf += SwitchToWritingSystem(WritingSystems.AnalysisWritingSystemDefault.Id);
				if (number != null)
				{
					rtf += number.ToString();
				}
				rtf += RenderBlankPosition();
			}
			return rtf;
		}

		private static string RenderBlankPosition()
		{
			return @" \ul        " + Convert.ToChar(160) + @"\ulnone  ";
		}

		private static string SwitchToWritingSystem(string writingSystemId, int sizeBoost)
		{
			if (!WritingSystems.Contains(writingSystemId))
			{
				return "";
				//that ws isn't actually part of our configuration, so can't get a special font for it
			}
			WritingSystemDefinition writingSystem = (WritingSystemDefinition)WritingSystems.Get(writingSystemId);
			string rtf = @"\f" + GetFontNumber(writingSystem);
			int fontSize = Convert.ToInt16((sizeBoost + WritingSystemInfo.CreateFont(writingSystem).SizeInPoints) * 2);
			rtf += @"\fs" + fontSize + " ";
			return rtf;
		}

		private static bool IsCurrentField(MultiText text,
										   LanguageForm l,
										   CurrentItemEventArgs currentItem)
		{
			if (currentItem == null)
			{
				return false;
			}
			return (currentItem.DataTarget == text &&
					currentItem.WritingSystemId == l.WritingSystemId);
		}

		private static string Utf16ToRtfAnsi(IEnumerable<char> inString)
		{
			var outString = new StringBuilder();
			foreach (char c in inString)
			{
				if (c > 128)
				{
					outString.Append(String.Format(@"\u{0:D}?", Convert.ToUInt16(c)));
					//outString.Append(Convert.ToUInt16(c).ToString());
				}
				else
				{
					outString.Append(c);
				}
			}
			return outString.ToString();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palaso.Text;
using Palaso.LexicalModel;
using Palaso.LexicalModel.Options;
using WeSay.LexicalModel;
using Palaso.LexicalModel;
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

			StringBuilder rtf = new StringBuilder();
			rtf.Append(@"{\rtf1\ansi\uc1\fs28 ");
			rtf.Append(MakeFontTable());
			RenderHeadword(entry, rtf, lexEntryRepository);

			int senseNumber = 1;
			foreach (LexSense sense in entry.Senses)
			{
				RenderSense(entry, sense, senseNumber, currentItem, rtf);

				++senseNumber;
			}
#if GlossMeaning
			rtf.Append(RenderGhostedField("Gloss", currentItem, entry.Senses.Count + 1));
#else
			rtf.Append(RenderGhostedField(null,
				LexSense.WellKnownProperties.Definition,
										  currentItem,
										  entry.Senses.Count + 1));
#endif

			rtf.Append(@"\par}");
			return Utf16ToRtfAnsi(rtf.ToString());
		}

		private static void RenderSense(LexEntry entry, LexSense sense, int senseNumber, CurrentItemEventArgs currentItem, StringBuilder rtf)
		{
//rtf.Append(SwitchToWritingSystem(WritingSystems.AnalysisWritingSystemDefault.Id));
#if GlossMeaning
				if (entry.Senses.Count > 1 || (currentItem != null && currentItem.PropertyName == "Gloss"))
#else
			if (entry.Senses.Count > 1 ||
				(currentItem != null &&
				 currentItem.PropertyName == LexSense.WellKnownProperties.Definition))
#endif
			{
				rtf.Append(" " + senseNumber);
			}

			RenderPartOfSpeech(sense, currentItem, rtf);
#if GlossMeaning
				rtf.Append(" " + RenderField(sense.Gloss, currentItem));
#else
			rtf.Append(" " + RenderField(sense.Definition, currentItem));
#endif
			//                rtf.Append(@"\i0 ");

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
			OptionRef posRef =
				sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			if (posRef != null)
			{
				OptionsList list =
					WeSayWordsProject.Project.GetOptionsList(
						LexSense.WellKnownProperties.PartOfSpeech);
				if (list != null)
				{
					Option posOption = list.GetOptionFromKey(posRef.Value);

					if (posOption != null)
					{
						Field posField =
							WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(
								LexSense.WellKnownProperties.PartOfSpeech);
						if (posField != null)
						{
							rtf.Append(@" \i ");
							rtf.Append(RenderField(posOption.Name, currentItem, 0, posField));
							rtf.Append(@"\i0 ");
						}
					}
				}
			}
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

				int homographNumber = lexEntryRepository.GetHomographNumber(entry,
																			WeSayWordsProject.
																					Project.
																					DefaultViewTemplate
																					.
																					HeadwordWritingSystem);
				if (homographNumber > 0)
				{
					rtf.Append(@"{\super " + homographNumber + "}");
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
			StringBuilder rtf = new StringBuilder(@"{\fonttbl");
			int i = 0;
			foreach (KeyValuePair<string, WritingSystem> ws in WritingSystems)
			{
				rtf.Append(@"\f" + i + @"\fnil\fcharset0" + " " + ws.Value.Font.FontFamily.Name +
						   ";");
				i++;
			}
			rtf.Append("}");
			return rtf.ToString();
		}

		private static WritingSystemCollection WritingSystems
		{
			get { return BasilProject.Project.WritingSystems; }
		}

		private static int GetFontNumber(WritingSystem writingSystem)
		{
			int i = 0;
			foreach (KeyValuePair<string, WritingSystem> ws in WritingSystems)
			{
				if (ws.Value == writingSystem)
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
			StringBuilder rtfBuilder = new StringBuilder();
			if (text != null)
			{
				if (text.Count == 0 && currentItem != null && text == currentItem.DataTarget)
				{
					rtfBuilder.Append(RenderBlankPosition());
				}

				if (field == null) // show them all
				{
					foreach (LanguageForm l in GetActualTextForms(text, WritingSystems))
					{
						RenderForm(text, currentItem, rtfBuilder, l, sizeBoost);
					}
				}
				else //todo: show all those turned on for the field?
				{
					LanguageForm form = text.GetBestAlternative(field.WritingSystemIds);
					if (form != null)
					{
						RenderForm(text, currentItem, rtfBuilder, form, sizeBoost);
					}
				}
			}
			return rtfBuilder.ToString();
		}

		public static IList<LanguageForm> GetActualTextForms(MultiText text, WritingSystemCollection writingSytems)
		{
			var x = text.Forms.Where((f) => !writingSytems[f.WritingSystemId].IsAudio);
			return new List<LanguageForm>(x);
		}

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
			if (currentItem != null && property == currentItem.PropertyName && parent==currentItem.Parent)
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
			WritingSystem writingSystem;
			if (!WritingSystems.TryGetValue(writingSystemId, out writingSystem))
			{
				return "";
				//that ws isn't actually part of our configuration, so can't get a special font for it
			}
			string rtf = @"\f" + GetFontNumber(writingSystem);
			rtf += @"\fs" + (sizeBoost + writingSystem.Font.SizeInPoints) * 2 + " ";
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
			StringBuilder outString = new StringBuilder();
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
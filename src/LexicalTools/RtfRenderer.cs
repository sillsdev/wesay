using System;
using System.Collections.Generic;
using System.Text;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.Language;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public static class RtfRenderer
	{
		public static string ToRtf(LexEntry entry, CurrentItemEventArgs currentItem)
		{
			if(entry == null)
			{
				return string.Empty;
			}

			StringBuilder rtf = new StringBuilder();
			rtf.Append(@"{\rtf1\ansi\fs28 ");
			rtf.Append(MakeFontTable());

			rtf.Append(@"\b ");
			rtf.Append(RenderField(entry.LexicalForm, currentItem, 2, null));
			rtf.Append(@"\b0");

			int senseNumber = 1;
			foreach (LexSense sense in entry.Senses)
			{
				//rtf.Append(SwitchToWritingSystem(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id));
				if (entry.Senses.Count > 1 || (currentItem != null && currentItem.PropertyName == "Gloss"))
				{
					rtf.Append(" " + senseNumber.ToString());
				}

				OptionRef posRef = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech.ToString());
				if (posRef != null)
				{
					OptionsList list =
						WeSayWordsProject.Project.GetOptionsList(LexSense.WellKnownProperties.PartOfSpeech);
					if (list != null)
					{
						Option posOption = list.GetOptionFromKey(posRef.Value);

						if (posOption != null)
						{
							Field posField = WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(
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
				rtf.Append(" " + RenderField(sense.Gloss, currentItem));
//                rtf.Append(@"\i0 ");

				foreach (LexExampleSentence exampleSentence in sense.ExampleSentences)
				{
					rtf.Append(@" \i ");
					rtf.Append(RenderField(exampleSentence.Sentence, currentItem));
					rtf.Append(@"\i0 ");
					rtf.Append(RenderField(exampleSentence.Translation, currentItem));
				}

				rtf.Append(RenderGhostedField("Sentence", currentItem, null));
				rtf.Append(RenderGhostedField("Translation", currentItem, null));

				++senseNumber;
			}
			rtf.Append(RenderGhostedField("Gloss", currentItem, entry.Senses.Count + 1));

			rtf.Append(@"\par}");
			return Utf16ToRtfAnsi(rtf.ToString());
		}

		private static string MakeFontTable()
		{
			StringBuilder rtf = new StringBuilder(@"{\fonttbl");
			int i = 0;
			foreach (KeyValuePair<string, WritingSystem> ws in BasilProject.Project.WritingSystems)
			{
				rtf.Append(@"\f" + i.ToString() + @"\fnil\fcharset0" + " " +ws.Value.Font.FontFamily.Name + ";");
				i++;
			}
			rtf.Append("}");
			return rtf.ToString();
		}

		private static int GetFontNumber(WritingSystem writingSystem){
			int i = 0;
			foreach (KeyValuePair<string, WritingSystem> ws in BasilProject.Project.WritingSystems)
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

		private static string RenderField(MultiText text, CurrentItemEventArgs currentItem, int sizeBoost, Field field)
		{
			StringBuilder rtfBuilder = new StringBuilder();
			if (text != null)
			{
				if (text.Count == 0 && currentItem != null && text == currentItem.DataTarget)
				{
					rtfBuilder.Append(RenderBlankPosition());
				}

				if (field == null)// show them all
				{
					foreach (LanguageForm l in text)
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

		private static void RenderForm(MultiText text, CurrentItemEventArgs currentItem, StringBuilder rtfBuilder, LanguageForm form, int sizeBoost)
		{
			if (IsCurrentField(text, form, currentItem))
			{
				rtfBuilder.Append(@"\ul");
			}
			rtfBuilder.Append(SwitchToWritingSystem(form.WritingSystemId, sizeBoost));
			rtfBuilder.Append(form.Form);// + " ");
			if (IsCurrentField(text, form, currentItem))
			{
				//rtfBuilder.Append(" ");
				//rtfBuilder.Append(Convert.ToChar(160));
				rtfBuilder.Append(@"\ulnone ");
				//rtfBuilder.Append(Convert.ToChar(160));
			}
			rtfBuilder.Append(" ");
		}

		private static string RenderGhostedField(string property, CurrentItemEventArgs currentItem, Nullable<int> number)
		{
			string rtf = string.Empty;
			if (currentItem != null && property == currentItem.PropertyName)
			{
				//REVIEW: is a ws switch needed for a blank? rtf += SwitchToWritingSystem(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id);
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
			if (!BasilProject.Project.WritingSystems.TryGetValue(writingSystemId, out writingSystem))
			{
				return "";//that ws isn't actually part of our configuration, so can't get a special font for it
			}
			string rtf = @"\f" + GetFontNumber(writingSystem);
			rtf += @"\fs" + (sizeBoost+ writingSystem.Font.SizeInPoints) * 2 + " ";
			return rtf;
		}

		private static bool IsCurrentField(MultiText text, LanguageForm l, CurrentItemEventArgs currentItem)
		{
			if (currentItem == null){
				return false;
			}
			return (currentItem.DataTarget == text && currentItem.WritingSystemId == l.WritingSystemId);
		}


		private static string Utf16ToRtfAnsi(string inString)
		{
			StringBuilder outString = new StringBuilder();
			foreach (char c in inString)
			{
				if (c > 128)
				{
				  outString.Append(@"\u");
				  outString.Append(Convert.ToUInt16(c).ToString());
				  outString.Append('?');
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

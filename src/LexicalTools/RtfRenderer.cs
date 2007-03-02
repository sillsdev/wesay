using System;
using System.Collections.Generic;
using System.Text;
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
			rtf.Append(RenderField(entry.LexicalForm, currentItem));
			rtf.Append(@"\b0");

			int senseNumber = 1;
			foreach (LexSense sense in entry.Senses)
			{
				//rtf.Append(SwitchToWritingSystem(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id));
				if (entry.Senses.Count > 1 || (currentItem != null && currentItem.PropertyName == "Gloss"))
				{
					rtf.Append(senseNumber.ToString());
				}
				rtf.Append(@" \i ");
				rtf.Append(RenderField(sense.Gloss, currentItem));
				rtf.Append(@"\i0 ");

				foreach (LexExampleSentence exampleSentence in sense.ExampleSentences)
				{
					rtf.Append(RenderField(exampleSentence.Sentence, currentItem));
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
			StringBuilder rtf = new StringBuilder();
			if (text != null)
			{
				if (text.Count == 0 && currentItem != null && text == currentItem.DataTarget)
				{
					rtf.Append(RenderBlankPosition());
				}

				foreach (LanguageForm l in text)
				{
					if (IsCurrentField(text, l, currentItem))
					{
						rtf.Append(@"\ul");
					}
					rtf.Append(SwitchToWritingSystem(l.WritingSystemId));
					rtf.Append(l.Form);// + " ");
					if (IsCurrentField(text, l, currentItem))
					{
					  //rtf.Append(" ");
						//rtf.Append(Convert.ToChar(160));
						rtf.Append(@"\ulnone ");
					  //rtf.Append(Convert.ToChar(160));
				  }
				  rtf.Append(" ");
				}
			}
			return rtf.ToString();
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

		private static string SwitchToWritingSystem(string writingSystemId)
		{
			WritingSystem writingSystem;
			if (!BasilProject.Project.WritingSystems.TryGetValue(writingSystemId, out writingSystem))
			{
				return "";//that ws isn't actually part of our configuration, so can't get a special font for it
			}
			string rtf = @"\f" + GetFontNumber(writingSystem);
			rtf += @"\fs" + writingSystem.Font.SizeInPoints * 2 + " ";
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
				  outString.Append(Convert.ToInt16(c).ToString());
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

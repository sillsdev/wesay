using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.LexicalModel;
using WeSay.Language;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class RtfRenderer
	{
		public static string ToRtf(LexEntry entry, CurrentItemEventArgs currentItem)
		{

			string rtf = @"{\rtf1\ansi\fs28 ";
			rtf += MakeFontTable();

			rtf += @"\b ";
			rtf += RenderField(entry.LexicalForm, currentItem);
			rtf += @"\b0 ";

			int senseNumber = 1;
			foreach (LexSense sense in entry.Senses)
			{
				rtf += SwitchToWritingSystem(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id);
				rtf += senseNumber.ToString();

				rtf += @" \i ";
				rtf += RenderField(sense.Gloss, currentItem);
				rtf += @"\i0 ";

				foreach (LexExampleSentence exampleSentence in sense.ExampleSentences)
				{
					rtf += RenderField(exampleSentence.Sentence, currentItem);
					rtf += RenderField(exampleSentence.Translation, currentItem);
				}

				rtf += RenderGhostedField("Sentence", currentItem, sense.ExampleSentences.Count + 1);
				rtf += RenderGhostedField("Translation", currentItem, sense.ExampleSentences.Count + 1);

				++senseNumber;
			}
			rtf += RenderGhostedField("Gloss", currentItem, entry.Senses.Count + 1);

			rtf += @"\par}";
			return Utf16ToRtfAnsi(rtf);
		}

		private static string MakeFontTable()
		{
			string rtf = @"{\fonttbl";
			int i = 0;
			foreach (KeyValuePair<string, WritingSystem> ws in BasilProject.Project.WritingSystems)
			{
				rtf += @"\f" + i.ToString() + @"\fnil\fcharset0" + " " +ws.Value.Font.FontFamily.Name + ";";
				i++;
			}
			rtf+="}";
			return rtf;
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
			string rtf = string.Empty;
			if (text != null)
			{
				if (text.Count == 0 && currentItem != null && text == currentItem.DataTarget)
				{
					rtf += RenderBlankPosition();
				}

				foreach (LanguageForm l in text)
				{
					if (IsCurrentField(text, l, currentItem))
					{
						rtf += @"\ul     ";
					}
					rtf += SwitchToWritingSystem(l.WritingSystemId);
					rtf += l.Form + " ";
					if (IsCurrentField(text, l, currentItem))
					{
						rtf += "   " + Convert.ToChar(160) + @"\ulnone  ";
					}
				}
			}
			return rtf;
		}

		private static string RenderGhostedField(string property, CurrentItemEventArgs currentItem, int number)
		{
			string rtf = string.Empty;
			if (currentItem != null && property == currentItem.PropertyName)
			{
				rtf += SwitchToWritingSystem(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id);
				rtf += number.ToString() + RenderBlankPosition();
			}
			return rtf;
		}

		private static string RenderBlankPosition()
		{
			return @" \ul        " + Convert.ToChar(160) + @"\ulnone  ";
		}

		private static string SwitchToWritingSystem(string WritingSystemId)
		{
			WritingSystem writingSystem = BasilProject.Project.WritingSystems[WritingSystemId];
			string rtf = @"\f" + GetFontNumber(writingSystem);
			rtf += @"\fs" + writingSystem.Font.SizeInPoints * 2 + " ";
			return rtf;
		}

		private static bool IsCurrentField(MultiText text, LanguageForm l, CurrentItemEventArgs currentItem)
		{
			if (currentItem == null){
				return false;
			}
			return (currentItem.DataTarget == text && currentItem.WritingSystem.Id == l.WritingSystemId);
		}


		private static string Utf16ToRtfAnsi(string inString)
		{
			string outString = String.Empty;
			foreach (char c in inString)
			{
				if (c > 128)
				{
					outString += @"\u" + Convert.ToInt16(c).ToString() + "?";
				}
				else
				{
					outString += c;
				}
			}
			return outString;
		}
	}
}

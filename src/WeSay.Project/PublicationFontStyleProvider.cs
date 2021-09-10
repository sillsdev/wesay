using SIL.DictionaryServices.Model;
using SIL.WritingSystems;
using System.Text;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	/// <summary>
	/// A REALLY poor man's style sheet improver, here.  The nice thing is that this can
	/// be enhanced someday, without effecting the rest of the code.
	/// </summary>
	public class PublicationFontStyleProvider
	{
		private readonly ViewTemplate _template;

		public PublicationFontStyleProvider(ViewTemplate template)
		{
			_template = template;
		}

		private Field FindFieldWithFieldName(string name)
		{
			return _template.Fields.Find(f => f.FieldName == name);
		}

		public string GetAutoFontsCascadingStyleSheetLinesForWritingSystem(WritingSystemDefinition ws)
		{
			var builder = new StringBuilder();
			//            var family = FontFamily.Families.FirstOrDefault(f => f.Name == ws.FontName);

			builder.AppendLine("font-family: '" + ws.DefaultFont.Name + "';");

			var word = FindFieldWithFieldName(LexEntry.WellKnownProperties.LexicalUnit);

			//make the first vernacular field bold
			if (word.WritingSystemIds.Count > 0 && word.WritingSystemIds[0] == ws.Id)
			{
				builder.AppendLine("font-weight: bold");
			}
			else
			{
				//if there are two meaning field writing systems, make the second one italic
				Field meaningField = FindFieldWithFieldName(LexSense.WellKnownProperties.Gloss);

				if ((meaningField == null) || (!meaningField.IsMeaningField))
				{
					meaningField = FindFieldWithFieldName(LexSense.WellKnownProperties.Definition);
				}

				if (meaningField.WritingSystemIds.Count > 1 && meaningField.WritingSystemIds[1] == ws.Id)
				{
					//                    if(family != default(FontFamily))
					//                    {
					//                        family.IsStyleAvailable(FontStyle.Italic)
					//                    }

					builder.AppendLine("font-style: italic");
				}

			}
			return builder.ToString();

		}
	}
}
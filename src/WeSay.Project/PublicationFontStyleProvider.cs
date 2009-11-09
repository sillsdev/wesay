using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WeSay.LexicalModel;
using System.Linq;
using WeSay.LexicalModel.Foundation;

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
			return _template.Fields.Find(delegate(Field f) { return f.FieldName == name; });
		}

		public string GetAutoFontsCascadingStyleSheetLinesForWritingSystem(WritingSystem ws)
		{
			var builder = new StringBuilder();
//            var family = FontFamily.Families.FirstOrDefault(f => f.Name == ws.FontName);

			builder.AppendLine("font-family: '" + ws.FontName + "';");

			var word = FindFieldWithFieldName(LexEntry.WellKnownProperties.LexicalUnit);

			//make the first vernacular field bold
			if(word.WritingSystemIds.Count > 0 && word.WritingSystemIds[0] == ws.Id)
			{
					builder.AppendLine("font-weight: bold");
			}
			else
			{
				//if there are two definition writing systems, make the second one italic

				var defField = FindFieldWithFieldName(LexSense.WellKnownProperties.Definition);
				if (defField.WritingSystemIds.Count > 1 && defField.WritingSystemIds[1] == ws.Id)
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
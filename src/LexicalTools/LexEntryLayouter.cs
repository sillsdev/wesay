using Gtk;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class LexEntryLayouter : Layouter
	{
		public LexEntryLayouter(TableBuilder builder)
		  : base(builder)
		{
		}

		public override int AddWidgets(object dataObject)
		{
			int rowCount = 1;
			LexEntry entry = (LexEntry)dataObject;

			_builder.AddWidgetRow("word: ", MakeBoundEntry(entry.LexicalForm, "th"));
			LexSenseLayouter layouter = new LexSenseLayouter(_builder);
			foreach (LexSense sense in entry.Senses)
			{
				rowCount+= layouter.AddWidgets(sense);
			}
			return rowCount;
		}
	}
}
using WeSay.LexicalModel;
using WeSay.UI;
namespace WeSay.LexicalTools
{
	public class LexExampleSentenceLayouter : Layouter
	{
		public LexExampleSentenceLayouter(DetailViewManager builder)
			: base(builder)
		{
		}

		public override int AddWidgets(object dataObject)
		{
			LexExampleSentence example = (LexExampleSentence)dataObject;

			_builder.AddWidgetRow("example: ", MakeBoundEntry(example.Sentence, "th"));
			_builder.AddWidgetRow("translation: ", MakeBoundEntry(example.Translation, "en"));
			return 2;
		}
	}
}

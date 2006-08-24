using WeSay.LexicalModel;
using WeSay.UI;
namespace WeSay.LexicalTools
{
	public class LexExampleSentenceLayouter : Layouter
	{
		public LexExampleSentenceLayouter(DetailList  builder)
			: base(builder)
		{
		}

		public override int AddWidgets(object dataObject)
		{
			LexExampleSentence example = (LexExampleSentence)dataObject;

			_builder.AddWidgetRow("Example", false, MakeBoundEntry(example.Sentence, "th"));
			_builder.AddWidgetRow("Translation", false, MakeBoundEntry(example.Translation, "en"));
			return 2;
		}
	}
}

using System;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	sealed public class LexExampleSentence : WeSayDataObject
	{
		private MultiText _sentence;
		private MultiText _translation;

		public LexExampleSentence()
		{
			_sentence = new MultiText();
			_translation = new MultiText();

			WireUpEvents();
		}

		protected override void WireUpEvents()
		{
			base.WireUpEvents();
			WireUpChild(_sentence);
			WireUpChild(_translation);
		}

		public MultiText Sentence
		{
			get { return _sentence; }
		}

		public MultiText Translation
		{
			get { return _translation; }
		}

		public override bool Empty
		{
			get
			{
				return Sentence.Empty && Translation.Empty;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexExampleSentence
	{
		private MultiText _sentence;
		private MultiText _translation;

		public LexExampleSentence()
		{
		}

		public MultiText Sentence
		{
			get { return _sentence; }
			set { _sentence = value; }
		}

		public MultiText Translation
		{
			get { return _translation; }
			set { _translation = value; }
		}
	}
}

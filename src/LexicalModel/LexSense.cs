using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexSense : WeSayDataObject
	{
		private MultiText _gloss;
		private WeSay.Data.InMemoryRecordList<LexExampleSentence> _exampleSentences;

		 public LexSense()
		{
			_gloss = new MultiText();
			_exampleSentences = new WeSay.Data.InMemoryRecordList<LexExampleSentence>();
			WireUpEvents();
		}

		protected override void WireUpEvents()
		{
			base.WireUpEvents();
			WireUpChild(_gloss);
			WireUpList(_exampleSentences, "exampleSentences");
		}

		public MultiText Gloss
		{
			get { return _gloss; }
		}

		public IBindingList ExampleSentences
		{
			get { return _exampleSentences; }
		}
	}
}

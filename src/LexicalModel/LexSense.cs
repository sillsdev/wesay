using System.ComponentModel;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	sealed public class LexSense : WeSayDataObject
	{
		private MultiText _gloss;
		private WeSay.Data.InMemoryBindingList<LexExampleSentence> _exampleSentences;

		 public LexSense()
		{
			_gloss = new MultiText();
			_exampleSentences = new WeSay.Data.InMemoryBindingList<LexExampleSentence>();
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

		public override bool Empty
		{
			get {
				return Gloss.Empty && ExampleSentences.Count == 0;
			}
		}

		public void RemoveEmptyExampleSentences() {
			// remove any example sentences that are empty
			int count = this._exampleSentences.Count;

			for (int i = count - 1; i >= 0 ; i--)
			{
				if(this._exampleSentences[i].Empty)
				{
					this._exampleSentences.RemoveAt(i);
				}
			}
			if(count != this._exampleSentences.Count)
			{
				OnEmptyObjectsRemoved();
			}
		}
		public override void SomethingWasModified(string PropertyModified)
		{
			if (PropertyModified != "exampleSentences")
			{
				RemoveEmptyExampleSentences();
			}
			base.SomethingWasModified(PropertyModified);
		}
	}
}

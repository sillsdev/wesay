using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	sealed public class LexExampleSentence : WeSayDataObject
	{
		private MultiText _sentence;
		private MultiText _translation;


		new public class WellKnownProperties : WeSayDataObject.WellKnownProperties
		{
			static public string Source = "source";
		} ;

		public LexExampleSentence(WeSayDataObject parent)
			: base(parent)
		{
			_sentence = new MultiText();
			_translation = new MultiText();

			WireUpEvents();
		}


		/// <summary>
		/// Used when a list of these items adds via "AddNew", where we have to have a default constructor.
		/// The parent is added in an even handler, on the parent, which is called by the list.
		/// </summary>
		public LexExampleSentence()
			: this(null)
		{
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
				return Sentence.Empty &&
					   Translation.Empty &&
					   !HasProperties;
			}
		}

	}
}

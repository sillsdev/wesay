using System.ComponentModel;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public sealed class LexSense : WeSayDataObject
	{
		private SenseGlossMultiText _gloss;
		private InMemoryBindingList<LexExampleSentence> _exampleSentences;

		new public class WellKnownProperties : WeSayDataObject.WellKnownProperties
		{
			static public string PartOfSpeech = "POS";
			static public string Definition = "def";
		} ;

		public LexSense(WeSayDataObject parent)
			: base(parent)
		{
			_gloss = new SenseGlossMultiText(this);
			_exampleSentences = new WeSay.Data.InMemoryBindingList<LexExampleSentence>();
			WireUpEvents();
		}

		/// <summary>
		/// Used when a list of these items adds via "AddNew", where we have to have a default constructor.
		/// The parent is added in an even handler, on the parent, which is called by the list.
		/// </summary>
		public LexSense(): this(null)
		{
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
				return Gloss.Empty &&
					   ExampleSentences.Count == 0 &&
					   !HasProperties;
			}
		}

		public override void CleanUpAfterEditting()
		{
			base.CleanUpAfterEditting();
			foreach (LexExampleSentence sentence in _exampleSentences)
			{
				sentence.CleanUpAfterEditting();
			}
		}

		private void RemoveEmptyExampleSentences() {
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
		public override void SomethingWasModified(string propertyModified)
		{
			base.SomethingWasModified(propertyModified);
			if (propertyModified != "exampleSentences")
			{
				RemoveEmptyExampleSentences();
			}
		}
	}

	/// <summary>
	/// See comment in MultiText.cs for an explanation of this class.
	/// </summary>
	public class SenseGlossMultiText : MultiText
	{
		public SenseGlossMultiText(LexSense parent)
			: base(parent)
		{
		}

		public new LexSense Parent
		{
			get { return _parent as LexSense; }
		}
	}
}

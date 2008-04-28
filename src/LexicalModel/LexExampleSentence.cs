using System.Collections.Generic;
using WeSay.Foundation;

namespace WeSay.LexicalModel
{
	sealed public class LexExampleSentence : WeSayDataObject
	{
		private MultiText _sentence;
		private MultiText _translation;
		private string _translationType;

		//!!What!! Is this done this way so that we don't end up storing
		//  the data in the object database?
		new public class WellKnownProperties : WeSayDataObject.WellKnownProperties
		{
			static public string ExampleSentence = "ExampleSentence";
			static public string Translation = "ExampleTranslation";
			static public string Source = "source";


			static public bool Contains(string fieldName)
			{
				List<string> list = new List<string>(new string[] {ExampleSentence, Source, Translation });
				return list.Contains(fieldName);
			}

		} ;

		public LexExampleSentence(WeSayDataObject parent)
			: base(parent)
		{
			_sentence = new MultiText(this);
			_translation = new MultiText(this);

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

		public override bool IsEmpty
		{
			get
			{
				return Sentence.Empty &&
					   Translation.Empty &&
					   !HasProperties;
			}
		}

		/// <summary>
		/// Supports round-tripping, though we don't use it
		/// </summary>
		public string TranslationType
		{
			get
			{
				return _translationType;
			}
			set { _translationType = value; }
		}
	}
}

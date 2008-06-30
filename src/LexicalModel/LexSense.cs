using System;
using System.Collections.Generic;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel
{
	public sealed class LexSense: WeSayDataObject
	{
		//private readonly SenseGlossMultiText _gloss;
		private readonly InMemoryBindingList<LexExampleSentence> _exampleSentences;
		private string _id;

		public new class WellKnownProperties: WeSayDataObject.WellKnownProperties
		{
			public static string PartOfSpeech = "POS";
			public static string SemanticDomainsDdp4 = "SemanticDomainDdp4";

			public static string Definition = "definition";
			//the lower case here is defined by LIFT standard

			public static string Picture = "Picture";
			public static string Gloss = "gloss";
			//static public string Relations = "relations";
			public static bool ContainsAnyCaseVersionOf(string fieldName)
			{
				List<string> list =
						new List<string>(
								new string[]
										{
												PartOfSpeech, Definition, SemanticDomainsDdp4, Picture,
												Note, Gloss
										});
				return
						list.Contains(fieldName) || list.Contains(fieldName.ToLower()) ||
						list.Contains(fieldName.ToUpper());
			}
		} ;

		public LexSense(WeSayDataObject parent): base(parent)
		{
			//   _gloss = new SenseGlossMultiText(this);
			_exampleSentences = new InMemoryBindingList<LexExampleSentence>();
			WireUpEvents();
		}

		/// <summary>
		/// Used when a list of these items adds via "AddNew", where we have to have a default constructor.
		/// The parent is added in an event handler, on the parent, which is called by the list.
		/// </summary>
		public LexSense(): this(null) {}

		protected override void WireUpEvents()
		{
			base.WireUpEvents();
			//WireUpChild(_gloss);
			WireUpList(_exampleSentences, "exampleSentences");
		}

		public string GetOrCreateId()
		{
			if (String.IsNullOrEmpty(_id))
			{
				_id = Guid.NewGuid().ToString();
				NotifyPropertyChanged("id");
			}

			return _id;
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>The signature here is MultiText rather than SenseGlossMultiText because we want
		/// to hide this (hopefully temporary) performance implementation detail. </remarks>
		public MultiText Gloss
		{
			get { return GetOrCreateProperty<SenseGlossMultiText>(WellKnownProperties.Gloss); }
		}

		public MultiText Definition
		{
			get { return GetOrCreateProperty<MultiText>(WellKnownProperties.Definition); }
		}

		public IList<LexExampleSentence> ExampleSentences
		{
			get { return _exampleSentences; }
		}

		public override bool IsEmpty
		{
			get { return Gloss.Empty && ExampleSentences.Count == 0 && !HasProperties; }
		}

		public bool IsEmptyForPurposesOfDeletion
		{
			get
			{
				SenseGlossMultiText gloss =
						GetProperty<SenseGlossMultiText>(WellKnownProperties.Gloss);
				bool noGloss = (gloss == null) || gloss.Empty;
				// careful, just asking the later will actually create a gloss
				return noGloss && ExampleSentences.Count == 0 && !HasPropertiesForPurposesOfDeletion;
			}
		}

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public override void CleanUpAfterEditting()
		{
			base.CleanUpAfterEditting();
			foreach (LexExampleSentence sentence in _exampleSentences)
			{
				sentence.CleanUpAfterEditting();
			}
			CleanUpEmptyObjects();
		}

		public override void CleanUpEmptyObjects()
		{
			base.CleanUpEmptyObjects();

			for (int i = 0;i < _exampleSentences.Count;i++)
			{
				_exampleSentences[i].CleanUpEmptyObjects();
			}

			// remove any example sentences that are empty
			int count = _exampleSentences.Count;

			for (int i = count - 1;i >= 0;i--)
			{
				if (_exampleSentences[i].IsEmpty)
				{
					_exampleSentences.RemoveAt(i);
				}
			}
			if (count != _exampleSentences.Count)
			{
				Logger.WriteMinorEvent("Empty example removed");
				OnEmptyObjectsRemoved();
			}
		}
	}

	/// <summary>
	/// See comment in MultiText.cs for an explanation of this class.
	/// </summary>
	public class SenseGlossMultiText: MultiText
	{
		public SenseGlossMultiText(WeSayDataObject parent): base(parent) {}

		public SenseGlossMultiText() {}

		public new LexSense Parent
		{
			get { return base.Parent as LexSense; }
		}
	}
}
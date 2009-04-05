using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using System.Linq;

namespace WeSay.LexicalModel
{
	public class MissingFieldQuery: IFieldQuery<LexEntry>
	{
		private readonly Field _field;
		private readonly string[] _writingSystemsOfInterest;

		public MissingFieldQuery(Field field, string[] writingSystemsOfInterest)
		{
			Guard.AgainstNull(field, "field");
			_field = field;

			if (writingSystemsOfInterest == null || writingSystemsOfInterest.Length == 0)
			{
				_writingSystemsOfInterest = field.WritingSystemIds.ToArray();
			}
			else
			{
				_writingSystemsOfInterest = writingSystemsOfInterest;
			}
		}

		#region IFilter<LexEntry> Members

		/// <summary>
		/// Filters are kept in a list; this is the string by which a filter is accessed.
		/// </summary>
		public string Key
		{
			get
			{
				string key = "Missing " + Field.FieldName;
				List<string> writingSystemIds = new List<string>(Field.WritingSystemIds);
				writingSystemIds.Sort(StringComparer.InvariantCulture);
				foreach (string writingSystemId in writingSystemIds)
				{
					key += " [" + writingSystemId + "]";
				}
				key += " Filter";
				return key;
			}
		}

		public Predicate<LexEntry> FilteringPredicate
		{
			get { return IsMissingItem; }
		}

		public string FieldName
		{
			get { return Field.FieldName; }
		}

		public Field Field
		{
			get { return _field; }
		}

		private bool IsMissingData(object content)
		{
			switch (Field.DataTypeName)
			{
				case "Option":
					return ((OptionRef) content).IsEmpty;
				case "OptionCollection":
					return ((OptionRefCollection) content).IsEmpty;
				case "MultiText":
					return IsMissingAnyWritingSystemOfInterest((MultiText) content);

				case "RelationToOneEntry":
					LexRelationCollection collection = (LexRelationCollection) content;
					if (IsSkipped(collection.Parent, Field.FieldName))
					{
						return false;
					}
					else
					{
						foreach (LexRelation r in collection.Relations)
						{
							if (!string.IsNullOrEmpty(r.TargetId))
							{
								return false; // has one non-empty relation
							}
						}
						return true;
						//collection is empty or all its members don't really have targets
					}
				default:
					Debug.Fail("unknown DataTypeName");
					return false;
			}
		}

		private bool IsMissingItem(LexEntry entry)
		{
			if (entry == null)
			{
				return false;
			}

			switch (Field.ClassName)
			{
				case "LexEntry":
					return IsMissingLexEntryField(entry);
				case "LexSense": // fall through
				case "LexExampleSentence":
					foreach (LexSense sense in entry.Senses)
					{
						if (Field.ClassName == "LexSense")
						{
							if (IsMissingLexSenseField(sense))
							{
								return true;
							}
						}
						else
						{
							foreach (LexExampleSentence example in sense.ExampleSentences)
							{
								if (Field.ClassName == "LexExampleSentence")
								{
									if (IsMissingLexExampleSentenceField(example))
									{
										return true;
									}
								}
							}
							if (sense.ExampleSentences.Count == 0 &&
								(Field.FieldName == Field.FieldNames.ExampleSentence.ToString()))
							{
								//ghost field
								return true;
							}
						}
					}
					if (entry.Senses.Count == 0 &&
						(Field.FieldName == LexSense.WellKnownProperties.Definition))
					{
						//ghost field
						return true;
					}

					break;
				default:
					Debug.Fail("unknown ClassName");
					break;
			}
			return false;
		}

		private static bool IsSkipped(WeSayDataObject parent, string fieldName)
		{
			return parent.GetHasFlag("flag-skip-" + fieldName);
		}

		private bool IsMissingLexExampleSentenceField(LexExampleSentence example)
		{
			if (!Field.IsBuiltInViaCode)
			{
				return IsMissingCustomField(example);
			}
			else
			{
				if (Field.FieldName == Field.FieldNames.ExampleSentence.ToString())
				{
					return IsMissingAnyWritingSystemOfInterest(example.Sentence);
				}
				else if (Field.FieldName == Field.FieldNames.ExampleTranslation.ToString())
				{
					return IsMissingAnyWritingSystemOfInterest(example.Translation);
				}
				else
				{
					Debug.Fail("unknown FieldName");
					return false;
				}
			}
		}

		private bool IsMissingLexSenseField(WeSayDataObject sense)
		{
			if (!Field.IsBuiltInViaCode)
			{
				return IsMissingCustomField(sense);
			}
			else
			{
				//                if(this._field.FieldName == LexSense.WellKnownProperties.Gloss)
				//                {
				//                    return IsMissingWritingSystem(sense.Gloss);
				//                }
				//                else
				{
					Debug.Fail("unknown FieldName");
					return false;
				}
			}
		}

		private bool IsMissingLexEntryField(LexEntry entry)
		{
			if (!Field.IsBuiltInViaCode)
			{
				return IsMissingCustomField(entry);
			}
			else
			{
				if (Field.FieldName == Field.FieldNames.EntryLexicalForm.ToString())
				{
					if (IsMissingAnyWritingSystemOfInterest(entry.LexicalForm))
					{
						return true;
					}
				}
				else
				{
					Debug.Fail("unknown FieldName");
				}
			}
			return false;
		}

		private bool IsMissingCustomField(WeSayDataObject weSayData)
		{
			IParentable content = weSayData.GetProperty<IParentable>(Field.FieldName);
			if (content == null)
			{
				return !IsSkipped(weSayData, Field.FieldName);
			}
			if(Field.FieldName == "POS")
			{
				if(IsPosUnknown(weSayData))
				{
					return true;
				}
			}
			return IsMissingData(content);
		}

		private bool IsPosUnknown(WeSayDataObject sense)
		{
			foreach(KeyValuePair<string, object> property in sense.Properties)
			if (property.Key == "POS" && (((OptionRef) property.Value).Key == "unknown"))
			{
				return true;
			}
			return false;
		}


		private bool IsMissingAnyWritingSystemOfInterest(MultiTextBase text)
		{
			foreach (var wsId in _writingSystemsOfInterest )
			{
				if (!text.ContainsAlternative(wsId))
				{
					return true;
				}
			}
			return false;
		}


		#endregion
	}
}
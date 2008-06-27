using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class MissingFieldQuery: IFieldQuery<LexEntry>
	{
		private readonly Field _field;

		public MissingFieldQuery(Field field)
		{
			if (field == null)
			{
				throw new ArgumentNullException();
			}
			_field = field;
		}

		public MissingFieldQuery(ViewTemplate viewTemplate, string fieldName)
		{
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			Field field;
			if (!viewTemplate.TryGetField(fieldName, out field))
			{
				throw new ArgumentOutOfRangeException("viewTemplate",
													  "ViewTemplate is missing " + fieldName +
													  " field definition");
			}
			_field = field;
		}

		#region IFilter<LexEntry> Members

		/// <summary>
		/// Filters are kept in a list; this is the string by which a filter is accessed.
		/// </summary>
		public string Key
		{
			get
			{
				string key = "Missing " + this.Field.FieldName;
				List<string> writingSystemIds = new List<string>(this.Field.WritingSystemIds);
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
			get { return this.Field.FieldName; }
		}

		public Field Field
		{
			get { return this._field; }
		}

		private bool IsMissingDataInWritingSystem(object content)
		{
			switch (this.Field.DataTypeName)
			{
				case "Option":
					return ((OptionRef) content).IsEmpty;
				case "OptionCollection":
					return ((OptionRefCollection) content).IsEmpty;
				case "MultiText":
					return IsMissingWritingSystem((MultiText) content);
				case "RelationToOneEntry":
					LexRelationCollection collection = (LexRelationCollection) content;
					if (IsSkipped(collection.Parent, this.Field.FieldName))
					{
						return false;
					}
					else
					{
						foreach (LexRelation r in collection.Relations)
						{
							if (r.TargetId != null)
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

			switch (this.Field.ClassName)
			{
				case "LexEntry":
					return IsMissingLexEntryField(entry);
				case "LexSense": // fall through
				case "LexExampleSentence":
					foreach (LexSense sense in entry.Senses)
					{
						if (this.Field.ClassName == "LexSense")
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
								if (this.Field.ClassName == "LexExampleSentence")
								{
									if (IsMissingLexExampleSentenceField(example))
									{
										return true;
									}
								}
							}
							if (sense.ExampleSentences.Count == 0 &&
								(this.Field.FieldName == Field.FieldNames.ExampleSentence.ToString()))
							{
								//ghost field
								return true;
							}
						}
					}
					if (entry.Senses.Count == 0 &&
						(this.Field.FieldName == LexSense.WellKnownProperties.Definition))
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
			return parent.GetHasFlag("flag_skip_" + fieldName);
		}

		private bool IsMissingLexExampleSentenceField(LexExampleSentence example)
		{
			if (!this.Field.IsBuiltInViaCode)
			{
				return IsMissingCustomField(example);
			}
			else
			{
				if (this.Field.FieldName == Field.FieldNames.ExampleSentence.ToString())
				{
					return IsMissingWritingSystem(example.Sentence);
				}
				else if (this.Field.FieldName == Field.FieldNames.ExampleTranslation.ToString())
				{
					return IsMissingWritingSystem(example.Translation);
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
			if (!this.Field.IsBuiltInViaCode)
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
			if (!this.Field.IsBuiltInViaCode)
			{
				return IsMissingCustomField(entry);
			}
			else
			{
				if (this.Field.FieldName == Field.FieldNames.EntryLexicalForm.ToString())
				{
					if (IsMissingWritingSystem(entry.LexicalForm))
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
			IParentable field = weSayData.GetProperty<IParentable>(this.Field.FieldName);
			if (field == null)
			{
				return !IsSkipped(weSayData, this.Field.FieldName);
			}
			return IsMissingDataInWritingSystem(field);
		}

		private bool IsMissingWritingSystem(MultiTextBase field)
		{
			foreach (string wsId in this.Field.WritingSystemIds)
			{
				if (field[wsId].Length == 0)
				{
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}
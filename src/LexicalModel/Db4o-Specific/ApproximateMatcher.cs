using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using WeSay.Data;

namespace WeSay.LexicalModel.Db4o_Specific
{
	public class ApproximateMatcher
	{
		static public IList<LexEntry> FindEntriesWithClosestLexemeForms(string key, IRecordListManager recordManager, IBindingList records)
		{
			return FindEntriesWithClosestLexemeForms(key, false, false, recordManager, records);
		}

		static public IList<LexEntry> FindEntriesWithClosestAndPrefixedLexemeForms(string key, IRecordListManager recordManager, IBindingList records)
		{
			return FindEntriesWithClosestLexemeForms(key, false, true, recordManager, records);
		}

		static public IList<LexEntry> FindClosestAndNextClosest(string key, IRecordListManager recordManager, IBindingList records)
		{
			return FindEntriesWithClosestLexemeForms(key, true, false, recordManager, records);
		}

		static public IList<LexEntry> FindClosestAndNextClosestAndPrefixed(string key, IRecordListManager recordManager, IBindingList records)
		{
			return FindEntriesWithClosestLexemeForms(key, true, true, recordManager, records);
		}


		static private IList<LexEntry> FindEntriesWithClosestLexemeForms(string key, bool includeNextClosest, bool includePrefixedForms, IRecordListManager recordManager, IBindingList records)
		{
			int bestEditDistance = int.MaxValue;
			IList<LexEntry> bestMatches = new List<LexEntry>();
			if (recordManager is Db4oRecordListManager)
			{
				Db4oDataSource db4oData = ((Db4oRecordListManager)recordManager).DataSource;
//                IRecordList<LexicalFormMultiText> lexicalForms = recordManager.GetListOfType<LexicalFormMultiText>();
				IExtObjectContainer database = db4oData.Data.Ext();
				IList<int> bestLexicalFormIds;
				List<string> lexicalFormsText = new List<string>();
				List<Type> OriginalList = Db4oLexModelHelper.Singleton.DoNotActivateTypes;
				Db4oLexModelHelper.Singleton.DoNotActivateTypes = new List<Type>();
				Db4oLexModelHelper.Singleton.DoNotActivateTypes.Add(typeof(LexEntry));


				IQuery query = database.Query();
				query.Constrain(typeof(LexicalFormMultiText));
				IObjectSet lexicalForms = query.Execute();



				foreach (LexicalFormMultiText lexicalForm in lexicalForms)
				{
					lexicalFormsText.Add(lexicalForm.ToString());
				}
				Db4oLexModelHelper.Singleton.DoNotActivateTypes = OriginalList;

				bestEditDistance = GetClosestLexicalForms(lexicalFormsText, key, 0, int.MaxValue, includePrefixedForms, out bestLexicalFormIds);
				GetEntriesFromLexicalForms(database, bestMatches, lexicalForms, bestLexicalFormIds);
				if (includeNextClosest || bestMatches.Count == 0)
				{
					GetClosestLexicalForms(lexicalFormsText, key, bestEditDistance + 1, int.MaxValue, false, out bestLexicalFormIds);
					GetEntriesFromLexicalForms(database, bestMatches, lexicalForms, bestLexicalFormIds);
				}
			}
			else
			{
				foreach (LexEntry entry in records)
				{
					int editDistance = EditDistance(key, entry.ToString(), bestEditDistance);
					if (editDistance < bestEditDistance)
					{
						bestMatches.Clear();
						bestEditDistance = editDistance;
					}
					if (editDistance == bestEditDistance)
					{
						bestMatches.Add(entry);
					}
				}
			}
			return bestMatches;
		}

		static private int GetClosestLexicalForms(IList<string> lexicalForms, string key, int minEditDistance, int maxEditDistance, bool includePrefixedForms, out IList<int> closestLexicalFormIds)
		{
			int bestEditDistance = maxEditDistance;
			closestLexicalFormIds = new List<int>();
			int i = 0;
			foreach (string lexicalForm in lexicalForms)
			{
				if (!string.IsNullOrEmpty(lexicalForm))
				{
					int editDistance;
					if (includePrefixedForms && lexicalForm.StartsWith(key))
					{
						editDistance = 0;
					}
					else
					{
						editDistance = EditDistance(key, lexicalForm, bestEditDistance);
					}
					if (minEditDistance <= editDistance && editDistance < bestEditDistance)
					{
						closestLexicalFormIds.Clear();
						bestEditDistance = editDistance;
					}
					if (editDistance == bestEditDistance)
					{
						closestLexicalFormIds.Add(i);
					}
				}
				++i;
			}
			return bestEditDistance;
		}

		static private void GetEntriesFromLexicalForms(Db4objects.Db4o.Ext.IExtObjectContainer database, IList<LexEntry> bestMatches, IList LexicalForms, IList<int> bestIds)
		{
			//review: now that we have "parent" paths, would using that be faster than this?
			foreach (int id in bestIds)
			{
				LexicalFormMultiText lexicalForm = (LexicalFormMultiText)LexicalForms[id];
				database.Activate(lexicalForm, 99);
				//IQuery query = database.Query();
				//query.Constrain(typeof(LexEntry));
				//query.Descend("_lexicalForm").Constrain(lexicalForm).Identity();
				//IObjectSet entries = query.Execute();
				//// If LexEntry does not cascade delete its lexicalForm then we could have a case where we
				//// don't have a entry associated with this lexicalForm.
				//if (entries.Count == 0)
				//{
				//    continue;
				//}
				//bestMatches.Add((LexEntry)entries[0]);
				if (lexicalForm.Parent != null)
				{
					bestMatches.Add(lexicalForm.Parent);
				}
			}
		}

		private static int EditDistance(string list1, string list2, int maxEditDistance)
		{
			const int deletionCost = 1;
			const int insertionCost = deletionCost; // should be symmetric
			const int substitutionCost = 1;
			const int transpositionCost = 1;
			int lastColumnThatNeedsToBeEvaluated = 2;

			// Validate parameters
			if (list1 == null)
				throw new ArgumentNullException("x");
			if (list2 == null)
				throw new ArgumentNullException("y");

			// list2 is the one that we are actually using storage space for so we want it to be the smaller of the two
			if (list1.Length < list2.Length)
			{
				swap(ref list1, ref list2);
			}
			int n1 = list1.Length, n2 = list2.Length;
			if (n1 == 0)
			{
				return n2 * insertionCost;
			}
			if (n2 == 0)
			{
				return n1 * deletionCost;
			}

			// Rather than maintain an entire matrix (which would require O(x*y) space),
			// just store the previous row, current row, and next row, each of which has a length min(x,y)+1,
			// so just O(min(x,y)) space.
			int prevRow = 0, curRow = 1, nextRow = 2;
			int[][] rows = new int[][] { new int[n2 + 1], new int[n2 + 1], new int[n2 + 1] };
			// Initialize the previous row.
			int maxIndex = Math.Min(n2, lastColumnThatNeedsToBeEvaluated);
			for (int list2index = 0; list2index <= maxIndex; ++list2index)
			{
				rows[curRow][list2index] = list2index;
				if (list2index <= maxEditDistance)
				{
					lastColumnThatNeedsToBeEvaluated = list2index;
				}
			}

			// For each virtual row (since we only have physical storage for two)
			for (int list1index = 1; list1index <= n1; ++list1index)
			{
				// Fill in the values in the row
				rows[nextRow][0] = list1index;
				maxIndex = Math.Min(n2, lastColumnThatNeedsToBeEvaluated + 1);
				lastColumnThatNeedsToBeEvaluated = 0;
				for (int list2index = 1; list2index <= maxIndex; ++list2index)
				{
					int distance;

					if (list1[list1index - 1].Equals(list2[list2index - 1]))
					{
						distance = rows[curRow][list2index - 1]; //assumes equal cost is 0
					}
					else
					{
						int deletionDistance = rows[curRow][list2index] + deletionCost;
						int insertionDistance = rows[nextRow][list2index - 1] + insertionCost;
						int substitutionDistance = rows[curRow][list2index - 1] + substitutionCost;

						distance = Math.Min(deletionDistance, Math.Min(insertionDistance, substitutionDistance));

						if (list1index > 1 && list2index > 1 &&
							list1[list1index - 1].Equals(list2[list2index - 2]) &&
							list1[list1index - 2].Equals(list2[list2index - 1]))
						{
							distance = Math.Min(distance, rows[prevRow][list2index - 2] + transpositionCost);
						}
					}
					rows[nextRow][list2index] = distance;
					if (distance <= maxEditDistance)
					{
						lastColumnThatNeedsToBeEvaluated = list2index;
					}
				}

				// cycle the previous, current and next rows
				switch (prevRow)
				{
					case 0:
						prevRow = 1;
						curRow = 2;
						nextRow = 0;
						break;
					case 1:
						prevRow = 2;
						curRow = 0;
						nextRow = 1;
						break;
					case 2:
						prevRow = 0;
						curRow = 1;
						nextRow = 2;
						break;
				}

				if(lastColumnThatNeedsToBeEvaluated == 0)
				{
					return int.MaxValue;
				}
			}

			// Return the computed edit distance
			return rows[curRow][n2];
		}

		private static void swap<A>(ref A x, ref A y)
		{
			A temp = x;
			x = y;
			y = temp;
		}
	}
}

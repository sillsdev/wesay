using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WeSay.LexicalModel.Db4o_Specific
{
	public class ApproximateMatcher
	{
		static public IList<string> FindClosestForms(string key, IEnumerable<string> forms)
		{
			return FindClosestForms(key, false, false, forms);
		}

		static public IList<string> FindClosestAndPrefixedForms(string key, IEnumerable<string> forms)
		{
			return FindClosestForms(key, false, true, forms);
		}

		static public IList<string> FindClosestAndNextClosestForms(string key, IEnumerable<string> forms)
		{
			return FindClosestForms(key, true, false, forms);
		}

		static public IList<string> FindClosestAndNextClosestAndPrefixedForms(string key, IEnumerable<string> forms)
		{
			return FindClosestForms(key, true, true, forms);
		}


		static private IList<string> FindClosestForms(string notNormalizedkey, bool includeNextClosest, bool includePrefixedForms, IEnumerable<string> forms)
		{
			string key = notNormalizedkey.Normalize(NormalizationForm.FormD);
			List<string> bestMatches = new List<string>();
			List<string> secondBestMatches = new List<string>();

			int bestEditDistance = int.MaxValue;
			int secondBestEditDistance = int.MaxValue;

			foreach (string originalForm in forms)
			{
				string form = originalForm.Normalize(NormalizationForm.FormD);
				if (!string.IsNullOrEmpty(form))
				{
					int editDistance;
					if (includePrefixedForms && form.StartsWith(key))
					{
						editDistance = 0;
					}
					else
					{
						editDistance = EditDistance(key, form, secondBestEditDistance);
					}
					if (editDistance < bestEditDistance)
					{
						if (includeNextClosest && bestEditDistance != int.MaxValue)
						{
							// best becomes second best
							secondBestMatches.Clear();
							secondBestMatches.AddRange(bestMatches);
							secondBestEditDistance = bestEditDistance;
						}
						bestMatches.Clear();
						bestEditDistance = editDistance;
					}
					else if (includeNextClosest &&
							 editDistance > bestEditDistance &&
							 editDistance < secondBestEditDistance)
					{
						secondBestEditDistance = editDistance;
						secondBestMatches.Clear();
					}
					if (editDistance == bestEditDistance)
					{
						bestMatches.Add(originalForm);
					}
					else if (includeNextClosest && editDistance == secondBestEditDistance)
					{
						secondBestMatches.Add(originalForm);
					}
					Debug.Assert(bestEditDistance != secondBestEditDistance);
				}
			}
			if (includeNextClosest)
			{
				bestMatches.AddRange(secondBestMatches);
			}
			return bestMatches;
		}


		public const int EditDistanceLargerThanMax = int.MaxValue;


		// The Damerau-Levenshtein distance is equal to the minimal number of insertions, deletions, substitutions and transpositions needed to transform one string into anothe
		// http://en.wikipedia.org/wiki/Damerau-Levenshtein_distance
		// This algorithm is O(|x||y|) time and O(min(|x|,|y|)) space in worst and average case
		// Ukkonen 1985 Algorithms for approximate string matching. Information and Control 64, 100-118.
		// Eugene W. Myers 1986. An O (N D) difference algorithm and its variations. Algorithmica 1:2, 251-266.
		// are algorithm that can compute the edit distance in O(editdistance(x,y)^2) time
		// and O(k) space
		// using a diagonal transition algorithm

		// Ukkonen's cut-off heuristic is faster than the original Sellers 1980

		// returns int.MaxValue if distance is greater than cutoff.
		public static int EditDistance(string list1, string list2, int maxEditDistance)
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
				// if we are on the last row and we don't need to evaluate to the end of
				// the column to determine if our edit distance is larger than the max
				// then the edit distance is larger than the max
				if(list1index == n1 && maxIndex < n2)
				{
					return EditDistanceLargerThanMax;
				}
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
					return EditDistanceLargerThanMax;
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

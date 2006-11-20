using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using WeSay.Data;
using WeSay.Language;

namespace WeSay.LexicalModel.Db4o_Specific
{
	public class Db4oLexQueryHelper
	{

		[CLSCompliant(false)]
		static public List<AncestorType> FindObjectsFromLanguageForm<AncestorType, MultiTextType>(IRecordListManager recordManager, string match)
			where AncestorType : class
			where MultiTextType : MultiText
		{
			if (recordManager is Db4oRecordListManager)
			{
				return FindObjectsFromLanguageForm<AncestorType, MultiTextType> (((Db4oRecordListManager)recordManager).DataSource, match);
			}
			else
			{
				throw new NotImplementedException("this search is not available for non-db4o sources");
			}
		}

		public static List<AncestorType> FindObjectsFromLanguageForm<AncestorType, MultiTextType>(Db4oDataSource db, string match)
			where AncestorType : class
			where MultiTextType : MultiText
		{
			Db4objects.Db4o.Query.IQuery q = db.Data.Query();
			q.Constrain(typeof(LanguageForm));
			q.Descend("_form").Constrain(match);
			q.Descend("_parent").Constrain(typeof(MultiTextType));
			IObjectSet matches = q.Execute();
			return FindAncestorsOfLanguageForms<AncestorType, MultiTextType>(matches);
		}


		static private List<AncestorType> FindAncestorsOfLanguageForms<AncestorType, MultiTextType>(IObjectSet matches)
			where AncestorType : class
			where MultiTextType : MultiText
		{
			List<AncestorType> list = new List<AncestorType>(matches.Count);
			foreach (LanguageForm languageForm in matches)
			{
				MultiTextType multiText = languageForm.Parent as MultiTextType;

				//walk up the tree until we find a parent of the desired class
				WeSayDataObject parent = multiText.ParentAsObject as WeSayDataObject;
				while ((parent != null) && !(parent is AncestorType))
				{
					parent = parent.Parent;
				}
				if (parent != null)
				{
					list.Add(parent as AncestorType);
				}
			}
			return list;
		}

		static public bool HasMatchingLexemeForm(LanguageForm form)
		{
			return form.Form == "findme" && form.Parent.GetType() == typeof(LexicalFormMultiText);
		}

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
	}
}

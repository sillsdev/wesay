using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using com.db4o;
using com.db4o.ext;
using com.db4o.query;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
using Debug=System.Diagnostics.Debug;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : UserControl
	{
		private IBindingList _records;
		private readonly FieldInventory _fieldInventory;
		public event EventHandler SelectedIndexChanged;
		private IRecordListManager _recordManager;

		public EntryDetailControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public EntryDetailControl(IRecordListManager recordManager, FieldInventory fieldInventory)
		{
			if (recordManager == null)
			{
				throw new ArgumentNullException("records");
			}
			if (fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_recordManager = recordManager;
			_records = recordManager.Get<LexEntry>();
			_fieldInventory = fieldInventory;
			InitializeComponent();
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.DataSource = CurrentRecord;

			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			_findText.TextChanged += new EventHandler(_findText_TextChanged);
			_findText.KeyDown += new KeyEventHandler(_findText_KeyDown);
			int originalHeight = _findText.Height;
			_findText.Font = _recordsListBox.Font;
			_findText.WritingSystem = _fieldInventory[Field.FieldNames.EntryLexicalForm.ToString()].WritingSystems[0];
			int heightDifference = _findText.Height - originalHeight;
			_recordsListBox.Height -= heightDifference;
			_recordsListBox.Location = new Point(_recordsListBox.Location.X,
												 _recordsListBox.Location.Y + heightDifference);
			_btnFind.Height += heightDifference;
		}

		void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Modifiers == Keys.None &&
			   e.KeyData == Keys.Enter)
			{
				Find(this._findText.Text);
			}
		}

		void _findText_TextChanged(object sender, EventArgs e)
		{
			if (_btnFind.Text == StringCatalog.Get("Clear"))
			{
				_btnFind.Text = StringCatalog.Get("Find");
			}
		}

		void _btnFind_Click(object sender, System.EventArgs e)
		{
			if (this._btnFind.Text == StringCatalog.Get("Find"))
			{
				Find(this._findText.Text);
			}
			else
			{
				ClearLastFind();
			}
		}


		private void ClearLastFind() {
			// reset to original records
			this._records = this._recordManager.Get<LexEntry>();
			this._recordsListBox.DataSource = this._records;
			this._btnFind.Text = StringCatalog.Get("Find");

			// toggle state between clear and find
			this._findText.ResetText();
		}

		private void Find(string text) {
			Cursor currentCursor = Cursor;
			Cursor = Cursors.WaitCursor;
			InMemoryBindingList<object> records = new InMemoryBindingList<object>();
			records.AddRange(FindClosest(text));
			this._records = records;
			this._recordsListBox.DataSource = this._records;

			// toggle state between find and clear
			this._btnFind.Text = StringCatalog.Get("Clear");
			Cursor = currentCursor;
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
		private static int EditDistance(string list1, string list2, int cutoff)
		{
			const int deletionCost = 1;
			const int insertionCost = deletionCost; // should be symmetric
			const int substitutionCost = 1;
			const int transpositionCost = 1;

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
			for (int list2index = 0; list2index <= n2; ++list2index)
			{
				rows[curRow][list2index] = list2index;
			}

			// For each virtual row (since we only have physical storage for two)
			for (int list1index = 1; list1index <= n1; ++list1index)
			{
				// Fill in the values in the row
				rows[nextRow][0] = list1index;
				for (int list2index = 1; list2index <= n2; ++list2index)
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


		public IList<LexEntry> FindClosest(string key)
		{
			int bestEditDistance = int.MaxValue;
			IList<LexEntry> bestMatches = new List<LexEntry>();
			if (_recordManager is Db4oRecordListManager)
			{
				Db4oDataSource db4oData = ((Db4oRecordListManager) _recordManager).DataSource;
				IRecordList<LexicalFormMultiText> lexicalForms = _recordManager.Get<LexicalFormMultiText>();
				ExtObjectContainer database = db4oData.Data.Ext();
				IList<LexicalFormMultiText> best;
				bestEditDistance = GetClosestLexicalForms(lexicalForms, key, 0, bestEditDistance, out best);
				GetEntriesFromLexicalForms(database, bestMatches, best);
				if(bestMatches.Count == 0)
				{
					GetClosestLexicalForms(lexicalForms, key, bestEditDistance + 1, int.MaxValue, out best);
					GetEntriesFromLexicalForms(database, bestMatches, best);
				}
			}
			else
			{
				foreach (LexEntry entry in _records)
				{
					int editDistance = EditDistance(key, entry.ToString(), bestEditDistance);
					if(editDistance < bestEditDistance)
					{
						bestMatches.Clear();
						bestEditDistance = editDistance;
					}
					if(editDistance == bestEditDistance)
					{
						bestMatches.Add(entry);
					}
				}
			}
			return bestMatches;
		}

		private static int GetClosestLexicalForms(IRecordList<LexicalFormMultiText> lexicalForms, string key, int minEditDistance, int maxEditDistance, out IList<LexicalFormMultiText> closestLexicalForms) {
			int bestEditDistance = maxEditDistance;
			closestLexicalForms = new List<LexicalFormMultiText>();

			foreach (LexicalFormMultiText lexicalForm in lexicalForms)
			{
				string vernacularLexicalForm = lexicalForm.ToString();
				if (!string.IsNullOrEmpty(vernacularLexicalForm))
				{
					int editDistance = EditDistance(key, vernacularLexicalForm, bestEditDistance);
					if (minEditDistance <= editDistance && editDistance < bestEditDistance)
					{
						closestLexicalForms.Clear();
						bestEditDistance = editDistance;
					}
					if (editDistance == bestEditDistance)
					{
						closestLexicalForms.Add(lexicalForm);
					}
				}
			}
			return bestEditDistance;
		}

		private static void GetEntriesFromLexicalForms(ExtObjectContainer database, IList<LexEntry> bestMatches, IList<LexicalFormMultiText> best) {
			foreach (LexicalFormMultiText lexicalForm in best)
			{
				Query query = database.Query();
				query.Constrain(typeof(LexEntry));
				query.Descend("_lexicalForm").Constrain(lexicalForm).Identity();
				ObjectSet entries = query.Execute();
				// If LexEntry does not cascade delete it's lexicalForm then we could have a case where we
				// don't have a entry associated with this lexicalForm.
				if (entries.Count == 0)
				{
					continue;
				}
				bestMatches.Add((LexEntry)entries[0]);
			}
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (_entryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}
			_entryDetailPanel.DataSource = CurrentRecord;
			_btnDeleteWord.Enabled = (CurrentRecord != null);
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this,null);
			}
		}

		private LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return (LexEntry)_records[CurrentIndex];
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}

		private void _btnNewWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LexEntry entry = new LexEntry();
			_records.Add(entry);
			_recordsListBox.SelectedIndex = _records.IndexOf(entry);
		}

		private void _btnDeleteWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Debug.Assert(CurrentIndex >= 0);
			_records.RemoveAt(CurrentIndex);
			//hack until we can get selection change events sorted out in BindingGridList
			OnRecordSelectionChanged(this, null);
			_recordsListBox.Refresh();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
	class WordGridHandler
	{
		protected DataService _dataService;

		public WordGridHandler(Gtk.TreeView entryList, DataService dataService)
		{
			_dataService = dataService;

			AddColumn(entryList, "Word", new TreeCellDataFunc(OnRenderLexemeForm));
			AddColumn(entryList, "Gloss", new TreeCellDataFunc(OnRenderGloss));

			TreeStore store = new TreeStore(typeof(int));
			_dataService = new DataService(@"c:\WeSay\src\unittests\thai5000.yap");

			int count = _dataService.LexicalEntries.Count;
			for (int i = 0; i < count; i++)
			{
				store.AppendValues(i);
			}

			entryList.Model = store;
		}

		private void AddColumn(Gtk.TreeView entryList, string title, TreeCellDataFunc handler)
		{
			Gtk.CellRendererText renderer = new Gtk.CellRendererText();
			Gtk.TreeViewColumn column = new Gtk.TreeViewColumn(title, renderer, new object[] { });
			column.SetCellDataFunc(renderer, handler);
			entryList.AppendColumn(column);
		}

		public void OnRenderLexemeForm(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			LexicalEntry entry = GetEnterFromIterator(tree_model, ref iter);
			(cell as Gtk.CellRendererText).Text = entry.LexicalForm;
		}

		public void OnRenderGloss(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			LexicalEntry entry = GetEnterFromIterator(tree_model, ref iter);
			(cell as Gtk.CellRendererText).Text = entry.Gloss;
		}

		private LexicalEntry GetEnterFromIterator(TreeModel tree_model, ref TreeIter iter)
		{
			int i = ((int)tree_model.GetValue(iter, 0));
			LexicalEntry entry = (LexicalEntry)_dataService.LexicalEntries[i];
			return entry;
		}
	}
 }

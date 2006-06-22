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

			entryList.Model = _dataService.Model;
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
			(cell as Gtk.CellRendererText).Text = _dataService.GetLexemeForm(tree_model, iter);
		}

		public void OnRenderGloss(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			(cell as Gtk.CellRendererText).Text = _dataService.GetGloss(tree_model, iter);
		}
	}
 }

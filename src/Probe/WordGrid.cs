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
		[Widget]    protected Gtk.VBox _root;
		 [Widget]   protected Gtk.TreeView _entryList;

		public WordGridHandler(Container container, DataService dataService)
		{
			_dataService = dataService;

			Glade.XML gxml = new Glade.XML("probe.glade", "_wordGridHolder", null);
			gxml.Autoconnect(this);
			_root.Reparent(container);


			AddColumn(_entryList, "Word", new TreeCellDataFunc(OnRenderLexemeForm), 25);
			AddColumn(_entryList, "Gloss", new TreeCellDataFunc(OnRenderGloss),12);

			_entryList.Model = _dataService.Model;
		}

		private void AddColumn(Gtk.TreeView entryList, string title, TreeCellDataFunc handler, int size)
		{
			Gtk.CellRendererText renderer = new Gtk.CellRendererText();
			renderer.SizePoints = size;
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

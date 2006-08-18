using System;
using System.Collections.Generic;
using Gtk;
using System.ComponentModel;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class EntryViewTool : WeSay.UI.ITask
	{
		private VBox _container;
		private IBindingList _records;
		private System.Windows.Forms.BindingSource _bindingSource; //could get by with CurrencyManager

		public EntryViewTool( IBindingList records)
		{
			_records = records;
			_bindingSource = new System.Windows.Forms.BindingSource(_records, null);
			_bindingSource.PositionChanged += new EventHandler(OnPositionChanged);
	   }

		public void Activate()
		{
			AddToolbar();
			AddTable();
			_container.ShowAll();
		}

		private void AddToolbar()
		{
			Gtk.Toolbar bar = new Toolbar();
			Gtk.ToolButton previous = new ToolButton(Gtk.Stock.GoBack);
			bar.Add(previous);
			previous.Clicked += new EventHandler(OnPreviousClicked);
			 Gtk.ToolButton next = new ToolButton(Gtk.Stock.GoForward);
			bar.Add(next);
			next.Clicked += new EventHandler(OnNextClicked);
		   _container.PackStart(bar, false, false, 0);
		}

		private void AddTable()
		{
		   LexEntry record = _bindingSource.Current as LexEntry;
		   if (record == null)
		   {
			   _container.PackStart(new Label("No Records Yet"));
			   return; //what to do?
		   }

		   TableBuilder builder = new TableBuilder();
		   LexEntryLayouter layout = new LexEntryLayouter(builder);
		   layout.AddWidgets(record);

			_container.PackStart(builder.BuildTable());
		}


		void OnPreviousClicked(object sender, EventArgs e)
		{
			_bindingSource.MovePrevious();

		}
		void OnNextClicked(object sender, EventArgs e)
		{
			_bindingSource.MoveNext();

		}
		void OnPositionChanged(object sender, EventArgs e)
		{
			_container.Children[1].Destroy();
			AddTable();
			_container.ShowAll();
		}


		public void Deactivate()
		{
			//hack
			_container.Children[1].Destroy();
			_container.Children[0].Destroy();
		}

		public string Label
		{
			get {return "Dictionary";}
		}

		public VBox Container
		{
			get { return _container; }
			set { _container = value; }
		}
	}
}

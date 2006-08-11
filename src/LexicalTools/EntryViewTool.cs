using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using System.ComponentModel;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class EntryViewTool : WeSay.UI.ITask
	{
		private VBox _container;
		private  BindingList<LexEntry> _records;
		private System.Windows.Forms.BindingSource _bindingSource; //could get by with CurrencyManager

		public EntryViewTool(BindingList<LexEntry> records)
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
		   TableBuilder builder = new TableBuilder();
			builder.AddWidgetRow("word: ", MakeBoundEntry(record.LexicalForm, "en"));
			foreach (LexSense sense in record.Senses)
			{
				TableBuilder senseTableBuilder = new TableBuilder();
				senseTableBuilder.AddWidgetRow("gloss: ", MakeBoundEntry(sense.Gloss, "en"));
				foreach (LexExampleSentence example in sense.ExampleSentences)
				{
					senseTableBuilder.AddWidgetRow("example: ", MakeBoundEntry(example.Sentence, "en"));
					senseTableBuilder.AddWidgetRow("translation: ", MakeBoundEntry(example.Translation, "en"));
				}
				builder.AddWidgetRow("meaning: ", senseTableBuilder.BuildTable());
		  }
			_container.PackStart(builder.BuildTable());
		}

		private Entry MakeBoundEntry(WeSay.Language.MultiText text, string writingSystemId)
		{
			Gtk.Entry entry = new Gtk.Entry(text[writingSystemId]);
			WeSay.UI.Binding binding = new WeSay.UI.Binding(text, writingSystemId, entry);
			return entry;
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

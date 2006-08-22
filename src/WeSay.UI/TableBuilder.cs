using System;
using System.Collections.Generic;
using System.Text;
using Gtk;

namespace WeSay.UI
{
	/// <summary>
	/// Builds a standard two-column editing view.
	/// The GTK table must be told on construction how many rows it will have.
	/// So one thing this class does for us is allow us to just add rows as we
	/// discover them and then at the end have the table constructed at the end,
	/// with the proper size.
	/// </summary>
	public class TableBuilder
	{
		struct Row
		{
			public Row(string label, Widget widget)
			{
				_label = label;
				_widget = widget;
			}

			public string _label;
			public Widget _widget;
		}

		private Gtk.Table _table;
		private List<Row> _rows;

		public TableBuilder()
		{
			_rows = new List<Row>();
		}

		/// <summary>
		/// This is provided primarily for unit testing
		/// </summary>
		public int RowCount
		{
			get
			{
				return _rows.Count;
			}
		}

		public Gtk.Table BuildTable()
		{
			uint _rowsSoFar = 0;
			_table = new Table((uint)_rows.Count, 2, false);
			foreach (Row r in _rows)
			{
				Label label = new Gtk.Label(r._label);

				_table.Attach(GetLeftTopAlignment(label), 0, 1, _rowsSoFar, 1 + _rowsSoFar, AttachOptions.Fill, AttachOptions.Fill, 10, 10);
				if (r._widget != null)
				{
					_table.Attach(r._widget, 1, 2, _rowsSoFar, 1 + _rowsSoFar, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
				}
				++_rowsSoFar;
			}


			return _table;
		}

		private static Alignment GetLeftTopAlignment(Label label)
		{
			Alignment leftTopAlignment = new Alignment(0.0f, 0.0f, 0.0f, 0.0f);
			leftTopAlignment.Add(label);
			return leftTopAlignment;
		}

		public void AddWidgetRow(string label, Gtk.Widget widget)
		{
			_rows.Add(new Row(label, widget));

		}
		public void AddLabelRow(string label)
		{
			_rows.Add(new Row(label, null));
			//          _table.Attach(new Gtk.Label(label), 0, 1, _rowsSoFar, 1 + _rowsSoFar, AttachOptions.Fill, AttachOptions.Fill, 10, 10);
		}
	}
}

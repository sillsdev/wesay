using System;
using System.Collections.Generic;
using System.Text;
using Gtk;

namespace WeSay.UI
{
	/// <summary>
	/// Builds a standard two-column editing view.
	/// </summary>
	public class DetailViewManager
	{

		private Gtk.VBox _box;

		public DetailViewManager()
		{
			_box = new VBox();
		}

		/// <summary>
		/// This is provided primarily for unit testing
		/// </summary>
		public int RowCount
		{
			get { return _box.Children.Length; }
		}

		public Widget Widget
		{
			get { return _box; }
		}

//        private static Alignment GetLeftTopAlignment(Label label)
//        {
//            Alignment leftTopAlignment = new Alignment(0.0f, 0.0f, 0.0f, 0.0f);
//            leftTopAlignment.Add(label);
//            return leftTopAlignment;
//        }

		public object AddWidgetRow(string label, Gtk.Widget widget)
		{
			HBox x = new HBox();
			x.PackStart(new Label( label));
			x.PackStart(widget);
			_box.PackStart(x);
			x.Show();
			return x;

		}

		public object AddLabelRow(string label)
		{
			return AddWidgetRow(label, null);
		}
	}
}

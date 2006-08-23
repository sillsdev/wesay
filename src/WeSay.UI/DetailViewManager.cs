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
			get
			{
				Alignment leftTopAlignment = new Alignment(0.0f, 0.0f, 0.0f, 0.0f);
				 leftTopAlignment.Add(_box);
				return leftTopAlignment;
			}
		}

//        private static Alignment GetLeftTopAlignment(Label label)
//        {
//            Alignment leftTopAlignment = new Alignment(0.0f, 0.0f, 0.0f, 0.0f);
//            leftTopAlignment.Add(label);
//            return leftTopAlignment;
//        }

		public Widget AddWidgetRow(string label, Gtk.Widget widget)
		{
			HBox x = new HBox();
			x.PackStart(new Label( label));
			x.PackStart(widget);
			_box.PackStart(x);
			x.Show();
			x.ShowAll();
			_box.Visible = false;
			_box.Visible = true;
			return x;

		}

		public void ReplaceRow(Widget existing)
		{
		}

		public object AddLabelRow(string label)
		{
			return AddWidgetRow(label, null);
		}
	}
}

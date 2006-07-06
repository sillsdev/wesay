using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
	class WordActionsView : ViewHandler
	{

#pragma warning disable 649
		[Widget]  protected Gtk.VBox _rootVBox;
		[Widget]  protected Gtk.Button  _btnNewWordsToUSB;

#pragma warning restore 649


		public WordActionsView()
		{

			Glade.XML gxml = new Glade.XML("probe.glade", "_actionsViewHolder", null);
			gxml.Autoconnect(this);

			_btnNewWordsToUSB.Clicked += new EventHandler(_btnNewWordsToUSB_Clicked);

			ParentTabControl = s_tabcontrol;
		}



		public Gtk.Notebook ParentTabControl
		{
			set
			{
				_rootVBox.Reparent(value);
				value.SetTabLabelText(_rootVBox, TabLabel);
			}
		}

		public string TabLabel
		{
			get
			{
				return "Admin Action Test";
			}
		}

	   public Container ParentContainer
		{
			set
			{
				_rootVBox.Reparent(value);
			 }
		}

		void _btnNewWordsToUSB_Clicked(object sender, EventArgs e)
		{
			XmlExporter exporter = new XmlExporter(this._model);
			string p = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			p = System.IO.Path.Combine(p, "words.zip");

			Gdk.Cursor busy = new Gdk.Cursor(Gdk.CursorType.Watch);
			this._rootVBox.GdkWindow.Cursor = busy;

			exporter.ExportToZip(p);
			this._rootVBox.GdkWindow.Cursor = null;


			Gtk.MessageDialog x = new MessageDialog(null, Gtk.DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Done saving to "+p, new object[] { });
			x.Run();
			x.Hide();
			x.Dispose();
		}


	}
}

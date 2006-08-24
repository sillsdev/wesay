using System;
using System.Windows.Forms;

namespace WeSay.UI
{
	public interface ITask
	{
		void Activate();
		void Deactivate();
		string Label { get; }
		//Gtk.VBox Container { get;set; }
		Control Control { get;}
	}
}

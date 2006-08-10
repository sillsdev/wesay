using System;
namespace WeSay.UI
{
	public interface ITask
	{
		void Activate();
		void Deactivate();
		string Label { get; }
		Gtk.VBox Container { get;set; }
	}
}

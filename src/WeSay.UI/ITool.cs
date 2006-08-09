using System;
namespace WeSay.UI
{
	public interface ITool
	{
		void Activate();
		void Deactivate();
		string Label { get; }
		Gtk.HBox Container { get;set; }
	}
}

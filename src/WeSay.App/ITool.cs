using System;
namespace WeSay.App
{
	interface ITool
	{
		void Activate();
		void Deactivate();
		string Label { get; }
		Gtk.HBox Container { get;set; }
	}
}

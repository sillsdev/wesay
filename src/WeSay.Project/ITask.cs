using System.Windows.Forms;

namespace WeSay.Project
{
	public interface ITask
	{
		void Activate();
		void Deactivate();
		bool IsActive { get; }
		string Label { get; }
		string Description { get; }

		//Gtk.VBox Container { get;set; }
		Control Control { get;}
		bool IsPinned { get; }
		string Status { get; }
	}
}

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
		string ExactStatus // this may take awhile to get but will be correct (Status may give you nothing if it takes awhile to get)
		{
			get;
		}
	}

	/// <summary>
	/// Used to help the dashboard update its stats after all the other tasks have good counts to give it
	/// </summary>
	public interface IFinishCacheSetup
	{
		void FinishCacheSetup();
	}
}

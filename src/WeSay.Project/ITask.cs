using System;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;

namespace WeSay.Project
{
	public interface ITask : IThingOnDashboard
	{
		void Activate();
		void Deactivate();
		void GoToUrl(string url);
		bool IsActive { get; }
		string Label { get; }
		string Description { get; }

		/// <summary>
		/// most of our tasks, so far, have a lot of caching to do; saying 'true' to this gives them
		/// a chance to be activated while the import (cache building) happens, which is a lot
		/// more efficient than waiting for the user to click on them.
		/// </summary>
		bool MustBeActivatedDuringPreCache { get;}

		Control Control { get;}
		bool IsPinned { get; }
		int GetRemainingCount();

		int ExactCount // this may take awhile to get but will be correct (State may give you nothing if it takes awhile to get)
		{
			get;
		}

		/// <summary>
		/// Gives a sense of the overall size of the task versus what's left to do
		/// </summary>
		int GetReferenceCount();
	}

	/// <summary>
	/// Used to help the dashboard update its stats after all the other tasks have good counts to give it
	/// </summary>
	public interface IFinishCacheSetup
	{
		void FinishCacheSetup();
	}

	public class NavigationException : Exception
	{
		public NavigationException(string message):base(message)
		{

		}
	}
}

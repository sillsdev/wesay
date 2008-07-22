using System;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;

namespace WeSay.Project
{
	/// <summary>
	/// This is the preferred way to activate any indices (caches) your task wants to have installed
	/// during changes to the cache.  THe old method was to actually get activated, which is crazy because
	/// it means we're setting up ui and such, even if wesay is a server, just starting up, running tests, etc.
	/// </summary>
	public interface ISetupIndices
	{
		void RegisterIndicesNow(ViewTemplate viewTemplate);

	}

	public interface ITask : IThingOnDashboard
	{
		void Activate();
		void Deactivate();
		void GoToUrl(string url);
		bool IsActive { get; }
		string Label { get; }

		/// <summary>
		/// ***** Deprecated:  Use ISetupIndices instead, and return false for this.
		/// Most of our tasks, so far, have a lot of caching to do; saying 'true' to this gives them
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

		/// <summary>
		/// Returns whether or not the GetReferenceCount and GetRemainingCount mean anything
		/// </summary>
		bool AreCountsRelevant();

		string GetRemainingCountText();
		string GetReferenceCountText();
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

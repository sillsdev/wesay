using System;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.Project
{
	public interface ITask: IThingOnDashboard
	{
		void Activate();
		void Deactivate();
		bool IsActive { get; }
		string Label { get; }
		bool Available { get; }


		Control Control { get; }
		bool IsPinned { get; }
		int GetRemainingCount();

		int ExactCount
			// this may take awhile to get but will be correct (State may give you nothing if it takes awhile to get)
		{ get; }



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

	public class NavigationException: Exception
	{
		public NavigationException(string message): base(message) {}
	}
}
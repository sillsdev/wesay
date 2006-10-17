using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Foundation
{
		/// <summary>
		/// Delegate for a method which allows the progress range to be reset
		/// </summary>
		public delegate void InitializeProgressCallback(int minimum, int maximum);
		/// <summary>
		/// Delegate for a method which allows the progress to be updated
		/// </summary>
		public delegate void ProgressCallback(int progress);
		/// <summary>
		/// Delegate for a method which allows the status text to be updated
		/// </summary>
		public delegate void StatusCallback(string statusText);

	public interface ILongRunningJob
	{


	}
}

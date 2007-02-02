// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using System;

namespace WeSay.UI
{
	/// <summary>
	/// Summary description for TriggerState.
	/// </summary>
	[Serializable]
	public enum TriggerState : int
	{
		None = 0,
		Show = 1,
		ShowAndConsume = 2,
		Hide = 3,
		HideAndConsume = 4,
		Select = 5,
		SelectAndConsume = 6
	}
}

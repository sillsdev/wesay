// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using System;
using System.Windows.Forms;

namespace WeSay.UI.AutoCompleteTextBox
{
	/// <summary>
	/// Summary description for AutoCompleteTrigger.
	/// </summary>
	[Serializable]
	public abstract class AutoCompleteTrigger
	{
		public virtual TriggerState OnTextChanged(string text)
		{
			return TriggerState.None;
		}

		public virtual TriggerState OnCommandKey(Keys keyData)
		{
			return TriggerState.None;
		}
	}
}

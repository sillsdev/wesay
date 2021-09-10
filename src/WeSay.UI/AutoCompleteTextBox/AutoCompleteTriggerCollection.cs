// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using System;
using System.Collections;
using System.Windows.Forms;

namespace WeSay.UI.AutoCompleteTextBox
{
	/// <summary>
	/// Summary description for AutoCompleteTriggerCollection.
	/// </summary>
	[Serializable]
	public class AutoCompleteTriggerCollection : CollectionBase
	{
		public AutoCompleteTrigger this[int index]
		{
			get { return InnerList[index] as AutoCompleteTrigger; }
		}

		public void Add(AutoCompleteTrigger item)
		{
			InnerList.Add(item);
		}

		public void Remove(AutoCompleteTrigger item)
		{
			InnerList.Remove(item);
		}

		public virtual TriggerState OnTextChanged(string text)
		{
			foreach (AutoCompleteTrigger trigger in InnerList)
			{
				TriggerState state = trigger.OnTextChanged(text);
				if (state != TriggerState.None)
				{
					return state;
				}
			}
			return TriggerState.None;
		}

		public virtual TriggerState OnCommandKey(Keys keyData)
		{
			foreach (AutoCompleteTrigger trigger in InnerList)
			{
				TriggerState state = trigger.OnCommandKey(keyData);
				if (state != TriggerState.None)
				{
					return state;
				}
			}
			return TriggerState.None;
		}
	}
}
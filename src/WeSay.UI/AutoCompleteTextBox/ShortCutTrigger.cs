// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using System;
using System.Windows.Forms;

namespace WeSay.UI.AutoCompleteTextBox
{
	/// <summary>
	/// Summary description for TextLengthTrigger.
	/// </summary>
	[Serializable]
	public class ShortCutTrigger : AutoCompleteTrigger
	{
		private Keys shortCut = Keys.None;
		public Keys ShortCut
		{
			get
			{
				return this.shortCut;
			}
			set
			{
				this.shortCut = value;
			}
		}

		private TriggerState result = TriggerState.None;
		public TriggerState ResultState
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		public ShortCutTrigger()
		{
		}

		public ShortCutTrigger(Keys shortCutKeys, TriggerState resultState)
		{
			this.shortCut = shortCutKeys;
			this.result = resultState;
		}

		public override TriggerState OnCommandKey(Keys keyData)
		{
			if (keyData == ShortCut)
				return ResultState;

			return TriggerState.None;
		}


	}
}

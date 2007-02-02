// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using System;

namespace WeSay.UI
{
	/// <summary>
	/// Summary description for TextLengthTrigger.
	/// </summary>
	[Serializable]
	public class TextLengthTrigger : AutoCompleteTrigger
	{
		private int textLength = 2;
		public int TextLength
		{
			get
			{
				return this.textLength;
			}
			set
			{
				this.textLength = value;
			}
		}

		public TextLengthTrigger()
		{
		}

		public TextLengthTrigger(int length)
		{
			this.textLength = length;
		}

		public override TriggerState OnTextChanged(string text)
		{
			if (text.Length >= this.TextLength)
				return TriggerState.Show;
			else if (text.Length < this.TextLength)
				return TriggerState.Hide;

			return TriggerState.None;
		}


	}
}

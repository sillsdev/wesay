using kmcomapi;

/// If Keyman 7 is not installed, the load of the Keyman Interop Assembly fails. The only way we can
/// catch that is by adding a layer of indirection.
namespace WeSay.UI
{
	class Keyman7 : TavultesoftKeymanClass
	{
		public void UseKeyboard(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return;
			}

			int index = Keyboards.IndexOf(s);
			if (index >= 0)
			{
				Control.ActiveKeyboard = Keyboards[index];
			}
		}

		public void ClearKeyboard()
		{
			Control.ActiveKeyboard = null;
		}
	}
}

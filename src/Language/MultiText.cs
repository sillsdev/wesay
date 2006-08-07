using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace WeSay.Language
{
	/// <summary>
	/// MultiText holds an array of strings, indexed by writing system ID.
	/// These are simple, single language Unicode strings.
	/// </summary>
	public class MultiText
	{
		protected System.Collections.Generic.Dictionary<string, string> _forms;
		public MultiText()
		{
			_forms = new Dictionary<string, string>();
		}

		public string this[string writingSystemId]
		{
			get
			{
				return GetAlternative(writingSystemId);
			}
			set
			{
				SetAlternative(writingSystemId, value);
			}
		}

		public string GetAlternative(string writingSystem)
		{
			if (!_forms.ContainsKey(writingSystem))
				return "";

			string form = _forms[writingSystem];
			if (form == null)
			{
				return "";
			}
			else
				return form;
		}

		public void SetAlternative(string writingSystem, string form)
		{
		   Debug.Assert(writingSystem != null && writingSystem.Length > 0, "The writing system id was empty.");
		   Debug.Assert(writingSystem.Trim() == writingSystem, "The writing system id had leading or trailing whitespace");

			if (form == null || form == "" ) // we don't use space to store empty strings.
			{
				if (_forms.ContainsKey(writingSystem))
				{
					_forms.Remove(writingSystem);
				}
			}

			_forms[writingSystem] = form;
		}

	}
}

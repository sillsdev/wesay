using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureDb4o
{
	class Entry
	{
		public MultiText name;
	}

	class MultiText
	{
		public LanguageForm _singleForm;
		public LanguageForm[] _forms;
		public MultiText()
		{
			_forms = new LanguageForm[2];
		}
	}

	class LanguageForm
	{
		public string _writingSystemId;
		public string _form;

		public LanguageForm(string writingSystemId, string form)
		{
			_writingSystemId = writingSystemId;
			_form = form;
		}
	}
}

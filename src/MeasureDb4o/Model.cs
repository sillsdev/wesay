using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;

namespace MeasureDb4o
{
	class Entry
	{
		public MultiText _name;
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

//        [CLSCompliant(false)]
//        public void ObjectOnActivate(ObjectContainer container)
//        {
//            _writingSystemId = "foo";
//        }
	}
}

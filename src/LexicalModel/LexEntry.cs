using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;
//using WeSay.Base;


namespace WeSay.LexicalModel
{
	public class LexEntry : INotifyPropertyChanged
	{
		private MultiText _lexicalForm;
		private Guid _guid;
		private System.Collections.Generic.List<LexSense> _senses;
		private DateTime _creationDate;
		private DateTime _modifiedDate;

		public event PropertyChangedEventHandler PropertyChanged;

		public LexEntry()
		{
			PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
			_lexicalForm = new MultiText(PropertyChanged);
			_senses = new List<LexSense>();

			//nb: will need to move this if a
			//backend might call this constructor when de-persisting this object, unless it would then reset the field
			_creationDate = DateTime.Now;
			_modifiedDate = _creationDate;
	   }

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_modifiedDate = DateTime.Now;
		}

		public MultiText LexicalForm
		{
			get   {  return _lexicalForm;       }
		}

		public DateTime CreationDate
		{
			get   {  return _creationDate;  }
		}

		public DateTime ModifiedDate
		{
			get { return _modifiedDate; }
		}

		public List<LexSense> Senses
		{
			get   {  return _senses;       }
		}

		/// <summary>
		/// Used to track this entry across programs, for the purpose of merging and such.
		/// </summary>
		public Guid Guid
		{
			get { return _guid; }
			set
			{
				if (_guid  != value)
				{
					_guid  = value;
					NotifyPropertyChanged("GUID");
				}
			}
		}

		private void NotifyPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}

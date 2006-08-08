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
		private BindingList<LexSense> _senses;
		private DateTime _creationDate;
		private DateTime _modifiedDate;

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		public LexEntry()
		{
			PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
			_lexicalForm = new MultiText();
			_lexicalForm.PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);

			_senses = new BindingList<LexSense>();
			//nb:  order of these two is important.  Touching adding new actually triggers ListChanged!
			_senses.AddingNew += new AddingNewEventHandler(OnAddingNewSense);
			_senses.ListChanged += new ListChangedEventHandler(OnListChanged);

			//nb: will need to move this if a
			//backend might call this constructor when de-persisting this object, unless it would then reset the field
			_creationDate = DateTime.Now;
			_modifiedDate = _creationDate;
	   }

		/// <summary>
		/// Create a new LexSense. Called by the binding list when a AddNew() is called on the list.
		/// </summary>
		void OnAddingNewSense(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new LexSense();//this.PropertyChanged)
			((LexSense)(e.NewObject)).PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
		}

		void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}

		/// <summary>
		/// called by the binding list when senses are added, removed, reordered, etc.
		/// </summary>
		void OnListChanged(object sender, ListChangedEventArgs e)
		{
			_modifiedDate = DateTime.Now;
		}

		/// <summary>
		/// called when any field that is part of the enty at some level is changed
		/// </summary>
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

		public BindingList<LexSense> Senses
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

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

using System;
using System.ComponentModel;
using Palaso.Annotations;

namespace WeSay.Foundation.Options
{
	/// <summary>
	/// Used to refer to this option from a field.
	/// This class just wraps the key, which is a string, with various methods to make it fit in
	/// with the system.
	/// </summary>
	public class OptionRef: Annotatable, IParentable, IValueHolder<string>, IReportEmptiness, IReferenceContainer
	{
		private string _humanReadableKey;

		/// <summary>
		/// This "backreference" is used to notify the parent of changes.
		/// IParentable gives access to this during explicit construction.
		/// </summary>
		private IReceivePropertyChangeNotifications _parent;

		private bool _suspendNotification=false;

		public OptionRef() : this(string.Empty)
		{

		}
		public OptionRef(string key) //WeSay.Foundation.WeSayDataObject parent)
		{
			_humanReadableKey = key;
		}
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(Value); }
		}

		#region IParentable Members

		public WeSayDataObject Parent
		{
			set { _parent = value; }
		}

		#endregion

		#region IValueHolder<string> Members

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public string Key
		{
			get{ return Value;}
			set{ Value = value;}
		}
		public string Value
		{
			get { return _humanReadableKey; }
			set
			{
				if(value !=null)
				{
					_humanReadableKey = value.Trim();
				}
				else
				{
					_humanReadableKey = null;
				}
				// this.Guid = value.Guid;
				NotifyPropertyChanged();
			}
		}

		// IReferenceContainer
		public object Target
		{
			get
			{
				// return Lexicon.FindFirstLexEntryMatchingId(_targetId);
				// OptionsList pretend = null;
				//return pretend.GetOptionFromKey(_humanReadableKey);
				throw new NotImplementedException();

			}
			set
			{
				if(value == null && String.IsNullOrEmpty(_humanReadableKey))
				{
					return;
				}

				Option o = value as Option;
				if (o.Key == _humanReadableKey)
				{
					return;
				}

				if (value == null)
				{
					_humanReadableKey = string.Empty;
				}
				else
				{
					_humanReadableKey = o.Key;
				}
				NotifyPropertyChanged();
			}
		}


		#endregion

		private void NotifyPropertyChanged()
		{
			if(_suspendNotification)
			{
				return;
			}
			//tell any data binding
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("option"));
				//todo
			}

			//tell our parent

			if (_parent != null)
			{
				_parent.NotifyPropertyChanged("option");
			}
		}

		#region IReportEmptiness Members

		public bool ShouldHoldUpDeletionOfParentObject
		{
			get { return false; }
		}

		public bool ShouldCountAsFilledForPurposesOfConditionalDisplay
		{
			get { return !IsEmpty; }
		}

		public bool ShouldBeRemovedFromParentDueToEmptiness
		{
			get { return IsEmpty; }
		}


		public void RemoveEmptyStuff()
		{
			if(Value == string.Empty)
			{
				_suspendNotification = true;
				Value = null; // better for matching 'missing' for purposes of missing info task
				_suspendNotification = false;
			}

		}

		#endregion
	}
}
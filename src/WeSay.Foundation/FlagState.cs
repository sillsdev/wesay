using System.ComponentModel;

namespace WeSay.Foundation
{
	/// <summary>
	/// Used to refer to this option from a field
	/// </summary>
	public class FlagState : IParentable, IValueHolder<bool>
	{
		/// <summary>
		/// This "backreference" is used to notify the parent of changes.
		/// IParentable gives access to this during explicit construction.
		/// </summary>
		private WeSayDataObject _parent;

		private bool _isChecked;

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		public FlagState()//WeSay.Foundation.WeSayDataObject parent)
		{
		}

		#region IParentable Members

		public WeSayDataObject Parent
		{
			set
			{
				_parent = value;
			}
		}

		#endregion

		private void NotifyPropertyChanged()
		{
			//tell any data binding
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("checkBox")); //todo
			}

			//tell our parent

			if (_parent != null)
			{
				this._parent.NotifyPropertyChanged("checkBox");
			}
		}

		public bool Value
		{
			get
			{
				return this._isChecked;
			}
			set
			{
				this._isChecked = value;
				// this.Guid = value.Guid;
				NotifyPropertyChanged();
			}
		}



	}
}

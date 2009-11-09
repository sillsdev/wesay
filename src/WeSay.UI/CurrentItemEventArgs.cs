using System;
using System.ComponentModel;
using Palaso.LexicalModel;

namespace WeSay.UI
{
	public class CurrentItemEventArgs: EventArgs
	{
		public PalasoDataObject Parent{ get; set;}
		private readonly string _propertyName;

		public string PropertyName
		{
			get { return _propertyName; }
		}

		private readonly INotifyPropertyChanged _dataTarget;

		public INotifyPropertyChanged DataTarget
		{
			get { return _dataTarget; }
		}

		private readonly string _writingSystemId;

		public string WritingSystemId
		{
			get { return _writingSystemId; }
		}

		public CurrentItemEventArgs(INotifyPropertyChanged dataTarget, string writingSystemId)
		{
			_dataTarget = dataTarget;
			_writingSystemId = writingSystemId;
		}

		public CurrentItemEventArgs(PalasoDataObject parent, string propertyName, string writingSystemId)
		{
			Parent = parent;
			_propertyName = propertyName;
			_writingSystemId = writingSystemId;
		}

		public bool IsGhosted()
		{
			return _propertyName != null;
		}
	}
}
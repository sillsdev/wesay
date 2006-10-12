using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.UI
{
	public class CurrentItemEventArgs : EventArgs
		{
			private string _propertyName;

			public string PropertyName
			{
				get
				{
					return _propertyName;
				}
			 }
			private INotifyPropertyChanged _dataTarget;

			public INotifyPropertyChanged DataTarget
			{
				get
				{
					return _dataTarget;
				}
			}
			private string _writingSystemId;

			public string WritingSystemId
			{
				get
				{
					return _writingSystemId;
				}
			}

//            public CurrentItemEventArgs(INotifyPropertyChanged dataTarget, WritingSystem writingSystem)
//            {
//                _dataTarget = dataTarget;
//                _writingSystemId = writingSystem;
//            }
		public CurrentItemEventArgs(INotifyPropertyChanged dataTarget, string writingSystemId)
			{
				_dataTarget = dataTarget;
				_writingSystemId = writingSystemId;
			}
//            public CurrentItemEventArgs(string propertyName, WritingSystem writingSystem)
//            {
//                _propertyName = propertyName;
//                _writingSystem = writingSystem;
//            }
			public CurrentItemEventArgs(string propertyName, string writingSystemId)
			{
				_propertyName = propertyName;
				_writingSystemId=writingSystemId;
			}
			public bool IsGhosted()
			{
				return _propertyName != null;
			}
		}
	}


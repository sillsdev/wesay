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
			private WritingSystem _writingSystem;

			public WritingSystem WritingSystem
			{
				get
				{
					return _writingSystem;
				}
			}

			public CurrentItemEventArgs(INotifyPropertyChanged dataTarget, WritingSystem writingSystem)
			{
				_dataTarget = dataTarget;
				_writingSystem = writingSystem;
			}
		public CurrentItemEventArgs(INotifyPropertyChanged dataTarget, string writingSystemId)
			{
				_dataTarget = dataTarget;
				_writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
			}
			public CurrentItemEventArgs(string propertyName, WritingSystem writingSystem)
			{
				_propertyName = propertyName;
				_writingSystem = writingSystem;
			}
			public CurrentItemEventArgs(string propertyName, string writingSystemId)
			{
				_propertyName = propertyName;
				_writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
			}
			public bool IsGhosted()
			{
				return _propertyName != null;
			}
		}
	}


using System;
using System.ComponentModel;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.UI
{
	/// <summary>
	/// This simple binding class connects a OptionRef with a SingleOptionControl
	/// Changes in either one are reflected in the other.
	/// </summary>
	public class SingleOptionBinding
	{
		public event EventHandler<CurrentItemEventArgs> CurrentItemChanged = delegate                                                                    {
																				 };
		private INotifyPropertyChanged _dataTarget;
		private SingleOptionControl _widgetTarget;
		private bool _inMidstOfChange;

		public SingleOptionBinding(INotifyPropertyChanged dataTarget,SingleOptionControl widgetTarget)
		{
			_dataTarget = dataTarget;
			_dataTarget.PropertyChanged += new PropertyChangedEventHandler(OnDataPropertyChanged);
			_widgetTarget = widgetTarget;
			_widgetTarget.ValueChanged += new EventHandler(OnWidgetValueChanged);
			_widgetTarget.HandleDestroyed += new EventHandler(_target_HandleDestroyed);
		  //  _widgetTarget.Enter += new EventHandler(OnTextBoxEntered);
		}

		void _target_HandleDestroyed(object sender, EventArgs e)
		{
			TearDown();
		}

//        void OnTextBoxEntered(object sender, EventArgs e)
//        {
//            CurrentItemChanged(sender, new CurrentItemEventArgs(DataTarget, _writingSystemId));
//        }


		void OnWidgetValueChanged(object sender, EventArgs e)
		{
			SetDataTargetValue(_widgetTarget.Value);
		}

		/// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			//Debug.WriteLine(" BindingTearDown boundTo: " + this._widgetTarget.Name);

			if (_dataTarget == null)
			{
				return; //teardown was called twice
			}

			_dataTarget.PropertyChanged -= new PropertyChangedEventHandler(OnDataPropertyChanged);
			_dataTarget = null;
			_widgetTarget.ValueChanged -= new EventHandler(OnWidgetValueChanged);
			_widgetTarget.HandleDestroyed -= new EventHandler(_target_HandleDestroyed);
			_widgetTarget = null;
		}

		/// <summary>
		/// Respond to a change in the data object that we are attached to.
		/// </summary>
		protected virtual void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_inMidstOfChange )
				return;

			try
			{
				_inMidstOfChange = true;
				_widgetTarget.ValueKey = GetTargetValue();
			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		protected string GetTargetValue()
		{
			OptionRef v = _dataTarget as OptionRef;
			if (v == null)
				throw new ArgumentException("Binding can't handle that type of target.");
			return v.Value;
		}

		protected virtual void SetDataTargetValue(Option value)
		{
			if (_inMidstOfChange)
				return;

			try
			{
				_inMidstOfChange = true;

				if (_dataTarget as OptionRef != null)
				{
					OptionRef t = _dataTarget as OptionRef;
					if (t == null)
						throw new ArgumentException("Binding can't handle that type of target.");
					t.Value =value.Name;
				}

				else
					throw new ArgumentException("Binding doesn't understand that type of target.");

			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		public INotifyPropertyChanged DataTarget
		{
			get
			{
				return _dataTarget;
			}
		}

	}
}
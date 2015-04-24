using System;
using System.ComponentModel;
using Palaso.Lift;
using SIL.UiBindings;

namespace WeSay.UI
{
	/// <summary>
	/// This simple binding class connects a IValueHolder<TValueType> (e.g. OptionRef)
	/// with a Widget (e.g. SingleOptionControl)
	/// Changes in either one are reflected in the other.
	/// </summary>
	public class SimpleBinding<TValueType>
	{
		public event EventHandler<CurrentItemEventArgs> CurrentItemChanged = delegate { };

		private IValueHolder<TValueType> _dataTarget;
		private IBindableControl<TValueType> _widget;
		private bool _inMidstOfChange;

		public SimpleBinding(IValueHolder<TValueType> dataTarget,
							 IBindableControl<TValueType> widgetTarget)
		{
			_dataTarget = dataTarget;
			_dataTarget.PropertyChanged += OnDataPropertyChanged;
			_widget = widgetTarget;
			_widget.ValueChanged += OnWidgetValueChanged;
			_widget.GoingAway += _target_HandleDestroyed;

			//Debug.WriteLine("++++++Constructed SimpleBinding boundTo: " + this._widget.Value);
		}

		private void _target_HandleDestroyed(object sender, EventArgs e)
		{
			TearDown();
		}

		//      todo: Make some kind of "infocus" event  void OnTextBoxEntered(object sender, EventArgs e)
		//        {
		//            CurrentItemChanged(sender, new CurrentItemEventArgs(DataTarget, _writingSystemId));
		//        }

		private void OnWidgetValueChanged(object sender, EventArgs e)
		{
#if DEBUG
			if (_widget == null)
			{
				throw new Exception(
						"SimpleBinding called after tearing down (apparently). Did editting the widget make something delete it?  Then perhaps SimpleBinding got the valueCHanged event afterwards... to late!");
			}
#endif
			if (_widget != null) //would not be worth crashing over in release build
			{
				SetDataTargetValue(_widget.Value);
			}
		}

		/// <summary>
		/// Drop our connections to everything so garbage collection can happen and we aren't
		/// a zombie responding to data change events.
		/// </summary>
		private void TearDown()
		{
			//  Debug.WriteLine("-----TearDown SimpleBinding  boundTo: " + this._widget.Value);

			if (_dataTarget == null)
			{
				return; //teardown was called twice
			}

			_dataTarget.PropertyChanged -= OnDataPropertyChanged;
			_dataTarget = null;
			_widget.ValueChanged -= OnWidgetValueChanged;
			_widget.GoingAway -= _target_HandleDestroyed;
			_widget = null;
		}

		/// <summary>
		/// Respond to a change in the data object that we are attached to.
		/// </summary>
		protected virtual void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_inMidstOfChange)
			{
				return;
			}

			try
			{
				_inMidstOfChange = true;
				_widget.Value = GetTargetValue();
			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		protected TValueType GetTargetValue()
		{
			IValueHolder<TValueType> holder = _dataTarget;
			if (holder == null)
			{
				throw new ArgumentException("Binding can't handle that type of target.");
			}
			return holder.Value;
		}

		protected virtual void SetDataTargetValue(TValueType value)
		{
			if (_inMidstOfChange)
			{
				return;
			}

			try
			{
				_inMidstOfChange = true;

				if (_dataTarget == null)
				{
					throw new ArgumentException("Binding found data target null.");
				}
				_dataTarget.Value = value;
			}
			finally
			{
				_inMidstOfChange = false;
			}
		}

		public INotifyPropertyChanged DataTarget
		{
			get { return _dataTarget; }
		}
	}
}
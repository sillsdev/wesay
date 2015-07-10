using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SIL.Lift;
using SIL.UiBindings;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.UI
{
	public partial class ReferenceCollectionEditor<KV, ValueT, KEY_CONTAINER>: FlowLayoutPanel
			where ValueT : class where KEY_CONTAINER : IValueHolder<ValueT>, IReferenceContainer
	{
		private readonly IBindingList _chosenItems;
		private readonly IEnumerable<KV> _sourceChoices;
		private readonly IList<WritingSystemDefinition> _writingSystems;
		private readonly CommonEnumerations.VisibilitySetting _visibility;
		private readonly IChoiceSystemAdaptor<KV, ValueT, KEY_CONTAINER> _choiceSystemAdaptor;
		private readonly IServiceProvider _serviceProvider;
		private IReportEmptiness _alternateEmptinessHelper;
		private AutoCompleteWithCreationBox<KV, ValueT> _emptyPicker;
		private int _popupWidth = -1;
		private bool _ignoreListChanged;
		public event EventHandler<CreateNewArgs> CreateNewTargetItem;

		public ReferenceCollectionEditor()
		{
			InitializeComponent();
		}

#if __MonoCS__
		public override Size GetPreferredSize (Size proposedSize) {
			Size retsize = GetPreferredSizeCore (proposedSize);
			Size maximum_size = MaximumSize;
			Size minimum_size = MinimumSize;
			// If we're bigger than the MaximumSize, fix that
			if (maximum_size.Width != 0 && retsize.Width > maximum_size.Width)
				retsize.Width = maximum_size.Width;
			if (maximum_size.Height != 0 && retsize.Height > maximum_size.Height)
				retsize.Height = maximum_size.Height;

			// If we're smaller than the MinimumSize, fix that
			if (minimum_size.Width != 0 && retsize.Width < minimum_size.Width)
				retsize.Width = minimum_size.Width;
			if (minimum_size.Height != 0 && retsize.Height < minimum_size.Height)
				retsize.Height = minimum_size.Height;

			retsize.Height = Math.Max(20, retsize.Height); // get around Mono problem of collapsing
			return retsize;
		}
		private Size GetPreferredSizeCore(Size proposedSize) {
			int width = 0;
			int height = 0;
			int row_width = 0;
			int row_height = 0;
			int max_width = 10;
			if (proposedSize.Width > 0)
			{
				max_width = proposedSize.Width;
			}
			else if (Parent != null)
			{
				// This is the normal case for sem dom, which is what this is used for in WeSay
				// Adjust for the size of the other controls in this row for the parent.
				// Looked but did not find a good way to reliably do that
				max_width = Parent.DisplayRectangle.Width - 200;
				if (max_width < 0)
				{
					max_width = 10;
				}
			}
			if (MaximumSize.Width > 0)
			{
				max_width = Math.Min(MaximumSize.Width, max_width);
			}
			bool horizontal = FlowDirection == FlowDirection.LeftToRight || FlowDirection == FlowDirection.RightToLeft;
			foreach (Control control in Controls)
			{
				Size control_preferred_size;
				if (control.AutoSize)
					control_preferred_size = control.PreferredSize;
				else
					control_preferred_size = control.Size;
				Padding control_margin = control.Margin;
				if (horizontal)
				{
					int control_width_increase =  control_preferred_size.Width + control_margin.Horizontal;
					if (row_width + control_width_increase > max_width)
					{
						// Start a new row
						row_width = 0;
						height += row_height;
					}
					row_width += control_preferred_size.Width + control_margin.Horizontal;
					row_height = Math.Max(row_height, control_preferred_size.Height + control_margin.Vertical);
					width = Math.Max(width, row_width);
				}
				else
				{
					// Use standard logic for vertical aligned control
					height += control_preferred_size.Height + control_margin.Vertical;
					width = Math.Max(width, control_preferred_size.Width + control_margin.Horizontal);
				}
			}
			height += row_height;
			return new Size (width, height);
		}
#else
		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			size.Height = Math.Max(20, size.Height); // get around Mono problem of collapsing
			return size;
		}
#endif
		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="chosenItems">The set of chosen items we are displaying/editting</param>
		/// <param name="sourceChoices"> The set of objects that the user can choose from. The AutoCompleteAdaptor is used
		/// to convert these into display strings.</param>
		/// <param name="writingSystems">a list of writing systems ordered by preference</param>
		/// <param name="visibility"></param>
		/// <param name="adaptor">does all the conversion between keys, wrappers, actual objects, etc.</param>
		/// <param name="serviceProvider">passed to the AutoCompleteWithCreation so it can get the appropriate type of AutoComplete control</param>
		public ReferenceCollectionEditor(IBindingList chosenItems,
										 IEnumerable<KV> sourceChoices,
										 IList<WritingSystemDefinition> writingSystems,
										 CommonEnumerations.VisibilitySetting visibility,
										 IChoiceSystemAdaptor<KV, ValueT, KEY_CONTAINER> adaptor,
										IServiceProvider serviceProvider)
		{
			if (chosenItems == null)
			{
				throw new ArgumentException("chosenItems");
			}
			if (adaptor == null)
			{
				throw new ArgumentException("adaptor");
			}
			if (writingSystems == null)
			{
				throw new ArgumentException("writingSystems");
			}
			if (sourceChoices == null)
			{
				throw new ArgumentException("sourceChoices");
			}
			InitializeComponent();

			_chosenItems = chosenItems;
			_sourceChoices = sourceChoices;
			_writingSystems = writingSystems;
			_visibility = visibility;
			_choiceSystemAdaptor = adaptor;
			chosenItems.ListChanged += chosenItems_ListChanged;
			BackColorChanged += OnBackColorChanged;
			_serviceProvider = serviceProvider;
		}

		private void OnBackColorChanged(object sender, EventArgs e)
		{
			if (_visibility == CommonEnumerations.VisibilitySetting.ReadOnly)
			{
				foreach (Control control in Controls)
				{
					control.BackColor = BackColor;
				}
			}
		}

		// esa: I don't like this. We are relying on the CreateNewTargetItem
		// event to be wired up before we AddControls since this passes that
		// wiring on. If we just do AddControls in the constructor where
		// it otherwise would be natural, adding a delegate to CreateNewTargetItem
		// has no effect.
		// This waits until the Control has become "real" presumably actually shown
		protected override void OnHandleCreated(EventArgs e)
		{
			AddControls();
			base.OnHandleCreated(e);
		}

		private void chosenItems_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (!_ignoreListChanged && !ContainsFocus)
			{
				AddControls();
			}
		}

		public int PopupWidth
		{
			get { return _popupWidth; }
			set { _popupWidth = value; }
		}

		private void OnChildLostFocus(object sender, EventArgs e)
		{
			// due to the way the popup listbox works, if it has focus,
			// ContainsFocus will not report it, so we check separately.
			bool listBoxFocused = false;
			var box = (IWeSayAutoCompleteTextBox) sender;
			if (box != null)
			{
				listBoxFocused = box.ListBoxFocused;
			}
			if (!(listBoxFocused || ContainsFocus))
					//doing cleanup while the user is in the area will lead to much grief
			{
				IReportEmptiness x = _alternateEmptinessHelper;
				if (x == null)
				{
					x = _chosenItems as IReportEmptiness;
				}
				if (x != null)
				{
					x.RemoveEmptyStuff();
				}
			}
		}

		public IReportEmptiness AlternateEmptinessHelper
		{
			get { return _alternateEmptinessHelper; }
			set { _alternateEmptinessHelper = value; }
		}

		private void AddControls()
		{
			if (DesignMode)
			{
				return;
			}
			SuspendLayout();
			if (Parent != null)
			{
				BackColor = Parent.BackColor;
			}

			Controls.Clear();
			foreach (KEY_CONTAINER item in _chosenItems)
			{
				AutoCompleteWithCreationBox<KV, ValueT> picker = MakePicker();
				picker.Box.Tag = item;
				picker.Box.SelectedItem = _choiceSystemAdaptor.GetKeyValueFromKey_Container(item);
				if (picker.Box.SelectedItem == null) //couldn't find a match for the key
				{
					picker.Box.Text = item.Key;
					// the box will recognize the problem and display a red background
				}

				//the binding itself doesn't need to be "owned" by us... it controls its own lifetime
				new SimpleBinding<ValueT>(item, picker);

				Controls.Add(picker);
			}
			OnBackColorChanged(this, null);
			//set the appropriate background for the pickers if we're readonly

			//add a blank to type in
			if (_visibility != CommonEnumerations.VisibilitySetting.ReadOnly)
			{
				AddEmptyPicker();
			}
			ResumeLayout(false);
			PerformLayout();
		}

		private void AddEmptyPicker()
		{
			_emptyPicker = MakePicker();
			_emptyPicker.ValueChanged += emptyPicker_ValueChanged;
			Controls.Add(_emptyPicker);
		}

		private void emptyPicker_ValueChanged(object sender, EventArgs e)
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker =
					(AutoCompleteWithCreationBox<KV, ValueT>) sender;
			KV kv = (KV) picker.Box.SelectedItem;
			if (kv != null)
			{
				picker.ValueChanged -= emptyPicker_ValueChanged;
				_emptyPicker = null;
				_ignoreListChanged = true;
				KEY_CONTAINER newGuy = (KEY_CONTAINER) _chosenItems.AddNew();
				_choiceSystemAdaptor.UpdateKeyContainerFromKeyValue(kv, newGuy);
				_ignoreListChanged = false;
				picker.Box.Tag = newGuy;

				//the binding itself doesn't need to be "owned" by us... it controls its own lifetime
				new SimpleBinding<ValueT>(newGuy, picker);

				AddEmptyPicker();
			}
		}

		private AutoCompleteWithCreationBox<KV, ValueT> MakePicker()
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker =
					new AutoCompleteWithCreationBox<KV, ValueT>(_visibility, _serviceProvider);
			picker.Box.FormToObjectFinder = _choiceSystemAdaptor.GetValueFromFormNonGeneric;

			picker.Box.WritingSystem = _writingSystems[0];
			picker.GetKeyValueFromValue = _choiceSystemAdaptor.GetKeyValueFromValue;
			picker.GetValueFromKeyValue = _choiceSystemAdaptor.GetValueFromKeyValue;
			picker.Box.ItemDisplayStringAdaptor = _choiceSystemAdaptor;
			picker.Box.Mode = EntryMode.List;
			picker.Box.Items = _sourceChoices;
			picker.Box.MinimumSize = new Size(30, 10);
			picker.Box.ItemFilterer = _choiceSystemAdaptor.GetItemsToOffer;
			picker.Box.PopupWidth = _popupWidth;

			picker.Box.UserLostFocus += OnChildLostFocus;

			if (CreateNewTargetItem != null)
			{
				picker.CreateNewClicked += OnCreateNewClicked;
			}
			return picker;
		}

		internal void OnCreateNewClicked(object sender, CreateNewArgs e)
		{
			if (CreateNewTargetItem != null)
			{
				CreateNewTargetItem.Invoke(this, e);
			}
		}
	}
}
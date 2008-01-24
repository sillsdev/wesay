using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.Language;
using WeSay.UI.AutoCompleteTextBox;


namespace WeSay.UI
{
	public partial class ReferenceCollectionEditor<KV, ValueT, KEY_CONTAINER> : UserControl
		where ValueT :  class
		where KEY_CONTAINER : IValueHolder<ValueT>, IReferenceContainer
	{
		private IBindingList _chosenItems;
		private IEnumerable<KV> _sourceChoices;
		private IList<WritingSystem> _writingSystems;
		private readonly CommonEnumerations.VisibilitySetting _visibility;
		private IChoiceSystemAdaptor<KV,ValueT,KEY_CONTAINER> _choiceSystemAdaptor;
		private IReportEmptiness _alternateEmptinessHelper;

		 private int _popupWidth=-1;
		private bool _ignoreListChanged = false;
		private bool _alreadyInUpdateSize=false;


		public event EventHandler<CreateNewArgs> CreateNewTargetItem;

		public ReferenceCollectionEditor()
		{
			InitializeComponent();
		 }

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="chosenItems">The set of chosen items we are displaying/editting</param>
		 /// <param name="sourceChoices"> The set of objects that the user can choose from. The AutoCompleteAdaptor is used
		 /// to convert these into display strings.</param>
		/// <param name="writingSystems">a list of writing systems ordered by preference</param>
		/// <param name="visibility"></param>
		/// <param name="adaptor">does all the conversion between keys, wrappers, actual objects, etc.</param>
		public ReferenceCollectionEditor(IBindingList chosenItems,
			IEnumerable<KV> sourceChoices,
			IList<WritingSystem> writingSystems,
			CommonEnumerations.VisibilitySetting visibility,
			IChoiceSystemAdaptor<KV,ValueT,KEY_CONTAINER> adaptor)
		{
			if (chosenItems == null)
				throw new ArgumentException("chosenItems");
			if (adaptor == null)
				throw new ArgumentException("adaptor");
			if (writingSystems == null)
				throw new ArgumentException("writingSystems");
			if (sourceChoices == null)
				throw new ArgumentException("sourceChoices");
			InitializeComponent();

			_chosenItems = chosenItems;
			_sourceChoices = sourceChoices;
			_writingSystems = writingSystems;
			_visibility = visibility;
			_choiceSystemAdaptor = adaptor;
			chosenItems.ListChanged += new ListChangedEventHandler(chosenItems_ListChanged);

			UpdateSize();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return _flowPanel.GetPreferredSize(proposedSize);
		}

		public override Size MinimumSize
		{
			get
			{
				return new Size(_flowPanel.MinimumSize.Width, _flowPanel.Height);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			//we determine the width, the flow determines the height
			_flowPanel.MaximumSize = new Size(this.Width, 500);
			_flowPanel.Width = Width;
			UpdateSize();
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			UpdateSize();
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			UpdateSize();
		}

		private void UpdateSize()
		{
			if(_alreadyInUpdateSize)
				return;
			_alreadyInUpdateSize = true;
			Height = _flowPanel.Height;
			_alreadyInUpdateSize = false;
		}

		void chosenItems_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (!_ignoreListChanged && !ContainsFocus)
			{
				OnLoad(this, null);
			}
		}


		public int PopupWidth
		{
			get { return _popupWidth; }
			set { _popupWidth = value; }
		}

		void OnChildLostFocus(object sender, EventArgs e)
		{
			if(!ContainsFocus)//doing cleanup while the user is in the area will lead to much grief
			{
				IReportEmptiness x = _alternateEmptinessHelper;
				if(x==null)
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


		private void OnLoad(object sender, EventArgs e)
		{
			if (DesignMode)
				return;
			SuspendLayout();
			if (Parent != null)
			{
				BackColor = Parent.BackColor;

				_flowPanel.BackColor = BackColor;
			}

			_flowPanel.Controls.Clear();
			foreach (KEY_CONTAINER item in _chosenItems)
			{
				AutoCompleteWithCreationBox<KV, ValueT> picker = MakePicker();
				picker.ValueChanged += new EventHandler(picker_ValueChanged);
				picker.Box.Tag =item;
			   // box.BorderStyle = System.Windows.Forms.BorderStyle.None;

				picker.Box.SelectedItem = _choiceSystemAdaptor.GetKeyValueFromKey_Container(item);

				if(picker.Box.SelectedItem ==null)//couldn't find a match for the key
				{
					picker.Box.Text = item.Key; // the box will recognize the problem and display a red background
				}

				//the binding itself doesn't need to be "owned" by us... it controls its own lifetime
				SimpleBinding<ValueT> binding = new SimpleBinding<ValueT>(item, picker);

				_flowPanel.Controls.Add(picker);

				//review
				picker.Parent = _flowPanel;
			}
			//add a blank to type in
			if(_visibility != CommonEnumerations.VisibilitySetting.ReadOnly)
			{
				AddEmptyPicker();
			}
			ResumeLayout(false);
		}

		void picker_ValueChanged(object sender, EventArgs e)
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = (AutoCompleteWithCreationBox<KV, ValueT>) sender;

			//this is dangerous; a user trying to replace whats there will loose the box they were working in
//            if(picker.Box.SelectedItem == null && string.IsNullOrEmpty(picker.Box.Text))
//            {
//                 KEY_CONTAINER container = (KEY_CONTAINER) picker.Box.Tag;
//                _chosenItems.Remove(container);
//                _flowPanel.Controls.Remove(picker);
//                picker.Dispose();
//            }
		}

		private void AddEmptyPicker()
		{
			AutoCompleteWithCreationBox<KV, ValueT> emptyPicker = MakePicker();
			emptyPicker.ValueChanged += new EventHandler(emptyPicker_ValueChanged);
			_flowPanel.Controls.Add(emptyPicker);

			//review
			emptyPicker.Parent = _flowPanel;
		}

		void emptyPicker_ValueChanged(object sender, EventArgs e)
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = (AutoCompleteWithCreationBox<KV, ValueT>) sender;
			KV kv = (KV) picker.Box.SelectedItem;
			if (kv != null)
			{
				picker.ValueChanged -= emptyPicker_ValueChanged;
				_ignoreListChanged = true;
				KEY_CONTAINER newGuy = (KEY_CONTAINER) _chosenItems.AddNew();
				_choiceSystemAdaptor.UpdateKeyContainerFromKeyValue(kv, newGuy);
				_ignoreListChanged = false;
				picker.ValueChanged += picker_ValueChanged;

				//the binding itself doesn't need to be "owned" by us... it controls its own lifetime
				SimpleBinding<ValueT> binding = new SimpleBinding<ValueT>(newGuy, picker);

				AddEmptyPicker();
			}
		}

		private AutoCompleteWithCreationBox<KV, ValueT> MakePicker()
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = new AutoCompleteWithCreationBox<KV, ValueT>(_visibility);
			picker.Box.FormToObectFinder = _choiceSystemAdaptor.GetValueFromFormNonGeneric;

			picker.GetKeyValueFromValue = _choiceSystemAdaptor.GetKeyValueFromValue;
			picker.GetValueFromKeyValue = _choiceSystemAdaptor.GetValueFromKeyValue;// _getValueFromKeyValueDelegate;
			picker.Box.ItemDisplayStringAdaptor = _choiceSystemAdaptor;

			//picker.Box.SelectedItemChanged += new EventHandler(OnSelectedItemChanged);

			if(_choiceSystemAdaptor != null)
			{
				picker.Box.ItemDisplayStringAdaptor = _choiceSystemAdaptor;
			}
			//picker.Box.TooltipToDisplayStringAdaptor = _choiceSystemAdaptor.ToolTipAdaptor;
			picker.Box.PopupWidth = 100;
			picker.Box.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
			picker.Box.Items = _sourceChoices;
			picker.Box.WritingSystem = _writingSystems[0];
			picker.Box.MinimumSize = new Size(20, 10);
			picker.Box.ItemFilterer = _choiceSystemAdaptor.GetItemsToOffer;
			picker.Box.PopupWidth = _popupWidth;

			picker.Box.LostFocus   += new EventHandler(OnChildLostFocus);

			if (CreateNewTargetItem != null)
			{
				picker.CreateNewClicked += OnCreateNewClicked;
			}
			return picker;
		}

		internal void OnCreateNewClicked(object sender, CreateNewArgs e)
		{
			if (this.CreateNewTargetItem!=null)
			{
				CreateNewTargetItem.Invoke(this, e);
			}
		}

		private class LexemeInfoProvider : IDisplayStringAdaptor
		{
			public string GetDisplayLabel(object item)
			{
				return "pretend: "+item.ToString();
			}

			#region IDisplayStringAdaptor Members

			public string GetToolTip(object item)
			{
				return "";
			}

			public string GetToolTipTitle(object item)
			{
				return "";
			}

			#endregion
		}

	}


}

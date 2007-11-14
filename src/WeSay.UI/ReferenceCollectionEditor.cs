using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.UI.AutoCompleteTextBox;


namespace WeSay.UI
{
	public partial class ReferenceCollectionEditor<KV, ValueT, KEY_CONTAINER> : UserControl
		where ValueT :  class /*, IReferenceContainer,*/ /*IValueHolder<KV>*/
		where KEY_CONTAINER : IValueHolder<ValueT>, IReferenceContainer
	  //  where CHOICE : IChoice
	{

		private IBindingList _chosenItems;
		private IEnumerable<KV> _sourceChoices;
		private IList<WritingSystem> _writingSystems;

		//-------------------------------------------------
		#region Delegates and Events
		public event EventHandler CollectionChanged;
		private WeSay.Foundation.IDisplayStringAdaptor _displayStringAdaptor;
		private WeSayAutoCompleteTextBox.FormToObectFinderDelegate _formToObectFinderDelegate;

		private IDisplayStringAdaptor _toolTipAdaptor;
		AutoCompleteWithCreationBox<KV, ValueT>.GetValueFromKeyValueDelegate _getValueFromKeyValueDelegate;
		private AutoCompleteWithCreationBox<KV, ValueT>.GetKeyValueFromValueDelegate _getKeyValueFromValueDelegate;

		public delegate KV GetKeyValueFromKey_ContainerDelegate(KEY_CONTAINER kc);
		private GetKeyValueFromKey_ContainerDelegate _getKeyValueFromKey_Container;

		public delegate void UpdateKeyContainerFromKeyValueDelegate(KV kv, KEY_CONTAINER kc);

		private UpdateKeyContainerFromKeyValueDelegate _updateKeyContainerFromKeyValue;
		#endregion
		//-------------------------------------------------

		public AutoCompleteWithCreationBox<KV, ValueT>.GetValueFromKeyValueDelegate GetValueFromKeyValue
		{
			get
			{
				return this._getValueFromKeyValueDelegate;
			}
			set
			{
				this._getValueFromKeyValueDelegate = value;
			}
		}


		public AutoCompleteWithCreationBox<KV, ValueT>.GetKeyValueFromValueDelegate GetKeyValueFromValue
		{
			get
			{
				return this._getKeyValueFromValueDelegate;
			}
			set
			{
				this._getKeyValueFromValueDelegate = value;
			}
		}


		public event EventHandler<CreateNewArgs> CreateNewTargetItem;

		public ReferenceCollectionEditor()
		{
			InitializeComponent();
		 }

		public ReferenceCollectionEditor(IBindingList collection, IList<WritingSystem> writingSystems)//, IEnumerable<IChoice> choices)
		{
			InitializeComponent();
			_chosenItems = collection;
			_writingSystems = writingSystems;
			if (_writingSystems == null)
				throw new ArgumentException("writingSystems");
		   // _sourceChoices = choices;
		}


		public IBindingList CollectionBeingEdited
		{
			get
			{
				return _chosenItems;
			}
			set
			{
				_chosenItems = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WeSay.Foundation.IDisplayStringAdaptor ItemDisplayStringAdaptor
		{
			get
			{
				return _displayStringAdaptor;
			}
			set
			{
				_displayStringAdaptor = value;
			}
		}


		/// <summary>
		/// The set of objects that the user can choose from. The AutoCompleteAdaptor is used
		/// to convert these into display strings.
		/// </summary>
		public IEnumerable<KV> SourceChoices
		{
			get
			{
				return _sourceChoices;
			}
			set
			{
				_sourceChoices = value;
			}
		}

		public WeSayAutoCompleteTextBox.FormToObectFinderDelegate FormToObectFinder
		{
			get
			{
				return _formToObectFinderDelegate;
			}
			set
			{
				_formToObectFinderDelegate = value;
			}
		}

		public GetKeyValueFromKey_ContainerDelegate GetKeyValueFromKey_Container
		{
			get { return _getKeyValueFromKey_Container; }
			set { _getKeyValueFromKey_Container = value; }
		}

		public UpdateKeyContainerFromKeyValueDelegate UpdateKeyContainerFromKeyValue
		{
			get { return _updateKeyContainerFromKeyValue; }
			set { _updateKeyContainerFromKeyValue = value; }
		}

		private void OnLoad(object sender, EventArgs e)
		{
			if(DesignMode)
				return;
			_flowPanel.Controls.Clear();
		  //  IDisplayStringAdaptor toolTipAdaptor = new LexemeInfoProvider();
			foreach (KEY_CONTAINER item in _chosenItems)
			{
				AutoCompleteWithCreationBox<KV, ValueT> picker = MakePicker();
				picker.ValueChanged += new EventHandler(picker_ValueChanged);
				picker.Box.Tag =item;
			   // box.BorderStyle = System.Windows.Forms.BorderStyle.None;

				picker.Box.SelectedItem = GetKeyValueFromKey_Container(item);

				if(picker.Box.SelectedItem ==null)//couldn't find a match for the key
				{
					picker.Box.Text = item.Key; // the box will recognize the problem and display a red background
				}
				SimpleBinding<ValueT> binding = new SimpleBinding<ValueT>(item, picker);

				_flowPanel.Controls.Add(picker);
			}
			//add a blank to type in
			AddEmptyPicker();
		}

		void picker_ValueChanged(object sender, EventArgs e)
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = (AutoCompleteWithCreationBox<KV, ValueT>) sender;
			if(picker.Box.SelectedItem == null && string.IsNullOrEmpty(picker.Box.Text))
			{
				 KEY_CONTAINER container = (KEY_CONTAINER) picker.Box.Tag;
				_chosenItems.Remove(container);
				_flowPanel.Controls.Remove(picker);
			}
		}

		private void AddEmptyPicker()
		{
			AutoCompleteWithCreationBox<KV, ValueT> emptyPicker = MakePicker();
			emptyPicker.ValueChanged += new EventHandler(emptyPicker_ValueChanged);
			_flowPanel.Controls.Add(emptyPicker);
		}

		void emptyPicker_ValueChanged(object sender, EventArgs e)
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = (AutoCompleteWithCreationBox<KV, ValueT>) sender;
			KV kv = (KV) picker.Box.SelectedItem;
			if (kv != null)
			{
				picker.ValueChanged -= emptyPicker_ValueChanged;
				KEY_CONTAINER newGuy = (KEY_CONTAINER) _chosenItems.AddNew();
				UpdateKeyContainerFromKeyValue(kv, newGuy);
				picker.ValueChanged += picker_ValueChanged;
				AddEmptyPicker();
			}
		}

		private AutoCompleteWithCreationBox<KV, ValueT> MakePicker()
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = new AutoCompleteWithCreationBox<KV, ValueT>();
			if(_formToObectFinderDelegate !=null)
			{
				picker.Box.FormToObectFinder = _formToObectFinderDelegate;
			}

			picker.GetKeyValueFromValue = _getKeyValueFromValueDelegate;
		   picker.GetValueFromKeyValue = _getValueFromKeyValueDelegate;
			picker.Box.ItemDisplayStringAdaptor = _displayStringAdaptor;

			//picker.Box.SelectedItemChanged += new EventHandler(OnSelectedItemChanged);

			if(_displayStringAdaptor != null)
			{
				picker.Box.ItemDisplayStringAdaptor = _displayStringAdaptor;
			}
			picker.Box.TooltipToDisplayStringAdaptor = _toolTipAdaptor;
			picker.Box.PopupWidth = 100;
			picker.Box.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
			picker.Box.Items = SourceChoices;
			picker.Box.WritingSystem = _writingSystems[0];
			picker.Box.MinimumSize = new Size(20, 10);
			if (CreateNewTargetItem != null)
			{
				picker.CreateNewClicked += new EventHandler<CreateNewArgs>(OnCreateNewClicked);
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
		}

//        void OnSelectedItemChanged(object sender, EventArgs e)
//        {
//            WeSayAutoCompleteTextBox box = sender as WeSayAutoCompleteTextBox;
//            ValueT referer = box.Tag as ValueT;
//            if(referer==null)
//            {
//                return;//referer = box.Tag = this. //TODO need a delegate to add chosen item
//            }
//            if (box.SelectedItem == null)
//            {
//
//            }
//            else
//            {
//                referer.Target = box.SelectedItem;
//            }
//
//
//            if (CollectionChanged != null)
//            {
//                CollectionChanged.Invoke(this, null);
//            }
//        }


	}


}

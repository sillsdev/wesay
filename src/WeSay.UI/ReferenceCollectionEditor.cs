using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
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
		private IChoiceSystemAdaptor<KV,ValueT,KEY_CONTAINER> _choiceSystemAdaptor;

		//-------------------------------------------------
		#region Delegates and Events
		private WeSayAutoCompleteTextBox.FormToObectFinderDelegate _formToObectFinderDelegate;

		#endregion


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
		/// <param name="adaptor">does all the conversion between keys, wrappers, actual objects, etc.</param>
		public ReferenceCollectionEditor(IBindingList chosenItems,
			IEnumerable<KV> sourceChoices,
			IList<WritingSystem> writingSystems,
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
			_choiceSystemAdaptor = adaptor;
	   }


		private void OnLoad(object sender, EventArgs e)
		{
			if(DesignMode)
				return;
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
				_choiceSystemAdaptor.UpdateKeyContainerFromKeyValue(kv, newGuy);
				picker.ValueChanged += picker_ValueChanged;
				AddEmptyPicker();
			}
		}

		private AutoCompleteWithCreationBox<KV, ValueT> MakePicker()
		{
			AutoCompleteWithCreationBox<KV, ValueT> picker = new AutoCompleteWithCreationBox<KV, ValueT>();
			if(_formToObectFinderDelegate !=null)
			{
				picker.Box.FormToObectFinder = _choiceSystemAdaptor.GetValueFromForm;
			}

			picker.GetKeyValueFromValue = _choiceSystemAdaptor.GetKeyValueFromValue;
			picker.GetValueFromKeyValue = _choiceSystemAdaptor.GetValueFromKeyValue;// _getValueFromKeyValueDelegate;
			picker.Box.ItemDisplayStringAdaptor = _choiceSystemAdaptor;

			//picker.Box.SelectedItemChanged += new EventHandler(OnSelectedItemChanged);

			if(_choiceSystemAdaptor != null)
			{
				picker.Box.ItemDisplayStringAdaptor = _choiceSystemAdaptor;
			}
			picker.Box.TooltipToDisplayStringAdaptor = _choiceSystemAdaptor.ToolTipAdaptor;
			picker.Box.PopupWidth = 100;
			picker.Box.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
			picker.Box.Items = _sourceChoices;
			picker.Box.WritingSystem = _writingSystems[0];
			picker.Box.MinimumSize = new Size(20, 10);
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
		}

	}


}

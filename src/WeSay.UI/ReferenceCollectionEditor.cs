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
	public partial class ReferenceCollectionEditor<CONTAINER> : UserControl
		where CONTAINER :  class, IReferenceContainer
	{

		private IList<CONTAINER> _collection;
		private IList<string> _writingSystemIds;
		private WeSay.Foundation.IDisplayStringAdaptor _displayStringAdaptor;
		private IEnumerable<object> _choices;
		private WeSayAutoCompleteTextBox.FormToObectFinderDelegate _formToObectFinderDelegate;

		public event EventHandler CollectionChanged;


		public event EventHandler<CreateNewArgs> CreateNewTargetItem;

		public ReferenceCollectionEditor()
		{
			InitializeComponent();
		 }

		public ReferenceCollectionEditor(IList<CONTAINER> collection, IList<string> writingSystemIds)//, IEnumerable<IChoice> choices)
		{
			InitializeComponent();
			_collection = collection;
			_writingSystemIds = writingSystemIds;
		   // _choices = choices;
		}

		public interface IChoice
		{
			string Label
			{
				get;
			}
			string Key
			{
				get;
			}
		}

		public IList<CONTAINER> Collection
		{
			get
			{
				return _collection;
			}
			set
			{
				_collection = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WeSay.Foundation.IDisplayStringAdaptor DisplayStringAdaptor
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
		public IEnumerable<object> Choices
		{
			get
			{
				return _choices;
			}
			set
			{
				_choices = value;
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

		private void OnLoad(object sender, EventArgs e)
		{
			if(DesignMode)
				return;
			_flowPanel.Controls.Clear();
			IDisplayStringAdaptor toolTipAdaptor = new LexemeInfoProvider();
			WritingSystem ws = new WeSay.Language.WritingSystem();
			foreach (CONTAINER referenceContainer in _collection)
			{
				AutoCompleteWithCreationBox<CONTAINER,CONTAINER> picker = new AutoCompleteWithCreationBox<CONTAINER,CONTAINER>();
				if(_formToObectFinderDelegate !=null)
				{
					picker.Box.FormToObectFinder = _formToObectFinderDelegate;
				}

				picker.Box.SelectedItemChanged += new EventHandler(OnSelectedItemChanged);

				picker.Box.ItemDisplayStringAdaptor = _displayStringAdaptor;
				picker.Box.TooltipToDisplayStringAdaptor = toolTipAdaptor;
				picker.Box.PopupWidth = 100;
				picker.Box.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
				picker.Box.Tag =referenceContainer;
				picker.Box.Items = Choices;
				picker.Box.WritingSystem = ws;
				picker.Box.MinimumSize = new Size(20, 10);
			   // box.BorderStyle = System.Windows.Forms.BorderStyle.None;
				picker.Box.SelectedItem = referenceContainer.Target; //todo: change this
				picker.CreateNewClicked += new EventHandler<CreateNewArgs>(picker_CreateNewClicked);
				_flowPanel.Controls.Add(picker);
			}
		}

		void picker_CreateNewClicked(object sender, CreateNewArgs e)
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

		void OnSelectedItemChanged(object sender, EventArgs e)
		{
			WeSayAutoCompleteTextBox box = sender as WeSayAutoCompleteTextBox;
			CONTAINER relation = (CONTAINER)box.Tag ;
			if (box.SelectedItem == null)
			{

			}
			else
			{
				relation.Target = box.SelectedItem;
			}


			if (CollectionChanged != null)
			{
				CollectionChanged.Invoke(this, null);
			}
		}


	}
}

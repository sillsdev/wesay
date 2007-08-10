using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeSay.Foundation;

namespace RelationControlTestApp
{
	public partial class Form1 : Form
	{
		private WeSay.UI.ReferenceCollectionEditor<PretendReference> _referenceCollectionControl;

		/// <summary>
		/// E.g. this would be the list of LexEntries in the dictionary
		/// </summary>
		private List<object> _potentialTargets = new List<object>();

		public Form1()
		{
			InitializeComponent();
			if(DesignMode)
				return;

			this._referenceCollectionControl = new WeSay.UI.ReferenceCollectionEditor<PretendReference>();
			this.Controls.Add(this._referenceCollectionControl);
			_placeholder.Visible = false;
			//
			// lexRelationControl1
			//
			this._referenceCollectionControl.Anchor = _placeholder.Anchor;

			this._referenceCollectionControl.BackColor = System.Drawing.Color.NavajoWhite;
			this._referenceCollectionControl.Choices = null;
			this._referenceCollectionControl.Location = _placeholder.Location;
			this._referenceCollectionControl.Name = "_referenceCollectionControl";
			this._referenceCollectionControl.Collection = null;
			this._referenceCollectionControl.Size = _placeholder.Size;
			this._referenceCollectionControl.TabIndex = 0;

			_referenceCollectionControl.CreateNewTargetItem += new EventHandler<WeSay.UI.AutoCompleteWithCreationBox.CreateNewArgs>(OnCreateNewTargetItem);

			_referenceCollectionControl.Collection = new List<PretendReference>();
			_referenceCollectionControl.DisplayStringAdaptor = new TestLabelAdaptor();
			AddReference(AddTargetObject("IDone", "one"));
			AddTargetObject("IDtwo", "two");
			AddReference(AddTargetObject("IDthree", "three"));
			AddTargetObject("IDfour", "four");

			_referenceCollectionControl.Choices = _potentialTargets;

			_referenceCollectionControl.CollectionChanged += new EventHandler(lexRelationControl1_CollectionChanged);
			LoadListViews();
		}
		private PretendTarget AddTargetObject(string id, string label)
		{
			//LexRelation one = new LexRelation("baseEntry", id);
			PretendTarget item = new PretendTarget(id, label);
			_potentialTargets.Add(item);
			return item;
		}

		private void AddReference(PretendTarget target)
		{
			//LexRelation one = new LexRelation("baseEntry", id);
			_referenceCollectionControl.Collection.Add(new PretendReference(target));
		}

	   void OnCreateNewTargetItem(object sender, WeSay.UI.AutoCompleteWithCreationBox.CreateNewArgs e)
		{
		   e.NewlyCreatedItem = AddTargetObject(e.LabelOfNewItem+"ID", e.LabelOfNewItem);
		   LoadListViews();
		}

		void lexRelationControl1_CollectionChanged(object sender, EventArgs e)
		{
			LoadListViews();
		}



		class TestLabelAdaptor : WeSay.Foundation.IDisplayStringAdaptor
		{
			public string GetDisplayLabel(object item)
			{
				return ((PretendTarget) item)._label;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			LoadListViews();
		}

		private void LoadListViews()
		{
			_referenceListView.Items.Clear();
			foreach (PretendReference reference in _referenceCollectionControl.Collection)
			{
				ListViewItem item = new ListViewItem(reference.Target.ToString());
				item.SubItems.Add(_referenceCollectionControl.DisplayStringAdaptor.GetDisplayLabel( reference.Target));
				_referenceListView.Items.Add(item);
			}


			_potentialTargetsListView.Items.Clear();
			foreach (PretendTarget target in _potentialTargets)
			{
				ListViewItem item = new ListViewItem(target._key);
				item.SubItems.Add(target._label);
				_potentialTargetsListView.Items.Add(item);
			}
		}

		private void label2_Click(object sender, EventArgs e)
		{

		}
	}

	public class PretendTarget
	{
		public string _key;
		public string _label;

		public PretendTarget(string key, string label)
		{
			_key = key;
			_label = label;
		}
	}

	public class PretendReference : IReferenceContainer
	{
		private PretendTarget _target;

		public PretendReference(PretendTarget target)
		{
			_target = target;
		}

		public object Target
		{
			get
			{
				return _target;
			}

			set
			{
				_target = (PretendTarget) value;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// A Layouter is responsible for filling a detailed list with the contents
	/// of a single data object (e.g. LexSense, LexExample), etc.
	/// There are will normally be a single subclass per class of data,
	/// and each of these layout erstwhile call a different layouter for each
	/// child object (e.g. LexEntryLayouter would employ a SenseLayouter to display senses).
	/// </summary>
	public abstract class Layouter
	{
		/// <summary>
		/// The DetailList we are filling.
		/// </summary>
	  private DetailList _detailList;

		private readonly FieldInventory _fieldInventory;

		protected DetailList DetailList
	  {
		get
		{
		  return _detailList;
		}
		set
		{
		  _detailList = value;
		}
	  }

		public FieldInventory FieldInventory
		{
			get { return this._fieldInventory; }
		}

		protected Layouter(DetailList builder, FieldInventory fieldInventory)
		{
			if(builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			if(fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_detailList = builder;
			_fieldInventory = fieldInventory;
		}

		/// <summary>
		/// actually add the widget's that are needed to the detailed list
		/// </summary>
		public abstract int AddWidgets(IBindingList list, int index);

		internal abstract int AddWidgets(IBindingList list, int index, int row);

		protected Control MakeBoundEntry(WeSay.Language.MultiText text, Field field)
		{
			MultiTextControl m = new MultiTextControl(field.WritingSystemIds, text, field.FieldName);

			foreach (WeSayTextBox box in m.TextBoxes)
			{
				 WeSay.UI.Binding binding = new WeSay.UI.Binding(text, box.WritingSystem.Id, box);
				 binding.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(_detailList.OnBindingCurrentItemChanged);
			}
		   return m;
		}


//        protected Control MakeGhostEntry(IBindingList list, string ghostPropertyName, IList<String> writingSystemIds)
//        {
//            WeSayMultiText m = new WeSayMultiText(writingSystemIds, new MultiText());
////
////            foreach (WeSayTextBox box in m.TextBoxes)
////            {
////                MakeGhostBinding(list, ghostPropertyName, box.WritingSystem, box);
////            }
////            GhostBinding g = MakeGhostBinding(list, "Sentence", writingSystem, entry);
////            //!!!!!!!!!!!!!!!!! g.ReferenceControl =
////                DetailList.AddWidgetRow(StringCatalog.Get("New Example"), false, entry, insertAtRow+rowCount);
////
//////            WeSayTextBox entry = new WeSayTextBox(writingSystem);
////            MakeGhostBinding(list, ghostPropertyName, writingSystem, entry);
//            return m;
//        }

		protected int MakeGhostWidget(IBindingList list, int insertAtRow, string fieldName, string label, string propertyName)
		{
			int rowCount = 0;
			Field field = FieldInventory.GetField(fieldName);
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{

				MultiTextControl m = new MultiTextControl(field.WritingSystemIds, new MultiText());
				Control refWidget = DetailList.AddWidgetRow(StringCatalog.Get(label), false, m, insertAtRow + rowCount);

				foreach (WeSayTextBox box in m.TextBoxes)
				{
					GhostBinding g = MakeGhostBinding(list, propertyName, box.WritingSystem, box);
					g.ReferenceControl = refWidget;
				}
				return 1;
			}
			else
			{
				return 0; //didn't add a row
			}

		}

		protected GhostBinding MakeGhostBinding(IBindingList list, string ghostPropertyName, WritingSystem writingSystem,
			WeSayTextBox entry)
		{
			WeSay.UI.GhostBinding binding = new WeSay.UI.GhostBinding(list, ghostPropertyName, writingSystem, entry);
			binding.Triggered += new GhostBinding.GhostTriggered(OnGhostBindingTriggered);
			binding.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(_detailList.OnBindingCurrentItemChanged);
			return binding;
		}

		protected virtual void OnGhostBindingTriggered(GhostBinding sender, IBindingList list, int index, System.EventArgs args)
		{
			AddWidgetsAfterGhostTrigger(list, index, sender.ReferenceControl);
		}

		protected void AddWidgetsAfterGhostTrigger(IBindingList list, int index, Control refControl)
		{
			int row    = _detailList.GetRowOfControl(refControl);
			AddWidgets(list, index, row);
			_detailList.MoveInsertionPoint(row);
		}

		protected static int AddChildrenWidgets(Layouter layouter, IBindingList list, int insertAtRow , int rowCount)
		{
			int r;

			for (int i = 0; i < list.Count; i++)
			{
			  if (insertAtRow < 0)
			  {
				r = insertAtRow;    // just stick at the end
			  }
			  else
			  {
				r = insertAtRow + rowCount;
			  }
			  rowCount += layouter.AddWidgets(list, i, r);
			}
			return rowCount;
		}

	}
}

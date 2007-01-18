using System;
using System.ComponentModel;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
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

		private readonly ViewTemplate _viewTemplate;

		/// This field is for temporarily storing a ghost field about to become "real".
		/// This is critical, though messy, because
		/// otherwise, the first character typed into a ghost text field is not considered by
		/// Keyman, since switching focus to a new, "real" box clears Keymans history of your typing.
		/// See WS-23 for more info.
		private MultiTextControl _previouslyGhostedControlToReuse=null;

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

		public ViewTemplate ActiveViewTemplate
		{
			get { return this._viewTemplate; }
		}

		protected Layouter(DetailList builder, ViewTemplate viewTemplate)
		{
			if(builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			if(viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_detailList = builder;
			_viewTemplate = viewTemplate;
		}

		/// <summary>
		/// actually add the widgets that are needed to the detailed list
		/// </summary>
		public  int AddWidgets(IBindingList list, int index)
		{
			return AddWidgets(list, index, -1);
		}
		internal abstract int AddWidgets(IBindingList list, int index, int row);


		protected Control MakeOptionWidget(WeSay.Foundation.OptionRef optionToBindTo, Field field)
		{
			SingleOptionControl c = new SingleOptionControl(optionToBindTo);
			//TODO            BindOptionControlToField(c, optionToBindTo);
			return c;
		}

		protected Control MakeBoundEntry(WeSay.Language.MultiText multiTextToBindTo, Field field)
		{
			MultiTextControl m=null;
			if (_previouslyGhostedControlToReuse == null)
			{
				m = new MultiTextControl(field.WritingSystems, multiTextToBindTo, field.FieldName);
			}
			else
			{
				m = _previouslyGhostedControlToReuse;
				_previouslyGhostedControlToReuse = null;
			}

			BindMultiTextControlToField(m, multiTextToBindTo);
			return m;
		}

		private void BindMultiTextControlToField(MultiTextControl control, MultiText multiTextToBindTo)
		{
			foreach (WeSayTextBox box in control.TextBoxes)
			{
				WeSay.UI.Binding binding = new WeSay.UI.Binding(multiTextToBindTo, box.WritingSystem.Id, box);
				binding.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(_detailList.OnBindingCurrentItemChanged);
			}
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
////                DetailList.AddWidgetRow(StringCatalog.GetListOfType("New Example"), false, entry, insertAtRow+rowCount);
////
//////            WeSayTextBox entry = new WeSayTextBox(writingSystem);
////            MakeGhostBinding(list, ghostPropertyName, writingSystem, entry);
//            return m;
//        }

		protected int MakeGhostWidget(IBindingList list, int insertAtRow, string fieldName, string label, string propertyName)
		{
			int rowCount = 0;
			Field field = ActiveViewTemplate.GetField(fieldName);
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{

				MultiTextControl m = new MultiTextControl(field.WritingSystems, new MultiText(), fieldName+"_ghost");
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

		protected virtual void OnGhostBindingTriggered(GhostBinding sender, IBindingList list, int index, MultiTextControl previouslyGhostedControlToReuse, System.EventArgs args)
		{
			_previouslyGhostedControlToReuse = previouslyGhostedControlToReuse;
			AddWidgetsAfterGhostTrigger(list, index, sender.ReferenceControl);
		}

		protected void AddWidgetsAfterGhostTrigger(IBindingList list, int index, Control refControl)
		{
			int row    = _detailList.GetRowOfControl(refControl);
			AddWidgets(list, index, row);
			_detailList.MoveInsertionPoint(row+1);
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

		protected int AddCustomFields(WeSayDataObject target, int insertAtRow)
		{
			int rowCount = 0;
			foreach (Field customField in ActiveViewTemplate.GetCustomFields(target.GetType().Name))
			{
				Control box = null;
				switch (customField.DataTypeName)
				{
					case "Option":
						box = MakeOptionWidget(target.GetProperty<WeSay.Foundation.OptionRef>(customField.FieldName), customField);
						break;
					case "MultiText":
						 box = MakeBoundEntry(target.GetProperty<MultiText>(customField.FieldName), customField);
					 break;
					default:
						throw new ApplicationException(
							string.Format("WeSay doesn't understand how to a layout a {0}", customField.DataTypeName));
				}
				DetailList.AddWidgetRow(StringCatalog.Get(customField.DisplayName), true, box, insertAtRow);

				++rowCount;
			}
			return rowCount;
		}
	}
}

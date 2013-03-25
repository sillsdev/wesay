using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.UiBindings;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalModel.Foundation.Options;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;
using Palaso.i18n;

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

		private DetailList _parentDetailList;

		private bool _deletable;

		/// <summary>
		/// Use for establishing relations been this entry and the rest
		/// </summary>
		private readonly LexEntryRepository _lexEntryRepository;

		private readonly ViewTemplate _viewTemplate;

		protected IServiceProvider _serviceProvider;

		/// This field is for temporarily storing a ghost field about to become "real".
		/// This is critical, though messy, because
		/// otherwise, the first character typed into a ghost text field is not considered by
		/// Keyman, since switching focus to a new, "real" box clears Keymans history of your typing.
		/// See WS-23 for more info.
		private MultiTextControl _previouslyGhostedControlToReuse;

		private bool _showNormallyHiddenFields;

		private readonly DeleteButton _deleteButton = new DeleteButton();

		public EventHandler DeleteClicked;

		public bool Deletable
		{
			get { return _deletable; }
			set
			{
				if (value == _deletable)
				{
					return;
				}
				_deletable = value;
				_deleteButton.Visible = _deletable;
				_deleteButton.Active = DetailList.MouseIsInBounds;
			}
		}

		private void OnMouseLeftBounds(object sender, EventArgs e)
		{
			_deleteButton.Active = false;
		}

		private void OnMouseEnteredBounds(object sender, EventArgs e)
		{
			if (Deletable)
			{
				_deleteButton.Active = true;
			}
		}

		private void OnDeleteClicked(object sender, EventArgs e)
		{
			if (DeleteClicked != null)
			{
				DeleteClicked(this, e);
			}
		}

		protected DetailList DetailList
		{
			get { return _detailList; }
			set { _detailList = value; }
		}

		public ViewTemplate ActiveViewTemplate
		{
			get { return _viewTemplate; }
		}

		/// <summary>
		/// Use for establishing relations been this entry and the rest
		/// </summary>
		protected LexEntryRepository RecordListManager
		{
			get { return _lexEntryRepository; }
		}

		public bool ShowNormallyHiddenFields
		{
			get { return _showNormallyHiddenFields; }
			set { _showNormallyHiddenFields = value; }
		}

		public DetailList ParentDetailList
		{
			get { return _parentDetailList; }
			set { _parentDetailList = value; }
		}

		public PalasoDataObject PdoToLayout { get; private set; }

		public EventHandler GhostRequestedLayout;

		protected Layouter(DetailList parentDetailList,
						   int rowInParent,
						   ViewTemplate viewTemplate,
						   LexEntryRepository lexEntryRepository,
						   IServiceProvider serviceProvider,
						   PalasoDataObject pdoToLayout)
		{
			if (parentDetailList == null)
			{
				throw new ArgumentNullException("parentDetailList");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			PdoToLayout = pdoToLayout;
			_parentDetailList = parentDetailList;
			_detailList = new DetailList();
			_viewTemplate = viewTemplate;
			_lexEntryRepository = lexEntryRepository;
			_serviceProvider = serviceProvider;
			//Set up the space for the delete icon
			_deleteButton.Click += OnDeleteClicked;
			_deleteButton.Active = false;
			_deleteButton.Visible = false;
			_deleteButton.ToolTip = "Delete Sense";
			DetailList.Controls.Add(_deleteButton, 2, 0);
			DetailList.MouseEnteredBounds += OnMouseEnteredBounds;
			DetailList.MouseLeftBounds += OnMouseLeftBounds;
			ParentDetailList.AddDetailList(DetailList, rowInParent);
		}

		/// <summary>
		/// actually add the widgets that are needed to the detailed list
		/// </summary>
		public int AddWidgets(PalasoDataObject wsdo)
		{
			return AddWidgets(wsdo, -1);
		}

		internal abstract int AddWidgets(PalasoDataObject wsdo, int row);

		protected Control MakeBoundControl(MultiText multiTextToBindTo, Field field)
		{
			MultiTextControl m;
			if (_previouslyGhostedControlToReuse == null)
			{
				m = new MultiTextControl(field.WritingSystemIds,
										 multiTextToBindTo,
										 field.FieldName,
										 field.Visibility !=
										 CommonEnumerations.VisibilitySetting.ReadOnly,
										 //show annotation
										 BasilProject.Project.WritingSystems,
										 field.Visibility,
										 field.IsSpellCheckingEnabled,
										 field.IsMultiParagraph,
										 _serviceProvider);
			}
			else
			{
				m = _previouslyGhostedControlToReuse;
				_previouslyGhostedControlToReuse = null;
			}

			BindMultiTextControlToField(m, multiTextToBindTo);
			return m;
		}

		private void BindMultiTextControlToField(MultiTextControl control,
												 INotifyPropertyChanged multiTextToBindTo)
		{
			foreach (Control c in control.TextBoxes)
			{
					TextBinding binding = new TextBinding(multiTextToBindTo, ((IControlThatKnowsWritingSystem) c).WritingSystem.Id, c);
					binding.ChangeOfWhichItemIsInFocus +=
						_detailList.OnBinding_ChangeOfWhichItemIsInFocus;

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

		protected int MakeGhostWidget<T>(PalasoDataObject parent,
										IList<T> list,
										 string fieldName,
										 string label,
										 string propertyName,
										 bool isHeading) where T : PalasoDataObject, new()
		{
			int rowCount = 0;
			Field field = ActiveViewTemplate.GetField(fieldName);
			if (field != null && field.Enabled &&
				field.Visibility == CommonEnumerations.VisibilitySetting.Visible)
			{
				MultiTextControl m = new MultiTextControl(field.WritingSystemIds,
														  new MultiText(),
														  fieldName + "_ghost",
														  false,
														  BasilProject.Project.WritingSystems,
														  field.Visibility,
														  field.IsSpellCheckingEnabled, false, null);

				Control refWidget = DetailList.AddWidgetRow(label,
															isHeading,
															m,
															0,
															true);

				foreach (IControlThatKnowsWritingSystem box in m.TextBoxes)
				{
					WeSayTextBox tb = box as WeSayTextBox;
					if (tb != null)
					{
						GhostBinding<T> g = MakeGhostBinding(parent, list, propertyName, box.WritingSystem, tb);
						g.ReferenceControl = refWidget;
					}
				}
				return 1;
			}
			else
			{
				return 0; //didn't add a row
			}
		}

		protected GhostBinding<T> MakeGhostBinding<T>(PalasoDataObject parent, IList<T> list,
													  string ghostPropertyName,
													  WritingSystemDefinition writingSystem,
													  WeSayTextBox entry)
				where T : PalasoDataObject, new()
		{
			GhostBinding<T> binding = new GhostBinding<T>(parent,
				list,
														  ghostPropertyName,
														  writingSystem,
														  entry);
			binding.LayoutNeededAfterMadeReal += OnGhostBindingLayoutNeeded;
			binding.CurrentItemChanged += _detailList.OnBinding_ChangeOfWhichItemIsInFocus;
			return binding;
		}

		protected virtual void OnGhostBindingLayoutNeeded<T>(GhostBinding<T> sender,
															 IList<T> list,
															 int index,
															 MultiTextControl
																	 previouslyGhostedControlToReuse,
															 bool doGoToNextField,
															 EventArgs args)
				where T : PalasoDataObject, new()
		{
			_previouslyGhostedControlToReuse = previouslyGhostedControlToReuse;
			PdoToLayout = list[index];
			AddWidgetsAfterGhostTrigger(PdoToLayout, sender.ReferenceControl, doGoToNextField);
			if (GhostRequestedLayout != null)
			{
				GhostRequestedLayout(this, new EventArgs());
			}

		}

		protected void AddWidgetsAfterGhostTrigger(PalasoDataObject wsdo,
												   Control refControl,
												   bool doGoToNextField)
		{
			//remove the old ghost widgets and add the ones for the real sense
			_detailList.Controls.Clear();
			_detailList.RowCount = 0;
			_detailList.RowStyles.Clear();
			DetailList.Controls.Add(_deleteButton, 2, 0);
			AddWidgets(wsdo);
			Application.DoEvents();
			if (doGoToNextField)
			{
				_detailList.MoveInsertionPoint(1);
			}
			else
			{
				_detailList.MoveInsertionPoint(0);
			}
		}

		protected virtual void UpdateGhostLabel(int itemCount, int index) {}

		protected static int AddChildrenWidgets(Layouter layouter,
												PalasoDataObject po)
		{
			return layouter.AddWidgets(po);
		}

		protected int AddCustomFields(PalasoDataObject target, int insertAtRow)
		{
			int rowCount = 0;
			foreach (Field customField in ActiveViewTemplate.GetCustomFields(target.GetType().Name))
			{
#if GlossMeaning
#else
				if (customField.FieldName == LexSense.WellKnownProperties.Definition)
				{
					continue; //already put this in next to "Meaning"
				}
#endif
				rowCount = AddOneCustomField(target,
											 customField,
											 insertAtRow + rowCount /*changed feb 2008*/,
											 rowCount);
			}

			//grab any basetype class (to just one level). E.g., 'Note'
			foreach (Field customField in
					ActiveViewTemplate.GetCustomFields(target.GetType().BaseType.Name))
			{
				if (target.GetType() == typeof (LexExampleSentence) &&
					customField.FieldName == PalasoDataObject.WellKnownProperties.Note)
				{
					continue; //note actually isn't allowed at the moment
				}
				rowCount = AddOneCustomField(target,
											 customField,
											 insertAtRow + rowCount /*changed feb 2008*/,
											 rowCount);
			}
			return rowCount;
		}

		private int AddOneCustomField(PalasoDataObject target,
									  Field customField,
									  int insertAtRow,
									  int rowCount)
		{
			IReportEmptiness data = target.GetProperty<IReportEmptiness>(customField.FieldName);
			if (!customField.GetDoShow(data, ShowNormallyHiddenFields))
			{
				return rowCount;
			}
			Control box;
			switch (customField.DataTypeName)
			{
				case "Picture":
					box = MakePictureWidget(target, customField, _detailList);
					if (box == null)
						return rowCount; // other code does the user notification
					break;
				case "Flag":
					box = MakeCheckBoxWidget(target, customField);
					break;
				case "Option":
					box = MakeOptionWidget(target, customField);
					break;
				case "OptionCollection":
					box = MakeOptionCollectionWidget(target, customField);
					break;
				case "MultiText":
					box =
							MakeBoundControl(
									target.GetOrCreateProperty<MultiText>(customField.FieldName),
									customField);
					break;
				default:
					LexRelationType lexRelType = GetRelationType(customField.DataTypeName);
					if (lexRelType != null)
					{
						box = MakeRelationWidget(target, lexRelType, customField);
					}
					else
					{
						throw new ApplicationException(
								string.Format("WeSay doesn't understand how to layout a {0}",
											  customField.DataTypeName));
					}
					break;
			}

			string label = StringCatalog.Get(customField.DisplayName);

			//for checkboxes, the label is part of the control
			if (customField.DataTypeName == "Flag")
			{
				label = string.Empty;
			}

			Control c = DetailList.AddWidgetRow(StringCatalog.Get(label),
												false,
												box,
												insertAtRow,
												false);

			DetailList.GetRow(c);
			++rowCount;
			return rowCount;
		}

		private static LexRelationType GetRelationType(string dataTypeName)
		{
			foreach (LexRelationType type in WeSayWordsProject.Project.RelationTypes)
			{
				if (type.ID == dataTypeName)
				{
					return type;
				}
			}
			return null;
		}

		/// <summary>
		/// This is used to convert from the IEnuerable<string> that the cache give us
		/// to the IEnumerable<object> that AutoComplete needs.
		/// </summary>
		//        public class LexEntryEnumerableToObjectEnumerableWrapper : IEnumerable<object>
		//        {
		//            private readonly IEnumerable<LexEntry> _collection;
		//
		//            public LexEntryEnumerableToObjectEnumerableWrapper(IEnumerable<LexEntry> collection)
		//            {
		//                if (collection == null)
		//                {
		//                    throw new ArgumentNullException("collection");
		//                }
		//                _collection = collection;
		//            }
		//
		//            IEnumerator<object> IEnumerable<object>.GetEnumerator()
		//            {
		//                foreach (object s in _collection)
		//                {
		//                    yield return s;
		//                }
		//            }
		//
		//            public IEnumerator GetEnumerator()
		//            {
		//                return ((IEnumerable<object>)this).GetEnumerator();
		//            }
		//
		//        }
		private Control MakeRelationWidget(PalasoDataObject target, LexRelationType type, Field field)
		{
			return RelationController.CreateWidget(target,
												   type,
												   field,
												   _lexEntryRepository,
												   _detailList.OnBinding_ChangeOfWhichItemIsInFocus);
		}

		//        void OnSelectedItemChanged(object sender, EventArgs e)
		//        {
		//            WeSayAutoCompleteTextBox box = sender as WeSayAutoCompleteTextBox;
		//            LexRelation relation = (LexRelation)box.Tag;
		//            Palaso.Reporting.Logger.WriteMinorEvent("Changing value of LexRelation Control ({0})", relation.TypeId);
		//
		//            if (box.SelectedItem == null)
		//            {
		//                relation.Key = string.Empty;
		//            }
		//            else
		//            {
		//                relation.Target = box.SelectedItem;
		//            }
		//        }

		protected Control MakeOptionWidget(PalasoDataObject target, Field field)
		{
			OptionRef optionRefTarget = target.GetOrCreateProperty<OptionRef>(field.FieldName);
			OptionsList list = WeSayWordsProject.Project.GetOptionsList(field, false);
			WritingSystemDefinition preferredWritingSystem = _viewTemplate.GetDefaultWritingSystemForField(field.FieldName);
			SingleOptionControl control = new SingleOptionControl(optionRefTarget,
																  list,
																  field.FieldName,
																  preferredWritingSystem);
			SimpleBinding<string> binding = new SimpleBinding<string>(optionRefTarget, control);
			binding.CurrentItemChanged += _detailList.OnBinding_ChangeOfWhichItemIsInFocus;
			return control;
		}

		private static Control MakeOptionCollectionWidget(PalasoDataObject target, Field field)
		{
			OptionsList availableOptions = WeSayWordsProject.Project.GetOptionsList(field, false);
			OptionRefCollection refsOfChoices =
				target.GetOrCreateProperty<OptionRefCollection>(field.FieldName);
			//            OptionCollectionControl control =
			//                   new OptionCollectionControl(refsOfChoices, availableOptions, field.WritingSystemIds[0]);
			IList<WritingSystemDefinition> writingSystems =
				BasilProject.Project.WritingSystemsFromIds(field.WritingSystemIds);
			IChoiceSystemAdaptor<Option, string, OptionRef> displayAdaptor;

			if (field.FieldName== LexSense.WellKnownProperties.SemanticDomainDdp4)
			{
				displayAdaptor = new DdpOptionDisplayAdaptor(availableOptions, field.WritingSystemIds[0]);
			}
			else
			{
				displayAdaptor = new OptionDisplayAdaptor(availableOptions, field.WritingSystemIds[0]);
			}


		ReferenceCollectionEditor<Option, string, OptionRef> control =
					new ReferenceCollectionEditor<Option, string, OptionRef>(refsOfChoices.Members,
																			 availableOptions.
																					 Options,
																			 writingSystems,
																			 field.Visibility,
																			 displayAdaptor);
			control.AlternateEmptinessHelper = refsOfChoices;
			return control;
		}

		protected Control MakeCheckBoxWidget(PalasoDataObject target, Field field)
		{
			FlagState boxState = target.GetOrCreateProperty<FlagState>(field.FieldName);

			CheckBoxControl control = new CheckBoxControl(boxState.Value,
														  field.DisplayName,
														  field.FieldName);
			SimpleBinding<bool> binding = new SimpleBinding<bool>(boxState, control);
			binding.CurrentItemChanged += _detailList.OnBinding_ChangeOfWhichItemIsInFocus;
			return control;
		}

		protected virtual Control MakePictureWidget(PalasoDataObject target, Field field, DetailList detailList)
		{
			ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(),"Sorry, pictures are only supported on senses");
			return null;//only LexSenseLayouter actually has this
		}
	}

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Options;
using SIL.UiBindings;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation.Options;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;
using SIL.i18n;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// A Layouter is responsible for filling a detailed list with the contents
	/// of a single data object (e.g. LexSense, LexExample), etc.
	/// There are will normally be a single subclass per class of data,
	/// and each of these layout erstwhile call a different layouter for each
	/// child object (e.g. LexEntryLayouter would employ a SenseLayouter to display senses).
	/// </summary>
	public abstract class Layouter : IDisposable
	{
		/// <summary>
		/// The DetailList we are filling.
		/// </summary>
		private DetailList _detailList;
		/// <summary>
		/// The column width is used to initialize the width of the MultiTextControl objects.
		/// </summary>
		protected int[] _columnWidths;

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

		protected readonly DeleteButton _deleteButton = new DeleteButton();

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
				_deleteButton.Active = value &&
					DetailList.MouseOverRow >= FirstRow && DetailList.MouseOverRow <= LastRow;
			}
		}

		private void OnMouseLeftBounds(object sender, EventArgs e)
		{
			_deleteButton.Active = false;
		}

		private void OnMouseEnteredBounds(object sender, EventArgs e)
		{
			if (Deletable && DetailList.MouseOverRow >= FirstRow && DetailList.MouseOverRow <= LastRow)
				_deleteButton.Active = true;
			else
				_deleteButton.Active = false;
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

		public Layouter ParentLayouter { get; internal set; }

		public List<Layouter> ChildLayouts { get; protected set; }

		public int FirstRow { get; set; }

		public int LastRow { get; set; }

		public PalasoDataObject PdoToLayout { get; private set; }

		public EventHandler GhostRequestedLayout;

		protected Layouter(DetailList table,
						   int beginningRow,
						   ViewTemplate viewTemplate,
						   LexEntryRepository lexEntryRepository,
						   IServiceProvider serviceProvider,
						   PalasoDataObject pdoToLayout)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			PdoToLayout = pdoToLayout;
			FirstRow = beginningRow;
			_detailList = table;
			_viewTemplate = viewTemplate;
			_lexEntryRepository = lexEntryRepository;
			_serviceProvider = serviceProvider;
			//Set up the space for the delete icon
			_deleteButton.Click += OnDeleteClicked;
			_deleteButton.Active = false;
			_deleteButton.Visible = false;
			_deleteButton.ToolTip = StringCatalog.Get("Delete Meaning");
			DetailList.Controls.Add(_deleteButton, 2, beginningRow);
			DetailList.MouseEnteredBounds += OnMouseEnteredBounds;
			DetailList.MouseLeftBounds += OnMouseLeftBounds;
			ChildLayouts = new List<Layouter>();
		}
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Found that the delete button and the event handlers
				// were causing memory leaks by not allowing the objects to
				// be released.
				_deleteButton.Dispose();
				DetailList.MouseEnteredBounds -= OnMouseEnteredBounds;
				DetailList.MouseLeftBounds -= OnMouseLeftBounds;
			}
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
				if (_columnWidths != null && _columnWidths.Length == 3)
					m.Width = _columnWidths[1];
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
				TextBinding binding = new TextBinding(multiTextToBindTo, ((IControlThatKnowsWritingSystem)c).WritingSystem.LanguageTag, c);
				binding.ChangeOfWhichItemIsInFocus += _detailList.OnBinding_ChangeOfWhichItemIsInFocus;
			}
		}


		protected int MakeGhostWidget<T>(PalasoDataObject parent,
										IList<T> list,
										string fieldName,
										string label,
										string propertyName,
										bool isHeading,
										int row) where T : PalasoDataObject, new()
		{
			Field field = ActiveViewTemplate.GetField(fieldName);
			if (field != null && field.Enabled &&
				field.Visibility == CommonEnumerations.VisibilitySetting.Visible)
			{
				DetailList.SuspendLayout();
				MultiTextControl m = new MultiTextControl(field.WritingSystemIds,
														  new MultiText(),
														  fieldName + "_ghost",
														  false,
														  BasilProject.Project.WritingSystems,
														  field.Visibility,
														  field.IsSpellCheckingEnabled, false, _serviceProvider);
				if (_columnWidths != null && _columnWidths.Length == 3)
					m.Width = _columnWidths[1];

				Control refWidget = DetailList.AddWidgetRow(label,
															isHeading,
															m,
															row,
															true);

				foreach (IControlThatKnowsWritingSystem box in m.TextBoxes)
				{
					var tb = box as IWeSayTextBox;
					if (tb != null)
					{
						GhostBinding<T> g = MakeGhostBinding(parent, list, propertyName, box.WritingSystem, tb);
						g.ReferenceControl = refWidget;
					}
				}
				DetailList.ResumeLayout(false);
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
													  IWeSayTextBox entry)
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
															MultiTextControl previouslyGhostedControlToReuse,
															bool doGoToNextField,
															EventArgs args)
				where T : PalasoDataObject, new()
		{
			DetailList.SuspendLayout();
			var position = _detailList.GetCellPosition(sender.ReferenceControl);
			var rowIndex = position.Row;
			_previouslyGhostedControlToReuse = previouslyGhostedControlToReuse;
			PdoToLayout = list[index];
			AddWidgetsAfterGhostTrigger(PdoToLayout, sender.ReferenceControl, doGoToNextField);
			if (GhostRequestedLayout != null)
			{
				GhostRequestedLayout(this, new EventArgs());
			}
			DetailList.ResumeLayout();
			if (doGoToNextField)
			{
				_detailList.MoveInsertionPoint(rowIndex + 1);
			}
			else
			{
				_detailList.MoveInsertionPoint(rowIndex);
			}
			// For Linux/Mono, merely resuming layout and refreshing doesn't work.
			ForceLayoutAndRefresh();
		}

		/// <summary>
		/// Layout the DetailList again and then refresh it.  Both steps are needed for some
		/// reason for Linux/Mono, and they won't hurt for Windows/.Net.
		/// </summary>
		protected void ForceLayoutAndRefresh()
		{
			_detailList.PerformLayout();
			_detailList.Refresh();
		}

		protected virtual void AddWidgetsAfterGhostTrigger(PalasoDataObject wsdo,
												   Control refControl,
												   bool doGoToNextField)
		{
			Debug.Assert(!(this is LexEntryLayouter));
			_detailList.SuspendLayout();
			var position = _detailList.GetCellPosition(refControl);
			Debug.Assert(position.Row >= 0);
			var newLayouter = CreateAndInsertNewLayouter(position.Row, wsdo);
			Debug.Assert(newLayouter != null);
			// Add the widgets for the real object.	 Retain the existing ghost widget, which
			// gets shifted down to follow the row(s) for the real object.
			int rowCount = newLayouter.AddWidgets(wsdo, position.Row);
			var newPosition = _detailList.GetCellPosition(refControl);
			Debug.Assert(position.Row + rowCount == newPosition.Row);
			FirstRow = LastRow = newPosition.Row;	// adjust settings for ghost's layouter (ie, "this")
			AdjustLayoutRowsAfterGhostTrigger(rowCount);
			//DumpLayoutRowsForDebugging();
			_detailList.ResumeLayout();
		}

		private void DumpLayoutRowsForDebugging()
		{
			var parentEntryLayouter = this;
			while (parentEntryLayouter.ParentLayouter != null)
				parentEntryLayouter = parentEntryLayouter.ParentLayouter;
			WriteLayouterRows(parentEntryLayouter, "");
		}

		private static void WriteLayouterRows(Layouter layouter, string indent)
		{
			Debug.WriteLine("{0}Layout: this={1}, FirstRow={2}, LastRow={3}", indent, layouter, layouter.FirstRow, layouter.LastRow);
			foreach (var child in layouter.ChildLayouts)
				WriteLayouterRows(child, indent+"    ");
		}

		/// <summary>
		/// Create an appropriate Layouter for a new object created from a ghost, and insert
		/// it at the right place in the Layouter tree.
		/// </summary>
		protected abstract Layouter CreateAndInsertNewLayouter(int row, PalasoDataObject wsdo);

		protected virtual void AdjustLayoutRowsAfterGhostTrigger(int rowCount)
		{
			// Nothing needs to be done for LexEntryLayouter or LexSenseLayouter.
		}

		protected virtual void UpdateGhostLabel(int itemCount, int index) {}

		protected static int AddChildrenWidgets(Layouter layouter, PalasoDataObject po, int row)
		{
			return layouter.AddWidgets(po, row);
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
				if (target is LexExampleSentence &&
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

		private Control MakeRelationWidget(PalasoDataObject target, LexRelationType type, Field field)
		{
			return RelationController.CreateWidget(target,
												   type,
												   field,
												   _lexEntryRepository,
												   _detailList.OnBinding_ChangeOfWhichItemIsInFocus);
		}

		protected Control MakeOptionWidget(PalasoDataObject target, Field field)
		{
			OptionRef optionRefTarget = target.GetOrCreateProperty<OptionRef>(field.FieldName);
			OptionsList list = WeSayWordsProject.Project.GetOptionsList(field, false);
			WritingSystemDefinition preferredWritingSystem = _viewTemplate.GetDefaultWritingSystemForField(field.FieldName);
			SingleOptionControl control = new SingleOptionControl(optionRefTarget,
																  list,
																  field.FieldName,
																  preferredWritingSystem,
																  WeSayWordsProject.Project.ServiceLocator);
			SimpleBinding<string> binding = new SimpleBinding<string>(optionRefTarget, control);
			binding.CurrentItemChanged += _detailList.OnBinding_ChangeOfWhichItemIsInFocus;
			return control;
		}

		private static Control MakeOptionCollectionWidget(PalasoDataObject target, Field field)
		{
			OptionsList availableOptions = WeSayWordsProject.Project.GetOptionsList(field, false);
			OptionRefCollection refsOfChoices =
				target.GetOrCreateProperty<OptionRefCollection>(field.FieldName);
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
																			 displayAdaptor,
																			 WeSayWordsProject.Project.ServiceLocator);
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

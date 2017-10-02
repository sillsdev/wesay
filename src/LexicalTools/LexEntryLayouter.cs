using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using SIL.DictionaryServices.Model;
using SIL.i18n;
using SIL.Lift;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.audio;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexEntryLayouter: Layouter
	{
		private bool _sensesAreDeletable = false;
		private readonly ConfirmDeleteFactory _confirmDeleteFactory;

		public LexEntry Entry { get; set; }

		public LexEntryLayouter(DetailList parentDetailList,
								int parentRow,
								ViewTemplate viewTemplate,
								LexEntryRepository lexEntryRepository,
								IServiceLocator serviceLocator,
								LexEntry entry,
								bool sensesAreDeletable,
								ConfirmDeleteFactory confirmDeleteFactory,
								bool showMinorMeaningLabel)
			: base(parentDetailList, parentRow, viewTemplate, lexEntryRepository, CreateLayoutInfoServiceProvider(serviceLocator, entry), entry)
		{
			Entry = entry;
			ShowMinorMeaningLabel = showMinorMeaningLabel;
			_sensesAreDeletable = sensesAreDeletable;
			_confirmDeleteFactory = confirmDeleteFactory;
			DetailList.LabelsChanged += OnLabelsChanged;
			_columnWidths = parentDetailList.GetColumnWidths();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// This event handler was reported by the memory leak tool
				// as holding the detail list from being released.
				DetailList.LabelsChanged-= OnLabelsChanged;
			}
			base.Dispose(disposing);
		}
		internal bool SensesAreDeletable { get { return _sensesAreDeletable; } }

		private void OnLabelsChanged(object sender, EventArgs e)
		{
			var maxWidth = DetailList.WidestLabelWidthWithMargin;
			DetailList.LabelColumnWidth = maxWidth;
		}

		public int AddWidgets()
		{
			return AddWidgets(Entry, -1);
		}

		internal override int AddWidgets(PalasoDataObject wsdo, int insertAtRow)
		{
			return AddWidgets((LexEntry) wsdo, insertAtRow);
		}

		/// <summary>
		/// We don't have ghosts for LexEntry objects, so this implementation should never
		/// be called!
		/// </summary>
		protected override Layouter CreateAndInsertNewLayouter(int row, PalasoDataObject wsdo)
		{
			return null;	// no parent layouter, so we can't insert.
		}

		internal int AddWidgets(LexEntry entry, int insertAtRow)
		{
			DetailList.SuspendLayout();
			Debug.Assert(DetailList.RowCount == 0);
			Debug.Assert(DetailList.ColumnCount == 3);
			Debug.Assert(DetailList.RowStyles.Count == 0);
			FirstRow = 0;
			int rowCount = 0;
			Field field = ActiveViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null && field.GetDoShow(entry.LexicalForm, ShowNormallyHiddenFields))
			{
				Control formControl = MakeBoundControl(entry.LexicalForm, field);
				DetailList.AddWidgetRow(StringCatalog.Get(field.DisplayName),
										true,
										formControl,
										insertAtRow,
										false);
				insertAtRow = DetailList.GetRow(formControl);
				++rowCount;
			}
			rowCount += AddCustomFields(entry, insertAtRow + rowCount);

			var rowCountBeforeSenses = rowCount;
			LastRow = insertAtRow + rowCount;
			foreach (var lexSense in entry.Senses)
			{
				var layouter = new LexSenseLayouter(
					DetailList,
					rowCount,
					ActiveViewTemplate,
					RecordListManager,
					_serviceProvider,
					lexSense
				)
				{
					ShowNormallyHiddenFields = ShowNormallyHiddenFields,
					Deletable = _sensesAreDeletable,
					ShowMinorMeaningLabel = ShowMinorMeaningLabel,
					ParentLayouter = this
				};
				layouter.DeleteClicked += OnSenseDeleteClicked;
				rowCount += AddChildrenWidgets(layouter, lexSense, rowCount);
				ChildLayouts.Add(layouter);
			}

			//see: WS-1120 Add option to limit "add meanings" task to the ones that have a semantic domain
			//also: WS-639 (jonathan_coombs@sil.org) In Add meanings, don't show extra meaning slots just because a sense was created for the semantic domain
			var ghostingRule = ActiveViewTemplate.GetGhostingRuleForField(LexEntry.WellKnownProperties.Sense);
			if (rowCountBeforeSenses == rowCount || ghostingRule.ShowGhost)
			{
				AddSenseGhost(entry, rowCount);
				rowCount++;
			}

			DetailList.ResumeLayout(false);
			return rowCount;
		}

		internal void OnSenseDeleteClicked(object sender, EventArgs e)
		{
			var sendingLayouter = (Layouter) sender;
			var sense = (LexSense) sendingLayouter.PdoToLayout;
			IConfirmDelete confirmation = _confirmDeleteFactory();
			var deletionStringToLocalize = StringCatalog.Get("This will permanently remove the meaning");
			var meaningText = (GlossMeaningField ?
				sense.Gloss.GetBestAlternative(ActiveViewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Gloss).Id) :
				sense.Definition.GetBestAlternative(ActiveViewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Definition).Id)
				);
			confirmation.Message = String.Format("{0} {1}.", deletionStringToLocalize,
				meaningText);
			if (!confirmation.DeleteConfirmed)
			{
				return;
			}
			DetailList.SuspendLayout();
			Entry.Senses.Remove(sense);
			DetailList.Clear();
			//for now just relayout the whole thing as the meaning numbers will change etc.
			AddWidgets();
			DetailList.ResumeLayout();
			// For Linux/Mono, merely resuming layout doesn't work -- the display doesn't redraw properly.
			ForceLayoutAndRefresh();
		}

		private void AddSenseGhost(LexEntry entry, int row)
		{
			var layouter = new LexSenseLayouter(
				DetailList,
				row,
				ActiveViewTemplate,
				RecordListManager,
				_serviceProvider,
				null
				)
			{
				ParentLayouter = this
			};
			layouter.AddGhost(null, entry.Senses, true, row);
			layouter.Deletable = false;
			layouter.DeleteClicked += OnSenseDeleteClicked;
			ChildLayouts.Add(layouter);
		}

		/// <summary>
		/// Here we (somewhat awkwardly) create an inner container which is set up with knowledge of the current entry
		/// </summary>
		private static IServiceProvider CreateLayoutInfoServiceProvider(IServiceLocator serviceLocator, LexEntry entry)
		{
			SIL.Code.Guard.AgainstNull(serviceLocator, "serviceLocator");
			SIL.Code.Guard.AgainstNull(entry, "entry");

			var namingHelper = (MediaNamingHelper) serviceLocator.GetService(typeof (MediaNamingHelper));
			var ap = new AudioPathProvider(WeSayWordsProject.Project.PathToAudio,
						() => entry.LexicalForm.GetBestAlternativeString(namingHelper.LexicalUnitWritingSystemIds));

		   return serviceLocator.CreateNewUsing(c=>c.RegisterInstance(ap));
	   }
	}


}

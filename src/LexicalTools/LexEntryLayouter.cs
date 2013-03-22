using System;
using System.Windows.Forms;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using Palaso.DictionaryServices.Model;
using Palaso.i18n;
using Palaso.Lift;
using WeSay.LexicalModel;
using WeSay.LexicalTools.DictionaryBrowseAndEdit;
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
								ConfirmDeleteFactory confirmDeleteFactory)
			: base(parentDetailList, parentRow, viewTemplate, lexEntryRepository, CreateLayoutInfoServiceProvider(serviceLocator, entry), entry)
		{
			Entry = entry;
			DetailList.Name = "LexEntryDetailList";
			_sensesAreDeletable = sensesAreDeletable;
			_confirmDeleteFactory = confirmDeleteFactory;
			DetailList.LabelsChanged += OnLabelsChanged;
		}

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

		internal int AddWidgets(LexEntry entry, int insertAtRow)
		{
			DetailList.SuspendLayout();
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
			foreach (var lexSense in entry.Senses)
			{
				var layouter = new LexSenseLayouter(
					DetailList,
					rowCount,
					ActiveViewTemplate,
					RecordListManager,
					_serviceProvider,
					lexSense
				);
				layouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;
				layouter.Deletable = _sensesAreDeletable;
				layouter.DeleteClicked += OnSenseDeleteClicked;
				AddChildrenWidgets(layouter, lexSense);
				rowCount++;
			}

			//see: WS-1120 Add option to limit "add meanings" task to the ones that have a semantic domain
			//also: WS-639 (jonathan_coombs@sil.org) In Add meanings, don't show extra meaning slots just because a sense was created for the semantic domain
			var ghostingRule = ActiveViewTemplate.GetGhostingRuleForField(LexEntry.WellKnownProperties.Sense);
			if (rowCountBeforeSenses == rowCount || ghostingRule.ShowGhost)
			{
				AddSenseGhost(entry, rowCount);
				rowCount++;
			}

			DetailList.ResumeLayout();
			return rowCount;
		}

		private void OnSenseDeleteClicked(object sender, EventArgs e)
		{
			var sendingLayouter = (Layouter) sender;
			var sense = (LexSense) sendingLayouter.PdoToLayout;
			IConfirmDelete confirmation = _confirmDeleteFactory();
			confirmation.Message = String.Format("This will permanently remove the sense with meaning {0}.",
				sense.Definition.GetBestAlternative(ActiveViewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Definition).Id));
			if (!confirmation.DeleteConfirmed)
			{
				return;
			}
			Entry.Senses.Remove(sense);
			DetailList.Clear();
			//for now just relayout the whole thing as the meaning numbers will change etc.
			AddWidgets();
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
				);
			layouter.AddGhost(null, entry.Senses, true);
			layouter.Deletable = false;
			layouter.GhostRequestedLayout += OnGhostRequestedlayout;
			layouter.DeleteClicked += OnSenseDeleteClicked;
			row++;
		}

		private void OnGhostRequestedlayout(object sender, EventArgs e)
		{
			if (ActiveViewTemplate.GetGhostingRuleForField(LexEntry.WellKnownProperties.Sense).ShowGhost)
			{
				((Layouter) sender).Deletable = true;
				//The old ghost takes care of turing itself into a properly layouted sense.
				//We just add a new ghost here
				AddSenseGhost(Entry, DetailList.RowCount);
			}
		}

		/// <summary>
		/// Here we (somewhat awkwardly) create an inner container which is set up with knowledge of the current entry
		/// </summary>
		private static IServiceProvider CreateLayoutInfoServiceProvider(IServiceLocator serviceLocator, LexEntry entry)
		{
			Palaso.Code.Guard.AgainstNull(serviceLocator, "serviceLocator");
			Palaso.Code.Guard.AgainstNull(entry, "entry");

			var namingHelper = (MediaNamingHelper) serviceLocator.GetService(typeof (MediaNamingHelper));
			var ap = new AudioPathProvider(WeSayWordsProject.Project.PathToAudio,
						() => entry.LexicalForm.GetBestAlternativeString(namingHelper.LexicalUnitWritingSystemIds));

		   return serviceLocator.CreateNewUsing(c=>c.RegisterInstance(ap));
	   }
	}


}
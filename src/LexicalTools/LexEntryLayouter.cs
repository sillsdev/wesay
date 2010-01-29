using System;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using Palaso.DictionaryServices.Model;
using Palaso.I8N;
using Palaso.Lift;
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
		public LexEntry Entry { get; set; }

		public LexEntryLayouter(DetailList builder,
								ViewTemplate viewTemplate,
								LexEntryRepository lexEntryRepository,
								IServiceLocator serviceLocator,
								LexEntry entry)
			: base(builder, viewTemplate, lexEntryRepository, CreateLayoutInfoServiceProvider(serviceLocator, entry))
		{
			Entry = entry;
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
			var layouter = new LexSenseLayouter(
				DetailList,
				ActiveViewTemplate,
				RecordListManager,
				_serviceProvider
			);
			layouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;
			rowCount = AddChildrenWidgets(layouter, entry.Senses, insertAtRow, rowCount);

			//see: WS-1120 Add option to limit "add meanings" task to the ones that have a semantic domain
			//also: WS-639 (jonathan_coombs@sil.org) In Add meanings, don't show extra meaning slots just because a sense was created for the semantic domain
			var ghostingRule = ActiveViewTemplate.GetGhostingRuleForField(LexEntry.WellKnownProperties.Sense);
			if (rowCountBeforeSenses == rowCount || ghostingRule.ShowGhost)
			{
				rowCount += layouter.AddGhost(null, entry.Senses, true);
			}

			DetailList.ResumeLayout();
			return rowCount;
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

		   return serviceLocator.CreateNewUsing(c=>c.Register(ap));
	   }
	}


}
using System;
using System.Windows.Forms;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
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
			: base(builder, viewTemplate, lexEntryRepository, CreateLayoutInfoServiceProvider(serviceLocator, viewTemplate, entry))
		{
			Entry = entry;
		}

		public int AddWidgets()
		{
			return AddWidgets(Entry, -1);
		}

		internal override int AddWidgets(WeSayDataObject wsdo, int insertAtRow)
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
			LexSenseLayouter layouter = new LexSenseLayouter(DetailList,
															  ActiveViewTemplate,
															  RecordListManager,
															  _serviceProvider);
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

		private static IServiceProvider CreateLayoutInfoServiceProvider(IServiceLocator serviceLocator, ViewTemplate viewTemplate, LexEntry entry)
		{
			Guard.AgainstNull(serviceLocator, "serviceLocator");

//            if (viewTemplate == null)
//                return null;//some unrelated unit tests don't give us this parameter
//            Field lexicalUnitField = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
//            if(lexicalUnitField == null)
//                return null;//some unrelated unit tests lack this field
//
//            var ap = new AudioPathProvider(Project.WeSayWordsProject.Project.PathToAudio,
//                        () => entry.LexicalForm.GetBestAlternativeString(lexicalUnitField.WritingSystemIds));
//            return new LayoutInfoProvider(ap);

			if (viewTemplate == null)
				return null;//some unrelated unit tests don't give us this parameter
			Field lexicalUnitField = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (lexicalUnitField == null)
				return null;//some unrelated unit tests lack this field

			var ap = new AudioPathProvider(Project.WeSayWordsProject.Project.PathToAudio,
						() => entry.LexicalForm.GetBestAlternativeString(lexicalUnitField.WritingSystemIds));
		   // return new LayoutInfoProvider(ap);
			//              (b => b.Register(TaskMemoryRepository.CreateOrLoadTaskMemoryRepository(_project.Name, _project.PathToWeSaySpecificFilesDirectoryInProject )));

		   return serviceLocator.CreateNewUsing(c=>c.Register(ap));
		}
	}


}
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Palaso.Lift;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using Palaso.LexicalModel;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexSenseLayouter: Layouter
	{
		public LexSenseLayouter(DetailList builder, ViewTemplate viewTemplate, LexEntryRepository lexEntryRepository,
			IServiceProvider serviceProvider)
				: base(builder, viewTemplate, lexEntryRepository, serviceProvider)
		{
		}

		internal override int AddWidgets(PalasoDataObject wsdo, int insertAtRow)
		{
			LexSense sense = (LexSense) wsdo;
			int rowCount = 0;
			DetailList.SuspendLayout();
			try
			{
#if GlossMeaning
				Field field = ActiveViewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
				if (field != null && field.GetDoShow(sense.Gloss, this.ShowNormallyHiddenFields))
				{
					Control meaningControl = MakeBoundControl(sense.Gloss, field);
#else
				Field field = ActiveViewTemplate.GetField(LexSense.WellKnownProperties.Definition);
				if (field != null && field.GetDoShow(sense.Definition, ShowNormallyHiddenFields))
				{
					Control meaningControl = MakeBoundControl(sense.Definition, field);
#endif
					string label = StringCatalog.Get("~Meaning");
					LexEntry entry = sense.Parent as LexEntry;
					if (entry != null) // && entry.Senses.Count > 1)
					{
						label += " " + (entry.Senses.IndexOf(sense) + 1);
					}
					Control meaningRowControl = DetailList.AddWidgetRow(label,
																		true,
																		meaningControl,
																		insertAtRow,
																		false);
					++rowCount;
					insertAtRow = DetailList.GetRow(meaningRowControl);
				}

				//#if GlossMeaning
				//#else
				//                Field glossfield = ActiveViewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
				//                if (glossfield != null && glossfield.GetDoShow(sense.Gloss, this.ShowNormallyHiddenFields))
				//                {
				//                    Control control = MakeBoundControl(sense.Gloss, glossfield);
				//                    DetailList.AddWidgetRow(
				//                        StringCatalog.Get("~Gloss",
				//                                          "This is the field that normally has just a single word translation, not a full definition. Mostly used with interlinear text displays."),
				//                        false,
				//                        control, insertAtRow + rowCount, false);
				//                    ++rowCount;
				//                    insertAtRow = DetailList.GetRow(control);
				//                }
				//#endif

				rowCount += AddCustomFields(sense, insertAtRow + rowCount);

				LexExampleSentenceLayouter exampleLayouter =
						new LexExampleSentenceLayouter(DetailList, ActiveViewTemplate, _serviceProvider);
				exampleLayouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;

				rowCount = AddChildrenWidgets(exampleLayouter,
											  sense.ExampleSentences,
											  insertAtRow,
											  rowCount);

				//add a ghost for another example if we don't have one or we're in the "show all" mode
				//removed because of its effect on the Add Examples task, where
				//we'd like to be able to add more than one
				//if (ShowNormallyHiddenFields || sense.ExampleSentences.Count == 0)
				{
					rowCount += exampleLayouter.AddGhost(sense, sense.ExampleSentences,
														 insertAtRow + rowCount);
				}
			}
			catch (ConfigurationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
			DetailList.ResumeLayout();
			return rowCount;
		}

		public int AddGhost(PalasoDataObject parent, IList<LexSense> list, bool isHeading)
		{
			int insertAtRow = -1;
			string label = GetLabelForMeaning(list.Count);
#if GlossMeaning
			return MakeGhostWidget<LexSense>(list, insertAtRow, Field.FieldNames.SenseGloss.ToString(), label, "Gloss", isHeading);
#else
			return MakeGhostWidget<LexSense>(parent,
									list,
								   insertAtRow,
								   LexSense.WellKnownProperties.Definition,
								   label,
								   "Definition",
								   isHeading);
#endif
		}

		private static string GetLabelForMeaning(int itemCount)
		{
			string label = StringCatalog.Get("~Meaning",
											 "This label is shown once, but has two roles.  1) it labels the defintion field, and 2) marks the beginning of the set of fields which make up a sense. So, in english, if we labelled this 'definition', it would describe the field well but wouldn't label the section well.");
			if (itemCount > 0)
			{
				label += " " + (itemCount + 1);
			}
			return label;
		}

		protected override void UpdateGhostLabel(int itemCount, int rowOfGhost)
		{
			DetailList.GetLabelControlFromRow(rowOfGhost).Text = GetLabelForMeaning(itemCount);
		}

		protected override Control MakePictureWidget(PalasoDataObject target, Field field, DetailList detailList)
		{
			PictureRef pictureRef = target.GetOrCreateProperty<PictureRef>(field.FieldName);

			PictureControl control = _serviceProvider.GetService(typeof(PictureControl)) as PictureControl;
			control.SearchTermProvider = new SenseSearchTermProvider(target as LexSense);
			if (!String.IsNullOrEmpty(pictureRef.Value))
			{
				control.Value = pictureRef.Value;
			}
			SimpleBinding<string> binding = new SimpleBinding<string>(pictureRef, control);
			binding.CurrentItemChanged += detailList.OnBinding_ChangeOfWhichItemIsInFocus;
			return control;
		}
	}

	public class SenseSearchTermProvider : ISearchTermProvider
	{
		private readonly LexSense _sense;

		public SenseSearchTermProvider(LexSense sense)
		{
			_sense = sense;
		}

		public string SearchString
		{
			get { return _sense.Definition.GetFirstAlternative(); }//review
		}
	}
}

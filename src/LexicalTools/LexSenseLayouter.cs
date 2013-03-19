using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Palaso.DictionaryServices.Model;
using Palaso.i18n;
using Palaso.Lift;
using Palaso.Reporting;
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
		public LexSenseLayouter(DetailList parentDetailList, int parentRow, ViewTemplate viewTemplate, LexEntryRepository lexEntryRepository,
			IServiceProvider serviceProvider, LexSense senseToLayout)
			: base(parentDetailList, parentRow, viewTemplate, lexEntryRepository, serviceProvider, senseToLayout)
		{
			DetailList.Name = "LexSenseDetailList";
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
					//NB: http://jira.palaso.org/issues/browse/WS-33937 describes how this makes it hard to change this in English (but not other languages)
					string label = StringCatalog.Get("~Meaning");
					LexEntry entry = sense.Parent as LexEntry;
					if (entry != null) // && entry.Senses.Count > 1)
					{
						label += " " + (entry.Senses.IndexOf(sense) + 1);
					}
					DetailList.AddWidgetRow(label, true, meaningControl, insertAtRow, false);
					rowCount++;
				}

				rowCount += AddCustomFields(sense, rowCount);

				foreach (var lexExampleSentence in sense.ExampleSentences)
				{
					var exampleLayouter =
						new LexExampleSentenceLayouter(DetailList, rowCount, ActiveViewTemplate, _serviceProvider, lexExampleSentence);
					exampleLayouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;
					exampleLayouter.Deletable = false;
					AddChildrenWidgets(exampleLayouter, lexExampleSentence);
					rowCount++;
				}


				//add a ghost for another example if we don't have one or we're in the "show all" mode
				//removed because of its effect on the Add Examples task, where
				//we'd like to be able to add more than one
				//if (ShowNormallyHiddenFields || sense.ExampleSentences.Count == 0)
				{
					AddExampleSentenceGhost(sense, rowCount);
					rowCount++;

				}
			}
			catch (ConfigurationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
			DetailList.ResumeLayout();
			return rowCount;
		}

		private void AddExampleSentenceGhost(LexSense sense, int insertAtRow)
		{
			var exampleLayouter =
				new LexExampleSentenceLayouter(DetailList, insertAtRow, ActiveViewTemplate, _serviceProvider, null);
			exampleLayouter.AddGhost(null, sense.ExampleSentences);
			exampleLayouter.GhostRequestedLayout += OnGhostRequestedlayout;
		}

		private void OnGhostRequestedlayout(object sender, EventArgs e)
		{
			var row = DetailList.GetPositionFromControl((DetailList)sender).Row;
			//The old ghost takes care of turing itself into a properly layouted example sentence.
			//We just add a new ghost here
			AddExampleSentenceGhost((LexSense) PdoToLayout, row + 1);
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

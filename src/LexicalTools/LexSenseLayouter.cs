using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
		}

		private Control MeaningFieldControl(Field field, MultiText meaningText)
		{
			if (field != null && field.GetDoShow(meaningText, this.ShowNormallyHiddenFields))
			{
				return MakeBoundControl(meaningText, field);
			}
			return null;
		}

		internal override int AddWidgets(PalasoDataObject wsdo, int insertAtRow)
		{
			LexSense sense = (LexSense) wsdo;
			FirstRow = insertAtRow;
			int rowCount = 0;
			DetailList.SuspendLayout();
			try
			{
				Field field = null;
				Control meaningControl = null;
				if (GlossMeaningField)
				{
					field = ActiveViewTemplate.GetField(LexSense.WellKnownProperties.Gloss);
					if (field != null && field.GetDoShow(sense.Gloss, this.ShowNormallyHiddenFields))
					{
						meaningControl = MakeBoundControl(sense.Gloss, field);
					}
				}
				else
				{
					field = ActiveViewTemplate.GetField(LexSense.WellKnownProperties.Definition);
					if (field != null && field.GetDoShow(sense.Definition, this.ShowNormallyHiddenFields))
					{
						meaningControl = MakeBoundControl(sense.Definition, field);
					}
				}

				if (meaningControl != null)
				{
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

				rowCount += AddCustomFields(sense, insertAtRow + rowCount);

				foreach (var lexExampleSentence in sense.ExampleSentences)
				{
					var exampleLayouter =
						new LexExampleSentenceLayouter(DetailList, rowCount, ActiveViewTemplate, _serviceProvider, lexExampleSentence)
						{
							ShowNormallyHiddenFields = ShowNormallyHiddenFields,
							Deletable = false,
							ParentLayouter = this
						};
					rowCount += AddChildrenWidgets(exampleLayouter, lexExampleSentence, insertAtRow + rowCount);
					ChildLayouts.Add(exampleLayouter);
				}


				//add a ghost for another example if we don't have one or we're in the "show all" mode
				//removed because of its effect on the Add Examples task, where
				//we'd like to be able to add more than one
				//if (ShowNormallyHiddenFields || sense.ExampleSentences.Count == 0)
				{
					AddExampleSentenceGhost(sense, insertAtRow + rowCount);
					rowCount++;
				}
				LastRow = insertAtRow + rowCount - 1;	// want index of last row owned, not a limit
				FixDeleteButtonPosition();
			}
			catch (ConfigurationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
			DetailList.ResumeLayout(false);
			return rowCount;
		}

		private void FixDeleteButtonPosition()
		{
			var position = DetailList.GetCellPosition(_deleteButton);
			if (position.Row != FirstRow)
				DetailList.SetCellPosition(_deleteButton, new TableLayoutPanelCellPosition(2, FirstRow));
		}

		private void AddExampleSentenceGhost(LexSense sense, int insertAtRow)
		{
			DetailList.SuspendLayout();
			var exampleLayouter =
				new LexExampleSentenceLayouter(DetailList, insertAtRow, ActiveViewTemplate, _serviceProvider, null)
				{
					ParentLayouter = this
				};
			exampleLayouter.AddGhost(null, sense.ExampleSentences, insertAtRow);
			ChildLayouts.Add(exampleLayouter);
			DetailList.ResumeLayout(false);
		}

		public int AddGhost(PalasoDataObject parent, IList<LexSense> list, bool isHeading, int insertAtRow)
		{
			string label = GetLabelForMeaning(list.Count);
			if (GlossMeaningField)
			{
				return MakeGhostWidget<LexSense>(parent,
												list,
												LexSense.WellKnownProperties.Gloss,
												label,
												"Gloss",
												isHeading,
												insertAtRow);
			}
			else
			{
				return MakeGhostWidget<LexSense>(parent,
												list,
												LexSense.WellKnownProperties.Definition,
												label,
												"Definition",
												isHeading,
												insertAtRow);
			}
		}

		private static string GetLabelForMeaning(int itemCount)
		{
			string label = StringCatalog.Get("~Meaning",
											 "This label is shown once, but has two roles.  1) it labels the gloss/definition field, and 2) marks the beginning of the set of fields which make up a sense. So, in english, if we labelled this 'definition', it would describe the field well but wouldn't label the section well.");
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

		protected override void AddWidgetsAfterGhostTrigger(PalasoDataObject wsdo, Control refControl, bool doGoToNextField)
		{
			DetailList.SuspendLayout();
			base.AddWidgetsAfterGhostTrigger(wsdo, refControl, doGoToNextField);
			// We need to update the label on the ghost slice, either adding a number to the end
			// or incrementing the number at the end.
			var position = DetailList.GetCellPosition(refControl);
			var label = DetailList.GetLabelControlFromRow(position.Row);
			Debug.Assert(label != null);
			Match match = Regex.Match(label.Text, @"(^.*) ([0-9]+)$");
			int n;
			// Note that Regex Groups have 1-based indexing.
			if (match.Success && Int32.TryParse(match.Groups[2].Value, out n))
				label.Text = String.Format("{0} {1}", match.Groups[1].Value, n + 1);
			else
				label.Text = label.Text + @" 2";
			DetailList.ResumeLayout();
		}

		/// <summary>
		/// Create an appropriate LexSenseLayouter for a new LexSense created from a ghost,
		/// and insert it at the right place in the Layouter tree.
		/// </summary>
		protected override Layouter CreateAndInsertNewLayouter(int row, PalasoDataObject wsdo)
		{
			Debug.Assert(ParentLayouter is LexEntryLayouter);
			var lexEntryLayouter = ParentLayouter as LexEntryLayouter;
			var newLayouter = new LexSenseLayouter(DetailList, row, ActiveViewTemplate, RecordListManager,
				_serviceProvider, wsdo as LexSense)
			{
				ShowNormallyHiddenFields = ShowNormallyHiddenFields,
				Deletable = lexEntryLayouter.SensesAreDeletable,
				ParentLayouter = ParentLayouter
			};
			newLayouter.DeleteClicked += lexEntryLayouter.OnSenseDeleteClicked;
			var idx = ParentLayouter.ChildLayouts.IndexOf(this);
			if (idx >= 0)
				ParentLayouter.ChildLayouts.Insert(idx, newLayouter);
			else
				ParentLayouter.ChildLayouts.Add(newLayouter);
			return newLayouter;
		}
	}

	public class SenseSearchTermProvider : ISearchTermProvider
	{
		private readonly LexSense _sense;
		private bool _glossMeaningField;

		public SenseSearchTermProvider(LexSense sense)
		{
			_sense = sense;
			Field glossField = WeSayWordsProject.Project.GetFieldFromDefaultViewTemplate(LexSense.WellKnownProperties.Gloss);
			if (glossField == null)
			{
				_glossMeaningField = false;
			}
			else
			{
				_glossMeaningField = glossField.IsMeaningField;
			}
		}

		public string SearchString
		{
			get { return _glossMeaningField ? _sense.Gloss.GetFirstAlternative() : _sense.Definition.GetFirstAlternative(); }//review
		}
	}
}

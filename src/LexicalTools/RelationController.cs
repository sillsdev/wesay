using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class RelationController
	{
		private readonly WeSayDataObject _target;
		private readonly LexRelationType _relationType;
		private readonly Field _field;
		private readonly IBindingList _allRecords;
		private readonly EventHandler<CurrentItemEventArgs>  _focusDelegate;
		private Control _control;
		private SimpleBinding<LexEntry> _binding;

		public static Control CreateWidget(WeSayDataObject target, LexRelationType relationType, Field field,
			IBindingList allRecords, EventHandler<CurrentItemEventArgs>  focus)
		{
			RelationController controller = new RelationController(target, relationType, field, allRecords,focus);
			return controller.Control;
		}

		private RelationController(WeSayDataObject target, LexRelationType relationType, Field field,
			IBindingList allRecords, EventHandler<CurrentItemEventArgs>  focus)
		{
			_target = target;
			_relationType = relationType;
			_field = field;
			_allRecords = allRecords;
			_focusDelegate = focus;

			MakeControl();
		}

		private void OnCreateNewEntry(object sender, AutoCompleteWithCreationBox<LexEntry>.CreateNewArgs e)
		{
			LexEntry newGuy = Lexicon.AddNewEntry();
			newGuy.LexicalForm.SetAlternative(_field.WritingSystemIds[0], e.LabelOfNewItem);
			e.NewlyCreatedItem = newGuy;
		}

		public Control Control
		{
			get
			{
				return _control;
			}
		}

		private void MakeControl()
		{
			//relations come to us in collections, even when they are atomic
			// this will get a collection if we already have some for this field, or else
			// it will make one. If unused, it will be cleaned up at the right time by the WeSayDataObject parent.
			LexRelationCollection targetRelationCollection =
				_target.GetOrCreateProperty<LexRelationCollection>(_field.FieldName);

			switch (_relationType.Multiplicity)
			{
				case LexRelationType.Multiplicities.One:
					LexRelation relation;
					if (targetRelationCollection.Relations.Count > 0)
					{
						relation = targetRelationCollection.Relations[0];
						relation.Parent = _target;
					}
					else
					{
						//we have to make one so we can show the control. It will be cleaned up, if not used, by the WeSayDataObject target
						relation = new LexRelation(_field.FieldName, string.Empty, _target);
						targetRelationCollection.Relations.Add(relation);
					}

					AutoCompleteWithCreationBox<LexEntry> picker = new AutoCompleteWithCreationBox<LexEntry>();
					picker.Box.ItemDisplayStringAdaptor = new WeSayDataObjectLabelAdaptor(_field.WritingSystemIds);
					picker.Box.TooltipToDisplayStringAdaptor =
						new WeSayDataObjectToolTipProvider(_field.WritingSystemIds);
					picker.Box.FormToObectFinder = FindLexEntryFromForm;
					picker.Box.ItemFilterer = ApproximateMatcher.FindClosestAndNextClosestAndPrefixedForms;
					picker.Box.PopupWidth = 100;
					picker.Box.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
					picker.Box.Tag = relation;
					//                    switch (type.TargetType)
					//                    {
					//                        case LexRelationType.TargetTypes.Entry:
					//                            //picker.Box.Items = Project.WeSayWordsProject.Project
					//                            break;
					//                        case LexRelationType.TargetTypes.Sense:
					//                            break;
					//                        default:
					//                            break;
					//                    }
					picker.Box.Items = _allRecords;

					picker.Box.WritingSystem = _field.WritingSystems[0]; //review:
					picker.Box.MinimumSize = new Size(40, 10);
					picker.Box.SelectedItem = relation.Target;
					if (picker.Box.SelectedItem == null && !string.IsNullOrEmpty(relation.TargetId))
					{
						picker.Box.Text = relation.TargetId;
					   // picker.Box.ShowRedSquiggle = true;
					}

					_binding = new SimpleBinding<LexEntry>(relation, picker);
					//for underlinging the relation in the preview pane
					_binding.CurrentItemChanged += _focusDelegate;// _detailList.OnBinding_ChangeOfWhichItemIsInFocus;

					picker.CreateNewClicked +=OnCreateNewEntry;
					_control= picker;
					break;
				case LexRelationType.Multiplicities.Many:
					_control=
						new ReferenceCollectionEditor<LexRelation>(targetRelationCollection.Relations,
																   _field.WritingSystemIds);
					break;
				default:
					break;
			}
		}
		public void AddChangeBinding(EventHandler<CurrentItemEventArgs> handler)
		{
			_binding.CurrentItemChanged += handler;
		}

		private class WeSayDataObjectLabelAdaptor : WeSay.Foundation.IDisplayStringAdaptor
		{
			private IList<string> _writingSystemIds;//review: should this really be an ordered collection of preferred choices?

			public WeSayDataObjectLabelAdaptor(IList<string> writingSystemIds)
			{
				_writingSystemIds = writingSystemIds;
			}

			public string GetDisplayLabel(object item)
			{
				if (item is LexEntry)
					return ((LexEntry)item).LexicalForm.GetBestAlternative(_writingSystemIds);
				if (item is LexSense)
				{
					LexSense sense = (LexSense)item;
					return GetDisplayLabel(sense.Parent) + "." + sense.Gloss.GetBestAlternative(_writingSystemIds);
				}
				return "Program error";
			}
		}

		private class WeSayDataObjectToolTipProvider : WeSay.Foundation.IDisplayStringAdaptor
		{
			private IList<string> _writingSystemIds;//review: should this really be an ordered collection of preferred choices?

			public WeSayDataObjectToolTipProvider(IList<string> writingSystemIds)
			{
				_writingSystemIds = writingSystemIds;
			}

			public string GetDisplayLabel(object item)
			{
				if (item is LexEntry)
					return ((LexEntry)item).GetToolTipText();
				if (item is LexSense)
				{
					LexSense sense = (LexSense)item;
					return "What to show for senses?";
				}
				return "Program error";
			}
		}

		private object FindLexEntryFromForm(string form)
		{
			int index = ((CachedSortedDb4oList<string, LexEntry>)_allRecords).BinarySearch(form);

			if (index >= 0)
			{
				return ((CachedSortedDb4oList<string, LexEntry>)_allRecords).GetValue(index);
			}
			return null;
		}

	}
}

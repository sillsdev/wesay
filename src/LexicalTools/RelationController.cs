using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.UiBindings;
using Palaso.Reporting;
using Palaso.Text;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools
{
	public class RelationController
	{
		private readonly LexEntryRepository _lexEntryRepository;
		private readonly Field _field;
		private readonly EventHandler<CurrentItemEventArgs> _focusDelegate;
		private readonly LexRelationType _relationType;
		private readonly PalasoDataObject _relationParent;
		private SimpleBinding<string> _binding;
		private Control _control;
		private ResultSet<LexEntry> _resultSet;

		private RelationController(PalasoDataObject relationParent,
								   LexRelationType relationType,
								   Field field,
								   LexEntryRepository lexEntryRepository,
								   EventHandler<CurrentItemEventArgs> focus)
		{
			_relationParent = relationParent;
			_relationType = relationType;
			_field = field;
			_lexEntryRepository = lexEntryRepository;
			_focusDelegate = focus;

			MakeControl();
		}

		public Control Control
		{
			get { return _control; }
		}

		public static Control CreateWidget(PalasoDataObject relationParent,
										   LexRelationType relationType,
										   Field field,
										   LexEntryRepository lexEntryRepository,
										   EventHandler<CurrentItemEventArgs> focus)
		{
			if (field.WritingSystemIds.Count == 0)
			{
				throw new ConfigurationException("The field {0} has no writing systems enabled.", field.FieldName);
			}
			var controller = new RelationController(relationParent,
																   relationType,
																   field,
																   lexEntryRepository,
																   focus);
			return controller.Control;
		}

		//private void OnCreateNewLexEntry(object sender,
		//                                      CreateNewArgs e)
		//{
		//    LexEntry newGuy = CreateNewLexEntry(e);
		//    e.NewlyCreatedItem = newGuy;

		//}

		private void OnCreateNewPairStringLexEntryId(object sender, CreateNewArgs e)
		{
			LexEntry newGuy = CreateNewLexEntry(e);
			WritingSystem writingSystem = GetWritingSystemFromField();
			_lexEntryRepository.NotifyThatLexEntryHasBeenUpdated(newGuy);
			_resultSet = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(writingSystem);
			e.NewlyCreatedItem = GetRecordTokenFromLexEntry(newGuy);
		}

		private LexEntry CreateNewLexEntry(CreateNewArgs e)
		{
			LexEntry newGuy = _lexEntryRepository.CreateItem();
			newGuy.LexicalForm.SetAlternative(_field.WritingSystemIds[0], e.LabelOfNewItem);
			//hack: if something is a baseform itself, it isn't likely to have its own baseform
			//This satisfies Rene's request of WS-419
			if (_field.FieldName == "BaseForm")
			{
				newGuy.SetFlag(LexEntry.WellKnownProperties.FlagSkipBaseform);
			}
			return newGuy;
		}

		private void MakeControl()
		{
			//relations come to us in collections, even when they are atomic
			// this will get a collection if we already have some for this field, or else
			// it will make one. If unused, it will be cleaned up at the right time by the PalasoDataObject parent.
			var targetRelationCollection =
					_relationParent.GetOrCreateProperty<LexRelationCollection>(_field.FieldName);

			switch (_relationType.Multiplicity)
			{
				case LexRelationType.Multiplicities.One:
					LexRelation relation;
					if (targetRelationCollection.Relations.Count > 0)
					{
						relation = targetRelationCollection.Relations[0];
						relation.Parent = _relationParent;
					}
					else
					{
						//we have to make one so we can show the control. It will be cleaned up, if not used, by the PalasoDataObject target
						relation = new LexRelation(_field.FieldName, string.Empty, _relationParent);
						targetRelationCollection.Relations.Add(relation);
					}

					InitializeRelationControl(relation);

					break;
					//                case LexRelationType.Multiplicities.Many:
					//                    _control = new ReferenceCollectionEditor<LexRelation>
					//                                    (targetRelationCollection.Relations,
					//                                     _field.WritingSystemIds);
					//                    break;
				default:
					break;
			}
		}

		private void InitializeRelationControl(LexRelation relation)
		{
			//TODO: refactor this (sortHelper, pairStringLexEntryIdList, _keyIdMap, GetKeyIdPairFromLexEntry)
			//      to use ApproximateFinder. Eventually refactor the automcompletetextbox to just take one

			WritingSystem writingSystem = GetWritingSystemFromField();
			ResultSet<LexEntry> recordTokenList =
					_lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(writingSystem);
			_resultSet = recordTokenList;

			AutoCompleteWithCreationBox<RecordToken<LexEntry>, string> picker =
					CreatePicker<RecordToken<LexEntry>>(relation);
			picker.GetKeyValueFromValue = GetRecordTokenFromTargetId;
			picker.GetValueFromKeyValue = GetTargetIdFromRecordToken;

			picker.Box.ItemDisplayStringAdaptor = new PairStringLexEntryIdDisplayProvider();
			picker.Box.FormToObectFinder = FindRecordTokenFromForm;
			picker.Box.ItemFilterer = FindClosestAndNextClosestAndPrefixedPairStringLexEntryForms;

			picker.Box.Items = recordTokenList;
			if (!String.IsNullOrEmpty(relation.TargetId))
			{
				picker.Box.SelectedItem = GetRecordTokenFromLexEntry(_lexEntryRepository.GetLexEntryWithMatchingId(relation.TargetId));
			}

			picker.CreateNewClicked += OnCreateNewPairStringLexEntryId;
			_control = picker;
		}

		private WritingSystem GetWritingSystemFromField()
		{
			string firstWsId = _field.WritingSystemIds[0];
			return BasilProject.Project.WritingSystems.Get(firstWsId);
		}

		//private static LexEntry Identity(LexEntry e) {
		//    return e;
		//}

		private RecordToken<LexEntry> GetRecordTokenFromLexEntry(LexEntry e)
		{
			if (e == null)
			{
				return null;
			}
			int index = _resultSet.FindFirstIndex(e);
			if (index < 0)
			{
				return null;
			}
			return _resultSet[index];
		}

		private RecordToken<LexEntry> GetRecordTokenFromTargetId(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			LexEntry lexEntry = _lexEntryRepository.GetLexEntryWithMatchingId(s);
			if (lexEntry == null)
			{
				return null;
			}
			return GetRecordTokenFromLexEntry(lexEntry);
		}

		private static string GetTargetIdFromRecordToken(RecordToken<LexEntry> e)
		{
			return e.RealObject.Id;
		}

		private AutoCompleteWithCreationBox<T, string> CreatePicker<T>(LexRelation relation)
				where T : class
		{
			var picker = new AutoCompleteWithCreationBox<T, string> (
				CommonEnumerations.VisibilitySetting.Visible
			);
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
			picker.Box.WritingSystem = GetWritingSystemFromField();
			picker.Box.PopupWidth = 200;

			//review:
			picker.Box.MinimumSize = new Size(40, 10);
			if (picker.Box.SelectedItem == null && !string.IsNullOrEmpty(relation.Key))
			{
				picker.Box.Text = relation.Key;
				// picker.Box.ShowRedSquiggle = true;
			}

			_binding = new SimpleBinding<string>(relation, picker);
			//for underlinging the relation in the preview pane
			_binding.CurrentItemChanged += _focusDelegate;

			return picker;
		}

		private static IEnumerable FindClosestAndNextClosestAndPrefixedPairStringLexEntryForms(
				string text, IEnumerable items, IDisplayStringAdaptor adaptor)
		{
			return ApproximateMatcher.FindClosestForms<object>(items,
															   adaptor.GetDisplayLabel,
															   text,
															   ApproximateMatcherOptions.
																	   IncludePrefixedAndNextClosestForms);
		}

		//private static IEnumerable FindClosestAndNextClosestAndPrefixedLexEntryForms(string text, IEnumerable items, IDisplayStringAdaptor adaptor)
		//{
		//    return ApproximateMatcher.FindClosestForms<LexEntry>(items,
		//                                                       adaptor.GetDisplayLabel,
		//                                                       text,
		//                                                       ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
		//}

		public void AddChangeBinding(EventHandler<CurrentItemEventArgs> handler)
		{
			_binding.CurrentItemChanged += handler;
		}

		private object FindRecordTokenFromForm(string form)
		{
			return
					_resultSet.FindFirst(
							delegate(RecordToken<LexEntry> token) { return (string) token["Form"] == form; });
		}

		//        #region Nested type: PalasoDataObjectLabelAdaptor
		//
		//        private class PairStringLexEntryIdLabelAdaptor : IDisplayStringAdaptor
		//        {
		//            #region IDisplayStringAdaptor Members
		//
		//            public string GetDisplayLabel(object item)
		//            {
		//                KeyValuePair<string, long> kv = (KeyValuePair<string,long>)item;
		//                return kv.Key;
		//            }
		//
		//            #endregion
		//        }
		//
		//        #endregion
	}
}
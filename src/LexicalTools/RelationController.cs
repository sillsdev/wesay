using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
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
		private readonly WeSayDataObject _relationParent;
		private SimpleBinding<string> _binding;
		private Control _control;
		private List<RecordToken<LexEntry>> _recordTokenList;

		private RelationController(WeSayDataObject relationParent,
								   LexRelationType relationType,
								   Field field,
								   LexEntryRepository lexEntryRepository,
								   EventHandler<CurrentItemEventArgs> focus)
		{
			this._relationParent = relationParent;
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

		public static Control CreateWidget(WeSayDataObject relationParent,
										   LexRelationType relationType,
										   Field field,
										   LexEntryRepository lexEntryRepository,
										   EventHandler<CurrentItemEventArgs>
												   focus)
		{
			RelationController controller =
					new RelationController(relationParent,
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

		private void OnCreateNewPairStringLexEntryId(object sender,
									  CreateNewArgs e)
		{
			LexEntry newGuy = CreateNewLexEntry(e);
			e.NewlyCreatedItem = GetRecordTokenFromLexEntry(newGuy);
		}

		private LexEntry CreateNewLexEntry(CreateNewArgs e) {
			LexEntry newGuy = _lexEntryRepository.CreateItem();
			newGuy.LexicalForm.SetAlternative(this._field.WritingSystemIds[0],
											  e.LabelOfNewItem);
			//hack: if something is a baseform itself, it isn't likely to have its own baseform
			//This satisfies Rene's request of WS-419
			if (this._field.FieldName == "BaseForm")
			{
				newGuy.SetFlag("flag_skip_BaseForm");
			}
			return newGuy;
		}

		private void MakeControl()
		{
			//relations come to us in collections, even when they are atomic
			// this will get a collection if we already have some for this field, or else
			// it will make one. If unused, it will be cleaned up at the right time by the WeSayDataObject parent.
			LexRelationCollection targetRelationCollection =
					this._relationParent.GetOrCreateProperty<LexRelationCollection>(
							_field.FieldName);

			switch (_relationType.Multiplicity)
			{
				case LexRelationType.Multiplicities.One:
					LexRelation relation;
					if (targetRelationCollection.Relations.Count > 0)
					{
						relation = targetRelationCollection.Relations[0];
						relation.Parent = this._relationParent;
					}
					else
					{
						//we have to make one so we can show the control. It will be cleaned up, if not used, by the WeSayDataObject target
						relation =
								new LexRelation(_field.FieldName,
												string.Empty,
												this._relationParent);
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

		private void InitializeRelationControl(LexRelation relation) {
			//TODO: refactor this (sortHelper, pairStringLexEntryIdList, _keyIdMap, GetKeyIdPairFromLexEntry)
			//      to use ApproximateFinder. Eventually refactor the automcompletetextbox to just take one

			List<RecordToken<LexEntry>> recordTokenList = this._lexEntryRepository.GetAllEntriesSortedByLexicalForm(this._field.WritingSystems[0]);
			this._recordTokenList = recordTokenList;

			AutoCompleteWithCreationBox<RecordToken<LexEntry>, string> picker = CreatePicker<RecordToken<LexEntry>>(relation);
			picker.GetKeyValueFromValue = GetRecordTokenFromTargetId;
			picker.GetValueFromKeyValue = GetTargetIdFromRecordToken;

			picker.Box.ItemDisplayStringAdaptor =
					new PairStringLexEntryIdDisplayProvider(this._lexEntryRepository);
			picker.Box.FormToObectFinder = FindRecordTokenFromForm;
			picker.Box.ItemFilterer = FindClosestAndNextClosestAndPrefixedPairStringLexEntryForms;

			picker.Box.Items = recordTokenList;
			picker.Box.SelectedItem = GetRecordTokenFromLexEntry(relation.GetTarget(_lexEntryRepository));

			picker.CreateNewClicked += OnCreateNewPairStringLexEntryId;
			this._control = picker;
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
			RepositoryId id = this._lexEntryRepository.GetId(e);
			List<RecordToken<LexEntry>> recordTokenList = this._lexEntryRepository.GetAllEntriesSortedByLexicalForm(this._field.WritingSystems[0]);
			return recordTokenList.Find(delegate(RecordToken<LexEntry> token)
										{
											return token.Id == id;
										});
		}

		private RecordToken<LexEntry> GetRecordTokenFromTargetId(string s)
		{
			if (s == null)
			{
				return null;
			}
			LexEntry lexEntry = this._lexEntryRepository.GetLexEntryWithMatchingId(s);
			RepositoryId id = this._lexEntryRepository.GetId(lexEntry);
			List<RecordToken<LexEntry>> recordTokenList = this._lexEntryRepository.GetAllEntriesSortedByLexicalForm(this._field.WritingSystems[0]);
			return recordTokenList.Find(delegate(RecordToken<LexEntry> token)
										{
											return token.Id == id;
										});
		}

		private string GetTargetIdFromRecordToken(RecordToken<LexEntry> e)
		{
			return _lexEntryRepository.GetItem(e).Id;
		}

		private AutoCompleteWithCreationBox<T, string> CreatePicker<T>(LexRelation relation) where T:class
		{
			AutoCompleteWithCreationBox<T, string> picker = new AutoCompleteWithCreationBox<T, string>(CommonEnumerations.VisibilitySetting.Visible);
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
			picker.Box.WritingSystem = this._field.WritingSystems[0];

			//review:
			picker.Box.MinimumSize = new Size(40, 10);
			if (picker.Box.SelectedItem == null &&
				!string.IsNullOrEmpty(relation.Key))
			{
				picker.Box.Text = relation.Key;
				// picker.Box.ShowRedSquiggle = true;
			}

			_binding = new SimpleBinding<string>(relation, picker);
			//for underlinging the relation in the preview pane
			_binding.CurrentItemChanged += _focusDelegate;

			return picker;
		}


		private static IEnumerable FindClosestAndNextClosestAndPrefixedPairStringLexEntryForms(string text, IEnumerable items, IDisplayStringAdaptor adaptor)
		{
			return ApproximateMatcher.FindClosestForms<object>(items,
															   adaptor.GetDisplayLabel,
															   text,
															   ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
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
			int index = RecordToken<LexEntry>.FindFirstWithDisplayString(_recordTokenList, form);
			if (index >= 0)
			{
				return _recordTokenList[index];
			}
			return null;
		}





//        #region Nested type: WeSayDataObjectLabelAdaptor
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

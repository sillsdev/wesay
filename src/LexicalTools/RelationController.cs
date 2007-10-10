using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools
{
	public class RelationController
	{
		private readonly IRecordListManager _recordListManager;
		private readonly Field _field;
		private readonly EventHandler<CurrentItemEventArgs> _focusDelegate;
		private readonly LexRelationType _relationType;
		private readonly WeSayDataObject _target;
		private SimpleBinding<LexEntry> _binding;
		private Control _control;
		private CachedSortedDb4oList<string, LexEntry> _pairStringLexEntryIdList;
		private ReadOnlyCollection<KeyValuePair<string, long>> _keyIdMap;

		private RelationController(WeSayDataObject target,
								   LexRelationType relationType,
								   Field field,
								   IRecordListManager recordListManager,
								   EventHandler<CurrentItemEventArgs> focus)
		{
			_target = target;
			_relationType = relationType;
			_field = field;
			_recordListManager = recordListManager;
			_focusDelegate = focus;

			MakeControl();
		}

		public Control Control
		{
			get { return _control; }
		}

		public static Control CreateWidget(WeSayDataObject target,
										   LexRelationType relationType,
										   Field field,
										   IRecordListManager recordListManager,
										   EventHandler<CurrentItemEventArgs>
												   focus)
		{
			RelationController controller =
					new RelationController(target,
										   relationType,
										   field,
										   recordListManager,
										   focus);
			return controller.Control;
		}

		private void OnCreateNewEntry(object sender,
											  CreateNewArgs e)
		{
			LexEntry newGuy = Lexicon.AddNewEntry();
			newGuy.LexicalForm.SetAlternative(_field.WritingSystemIds[0],
											  e.LabelOfNewItem);
			e.NewlyCreatedItem = newGuy;
		}

		private void MakeControl()
		{
			//relations come to us in collections, even when they are atomic
			// this will get a collection if we already have some for this field, or else
			// it will make one. If unused, it will be cleaned up at the right time by the WeSayDataObject parent.
			LexRelationCollection targetRelationCollection =
					_target.GetOrCreateProperty<LexRelationCollection>(
							_field.FieldName);

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
						relation =
								new LexRelation(_field.FieldName,
												string.Empty,
												_target);
						targetRelationCollection.Relations.Add(relation);
					}

					InitializeRelationControl(relation);

					break;
				case LexRelationType.Multiplicities.Many:
					_control = new ReferenceCollectionEditor<LexRelation>
									(targetRelationCollection.Relations,
									 _field.WritingSystemIds);
					break;
				default:
					break;
			}
		}

		private void InitializeRelationControl(LexRelation relation) {
			Db4oRecordListManager recordListManager =
					this._recordListManager as Db4oRecordListManager;
			if (recordListManager != null)
			{
				LexEntrySortHelper sortHelper =
						new LexEntrySortHelper(
								recordListManager.DataSource,
								this._field.WritingSystems[0],
								true);
				CachedSortedDb4oList<string, LexEntry> pairStringLexEntryIdList = recordListManager.GetSortedList(sortHelper);
				this._pairStringLexEntryIdList = pairStringLexEntryIdList;
				this._keyIdMap = this._pairStringLexEntryIdList.KeyIdMap;

				AutoCompleteWithCreationBox<object, LexEntry> picker = CreatePicker<object>(relation);
				picker.GetKeyValueFromValue = GetKeyIdPairFromLexEntry;
				picker.GetValueFromKeyValue = GetLexEntryFromKeyIdPair;

				picker.Box.Items = this._keyIdMap;
				picker.Box.SelectedItem = relation.Target;

				picker.Box.ItemDisplayStringAdaptor =
						new PairStringLexEntryIdLabelAdaptor(this._field.WritingSystemIds, pairStringLexEntryIdList);
				picker.Box.TooltipToDisplayStringAdaptor =
						new PairStringLexEntryIdToolTipProvider( pairStringLexEntryIdList);
				picker.Box.FormToObectFinder = FindStringLexEntryFromForm;
				picker.Box.ItemFilterer = FindClosestAndNextClosestAndPrefixedPairStringLexEntryForms;
				picker.CreateNewClicked += OnCreateNewEntry;
				this._control = picker;
			}
			else
			{
				AutoCompleteWithCreationBox<LexEntry, LexEntry> picker = CreatePicker<LexEntry>(relation);
				picker.GetKeyValueFromValue = Identity;
				picker.GetValueFromKeyValue = Identity;
				picker.Box.Items = this._recordListManager.GetListOfType<LexEntry>();
				picker.Box.SelectedItem = relation.Target;

				picker.Box.ItemDisplayStringAdaptor =
						new WeSayDataObjectLabelAdaptor(this._field.WritingSystemIds);
				picker.Box.TooltipToDisplayStringAdaptor =
						new WeSayDataObjectToolTipProvider(this._field.WritingSystemIds);
				picker.Box.ItemFilterer = FindClosestAndNextClosestAndPrefixedLexEntryForms;
				picker.CreateNewClicked += OnCreateNewEntry;
				this._control = picker;
			}
		}

		private static LexEntry Identity(LexEntry e) {
			return e;
		}

		private object GetKeyIdPairFromLexEntry(LexEntry e) {
			int i = this._pairStringLexEntryIdList.IndexOf(e);
			if(i<0)
			{
				return null;
			}
			return this._keyIdMap[i];
		}

		private LexEntry GetLexEntryFromKeyIdPair(object e) {
			KeyValuePair<string, long> kv =(KeyValuePair<string,long>) e;
			return this._pairStringLexEntryIdList.GetValueFromId(kv.Value);
		}

		private AutoCompleteWithCreationBox<T, LexEntry> CreatePicker<T>(LexRelation relation) where T:class
		{
			AutoCompleteWithCreationBox<T, LexEntry> picker = new AutoCompleteWithCreationBox<T, LexEntry>();
			picker.Box.PopupWidth = 100;
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
				!string.IsNullOrEmpty(relation.TargetId))
			{
				picker.Box.Text = relation.TargetId;
				// picker.Box.ShowRedSquiggle = true;
			}

			_binding = new SimpleBinding<LexEntry>(relation, picker);
			//for underlinging the relation in the preview pane
			_binding.CurrentItemChanged += _focusDelegate;
			// _detailList.OnBinding_ChangeOfWhichItemIsInFocus;


			return picker;
		}


		private static IEnumerable FindClosestAndNextClosestAndPrefixedPairStringLexEntryForms(string text, IEnumerable items, IDisplayStringAdaptor adaptor)
		{
			return ApproximateMatcher.FindClosestForms<object>(items,
															   adaptor.GetDisplayLabel,
															   text,
															   ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
		}


		private static IEnumerable FindClosestAndNextClosestAndPrefixedLexEntryForms(string text, IEnumerable items, IDisplayStringAdaptor adaptor)
		{
			return ApproximateMatcher.FindClosestForms<LexEntry>(items,
															   adaptor.GetDisplayLabel,
															   text,
															   ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
		}


		public void AddChangeBinding(EventHandler<CurrentItemEventArgs> handler)
		{
			_binding.CurrentItemChanged += handler;
		}

		private object FindStringLexEntryFromForm(string form)
		{
			int index = _pairStringLexEntryIdList.
							BinarySearch(form);

			if (index >= 0)
			{
				return this._keyIdMap[index];
			}
			return null;
		}

		#region Nested type: WeSayDataObjectLabelAdaptor

		private class WeSayDataObjectLabelAdaptor: IDisplayStringAdaptor
		{
			private readonly IList<string> _writingSystemIds;
			//review: should this really be an ordered collection of preferred choices?

			public WeSayDataObjectLabelAdaptor(IList<string> writingSystemIds)
			{
				_writingSystemIds = writingSystemIds;
			}

			#region IDisplayStringAdaptor Members

			public string GetDisplayLabel(object item)
			{
				if (item is LexEntry)
				{
					return
							((LexEntry) item).LexicalForm.GetBestAlternativeString(
									_writingSystemIds);
				}
				if (item is LexSense)
				{
					LexSense sense = (LexSense) item;
					return
							GetDisplayLabel(sense.Parent) + "." +
							sense.Gloss.GetBestAlternativeString(_writingSystemIds);
				}
				return "Program error";
			}

			#endregion
		}

		#endregion

		#region Nested type: WeSayDataObjectToolTipProvider

		private class WeSayDataObjectToolTipProvider: IDisplayStringAdaptor
		{
			private IList<string> _writingSystemIds;
			//review: should this really be an ordered collection of preferred choices?

			public WeSayDataObjectToolTipProvider(IList<string> writingSystemIds)
			{
				_writingSystemIds = writingSystemIds;
			}

			#region IDisplayStringAdaptor Members

			public string GetDisplayLabel(object item)
			{
				if (item is LexEntry)
				{
					return ((LexEntry) item).GetToolTipText();
				}
				if (item is LexSense)
				{
					LexSense sense = (LexSense) item;
					return "What to show for senses?";
				}
				return "Program error";
			}

			#endregion
		}

		#endregion

		#region Nested type: WeSayDataObjectLabelAdaptor

		private class PairStringLexEntryIdLabelAdaptor : IDisplayStringAdaptor
		{
			private readonly IList<string> _writingSystemIds;
			//review: should this really be an ordered collection of preferred choices?

			private readonly CachedSortedDb4oList<string, LexEntry> _cachedSortedDb4oList;

			public PairStringLexEntryIdLabelAdaptor(IList<string> writingSystemIds,
			   CachedSortedDb4oList<string, LexEntry> cachedSortedDb4oList)
			{
				_writingSystemIds = writingSystemIds;
				_cachedSortedDb4oList = cachedSortedDb4oList;
			}

			#region IDisplayStringAdaptor Members

			public string GetDisplayLabel(object item)
			{
				KeyValuePair<string, long> kv = (KeyValuePair<string,long>)item;
				return kv.Key;
// Review: Eric had these two lines in, but they won't be run...
//                LexEntry entry = this._cachedSortedDb4oList.GetValueFromId(kv.Value);
//                return entry.LexicalForm.GetBestAlternativeString(_writingSystemIds);
			}

			#endregion
		}

		#endregion

		#region Nested type: WeSayDataObjectToolTipProvider

		private class PairStringLexEntryIdToolTipProvider : IDisplayStringAdaptor
		{
			private readonly CachedSortedDb4oList<string, LexEntry> _cachedSortedDb4oList;

			public PairStringLexEntryIdToolTipProvider(CachedSortedDb4oList<string, LexEntry> cachedSortedDb4oList)
			{
				_cachedSortedDb4oList = cachedSortedDb4oList;
			}

			#region IDisplayStringAdaptor Members

			public string GetDisplayLabel(object item)
			{
				KeyValuePair<string, long> kv = (KeyValuePair<string, long>)item;
				LexEntry entry = this._cachedSortedDb4oList.GetValueFromId(kv.Value);
				return entry.GetToolTipText();
			}

			#endregion
		}

		#endregion

	}
}

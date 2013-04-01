using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Palaso.Code;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.Progress;
using Palaso.Reporting;
using Palaso.Text;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.LexicalTools.GatherByWordList
{
	public class GatherWordListTask: WordGatheringTaskBase
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly string _lexemeFormListFileName;
		private GatherWordListControl _gatherControl;
		private List<LexEntry> _words;
		private int _currentWordIndex;
		private readonly string _preferredPromptingWritingSystemId;
		private readonly WritingSystemDefinition _lexicalUnitWritingSystem;
		private IList<string> _definitionWritingSystemIds;
		private IList<string> _glossWritingSystemIds;
		private bool _usingLiftFile;
		public string LoadFailureMessage;

		public GatherWordListTask(IGatherWordListConfig config,
									LexEntryRepository lexEntryRepository,
								  ViewTemplate viewTemplate,
			 TaskMemoryRepository taskMemoryRepository)

				: base(config, lexEntryRepository, viewTemplate, taskMemoryRepository)
		{
			_viewTemplate = viewTemplate;
			Guard.AgainstNull(config.WordListFileName, "config.WordListFileName");
			Guard.AgainstNull(config.WordListWritingSystemIdOfOldFlatWordList, "config.WordListWritingSystemIdOfOldFlatWordList");
			Guard.AgainstNull(viewTemplate, "viewTemplate");

			//enhance: this isn't really true anymore, as we're moving to wordpack (where it's the folder)
			//for now, this is figure out more carefully in GetPathToUse
			_usingLiftFile =  ".lift"==Path.GetExtension(config.WordListFileName).ToLower();

			_lexicalUnitWritingSystem =
				viewTemplate.GetDefaultWritingSystemForField(Field.FieldNames.EntryLexicalForm.ToString());
			_lexemeFormListFileName = config.WordListFileName;
			_words = null;
			_preferredPromptingWritingSystemId = config.WordListWritingSystemIdOfOldFlatWordList;
			var f = viewTemplate.GetField(LexSense.WellKnownProperties.Definition);
			Guard.AgainstNull(f, "No field for definition");
			_definitionWritingSystemIds = f.WritingSystemIds;

			 f = viewTemplate.GetField(LexSense.WellKnownProperties.Gloss);
			if(f!=null)
				_glossWritingSystemIds = f.WritingSystemIds;
			else
			{
				_glossWritingSystemIds = new List<string>();
			}
			if (_definitionWritingSystemIds.Count > 0)
				_preferredPromptingWritingSystemId = _definitionWritingSystemIds[0];
		}

		public WritingSystemDefinition PromptingWritingSystem
		{
			get
			{
				if (!_viewTemplate.WritingSystems.Contains(_preferredPromptingWritingSystemId))
				{
					return _viewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Definition);//shouldn't ever happen
				}
				return _viewTemplate.WritingSystems.Get(_preferredPromptingWritingSystemId);
			}
		}

		private void LoadWordList()
		{
			string pathToUse = GetPathToUse();
			if(pathToUse == null)
				return;

			_words = new List<LexEntry>();
			if (_usingLiftFile)
			{
				LoadLift(pathToUse);
			}
			else
			{
				LoadSimpleList(pathToUse);
			}

			NavigateFirstToShow();
		}

		/// <summary>
		/// all we know at this point is the name of the wordlist. It could be found in various places
		/// </summary>
		/// <returns></returns>
		private string GetPathToUse()
		{
			//back before wordpacks, when wordlists were just a txt file, the didn't get a whole
			//folder to themselves.
			string pathInProject =
				Path.Combine(
					WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject,
					_lexemeFormListFileName);
			if (File.Exists(pathInProject))
				return pathInProject;

			//ok, if it wasn't an old-style list, it should be a lift wordpack
			_usingLiftFile = true;

			//because wordpacks came along after several years, we can expect there are some
			//configs which list the .lift, and some don't, so it's easier to be tolerant here
			//than to fix each one.
			var wordpackFolderName = _lexemeFormListFileName.Replace(".lift", "");

			//look  for a custom wordpack in the project folder
			string wordpackDirectory = Path.Combine(WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject,
								wordpackFolderName);
			string pathInProjectOwnFolder = Path.Combine(wordpackDirectory, wordpackFolderName + ".lift");
			if (File.Exists(pathInProjectOwnFolder))
				return pathInProject;

			//look for a factory wordpack in the program folder (during development, in the "common/wordpacks" folder)
			wordpackDirectory = Path.Combine(BasilProject.ApplicationCommonDirectory,
								"wordpacks");
			wordpackDirectory = Path.Combine(wordpackDirectory,
								wordpackFolderName);
			string pathInProgramDirOwnFolder = Path.Combine(wordpackDirectory,
								wordpackFolderName+".lift");
			if (File.Exists(pathInProgramDirOwnFolder))
				return pathInProgramDirOwnFolder;

			ErrorReport.NotifyUserOfProblem(
				"WeSay could not find the wordlist.  It expected to find it either at {0}, {1}, or {2}.",
				pathInProject, //legacy
				pathInProjectOwnFolder,
				pathInProgramDirOwnFolder);
			return null;
		}

		private void LoadLift(string path)
		{
			//Performance wise, the following is not expecting a huge, 10k word list.

			using (var reader = new Palaso.DictionaryServices.Lift.LiftReader(new NullProgressState(),
				WeSayWordsProject.Project.GetSemanticDomainsList(),
				WeSayWordsProject.Project.GetIdsOfSingleOptionFields()))
			using(var m = new MemoryDataMapper<LexEntry>())
			{
				reader.Read(path, m);
				_words.AddRange(from RepositoryId repositoryId in m.GetAllItems() select m.GetItem(repositoryId));
			}
		}

		private void LoadSimpleList(string path)
		{
			using (TextReader r = File.OpenText(path))
			{
				do
				{
					string s = r.ReadLine();
					if (s == null)
					{
						break;
					}
					s = s.Trim();

					if (!string.IsNullOrEmpty(s))//skip blank lines
					{
						var entry = new LexEntry();
						entry.LexicalForm.SetAlternative(_preferredPromptingWritingSystemId, s);
						var sense = new LexSense(entry);
						sense.Gloss.SetAlternative(_preferredPromptingWritingSystemId, s);
						entry.Senses.Add(sense);
						_words.Add(entry);
					}
				}
				while (true);
			}
		}

		public bool IsTaskComplete
		{
			get
			{
				if (_words != null)
				{
					return CurrentIndexIntoWordlist >= _words.Count;
				}
				return true;
			}
		}

		/// <summary>
		/// The GatherWordListControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				if (_gatherControl == null)
				{
					_gatherControl = new GatherWordListControl(this, _lexicalUnitWritingSystem);
				}
				return _gatherControl;
			}
		}

		public string CurrentPromptingForm
		{
			get
			{
				var form = CurrentPromptingLanguageForm;
				if(form!=null)
					return form.Form;
				return string.Empty;
			}
		}
		public LanguageForm CurrentPromptingLanguageForm
		{
			get
			{
				//note, we choose to skip this word if it doesn't have any of the same languages as our
				//current definition field.  But we don't skip it just because it doesn't have our
				//preferred (first) one.
				var preferred = new string[]{_definitionWritingSystemIds.First()};
				var sense = CurrentTemplateSense;
				LanguageForm lf=null;
				if (sense != null)
					lf = sense.Gloss.GetBestAlternative(preferred);
				if(lf==null && sense!=null)
					lf = sense.Definition.GetBestAlternative(preferred);

				//ok, if we didn't get the best one, how about any others in the def field?
				if (lf == null)
					lf = CurrentTemplateLexicalEntry.LexicalForm.GetBestAlternative(_definitionWritingSystemIds);
				if (lf == null && sense != null)
					lf = sense.Gloss.GetBestAlternative(_definitionWritingSystemIds);
				if (lf == null && sense != null)
					lf = sense.Definition.GetBestAlternative(preferred);

				return lf;
			}
		}

		public WritingSystemDefinition GetWritingSystemOfLanguageForm(LanguageForm languageForm)
		{
			if(!_viewTemplate.WritingSystems.Contains(languageForm.WritingSystemId))
			{
				return null;
			}
			return _viewTemplate.WritingSystems.Get(languageForm.WritingSystemId);
		}

		public LexSense CurrentTemplateSense
		{
			get
			{
				return CurrentTemplateLexicalEntry.Senses.FirstOrDefault();
			}
		}
		public LexEntry CurrentTemplateLexicalEntry
		{
			get
			{
				Guard.Against(CurrentIndexIntoWordlist >= _words.Count, "CurrentIndexIntoWordlist must be < _words.Count");
				Guard.Against(_words.Count == 0, "There are no words in this list.");
				return _words[CurrentIndexIntoWordlist];
			}
		}

		public bool CanNavigateNext
		{
			get
			{
				if (_words == null)
				{
					return false;
				}
				return _words.Count > CurrentIndexIntoWordlist;
			}
		}

//        protected bool SomeFurtherWordWillHaveSomeElligbleWritingSystem
//        {
//            get
//            {
//                for (int i = 1+CurrentIndexIntoWordlist; i < _words.Count;i++ )
//                {
//                    if(GetWordHasElligibleWritingSystem(_words[i]))
//                        return true;
//                }
//                return false;
//            }
//        }

		private bool GetWordHasElligibleWritingSystem(LexEntry entry)
		{
			var sense = CurrentTemplateLexicalEntry.Senses.FirstOrDefault();
			foreach (var id in _definitionWritingSystemIds)
			{
				if ((entry.LexicalForm!=null && entry.LexicalForm.ContainsAlternative(id))
					|| (sense!=null && sense.Gloss!=null && sense.Gloss.ContainsAlternative(id))
					|| (sense!=null && sense.Definition!=null &&sense.Definition.ContainsAlternative(id)))
					return true;
			}
			return false;
		}

		public bool CanNavigatePrevious
		{
			get { return CurrentIndexIntoWordlist > 0; }
		}

		public int CurrentIndexIntoWordlist
		{
			get { return _currentWordIndex; }
			set
			{
				Guard.Against(value < 0, "_currentWordIndex must be > 0");
				_currentWordIndex = value;

				//nb: (CurrentWordIndex == _words.Count) is used to mark the "all done" state:

				//                if (!_suspendNotificationOfNavigation && UpdateSourceWord != null)
				//                {
				//                    UpdateSourceWord.Invoke(this, null);
				//                }
			}
		}

		public override void Activate()
		{

			if (!_usingLiftFile &&
					!WeSayWordsProject.Project.WritingSystems.Contains(
							 _preferredPromptingWritingSystemId))
			{
				ErrorReport.NotifyUserOfProblem(
						"The input system of the words in the word list will be used to add reversals and definitions.  Therefore, it needs to be in the list of input systems for this project.  Either change the input system that this task uses for the word list (currently '{0}') or add an input system with this id to the project.",
						_preferredPromptingWritingSystemId);
			}

			if (_words == null)
			{
				Cursor.Current = Cursors.WaitCursor;
				LoadWordList();
			}
			base.Activate();
		}

		/// <summary>
		/// Someday, we may indeed have multi-string foreign words
		/// </summary>
		public MultiText CurrentWordAsMultiText
		{
			get
			{
				var m = new MultiText();
				m.SetAlternative(_preferredPromptingWritingSystemId, CurrentPromptingForm);
				return m;
			}
		}

		public IList Words
		{
			get { return _words; }

		}

		public void WordCollected(MultiText newVernacularWord)
		{
//            var sense = new LexSense();
//            sense.Definition.MergeIn(CurrentWordAsMultiText);
//            sense.Gloss.MergeIn(CurrentWordAsMultiText);

//            var templateSense = CurrentTemplateLexicalEntry.Senses.FirstOrDefault();
//            if(templateSense !=null)
//            {
//                foreach (var optionProperty in templateSense.Properties.Where(p => p.Value.GetType() == typeof(OptionRefCollection)))
//                {
//                    var templateRefs = templateSense.GetProperty<OptionRefCollection>(optionProperty.Key);
//                    var destinationOptionRefs =
//                        sense.GetOrCreateProperty<OptionRefCollection>(optionProperty.Key);
//                    destinationOptionRefs.AddRange(templateRefs.Keys);
//                }
//
//                foreach (var optionProperty in templateSense.Properties.Where(p => p.Value.GetType() == typeof(OptionRef)))
//                {
//                    var optionRef = templateSense.GetProperty<OptionRef>(optionProperty.Key);
//                    sense.GetOrCreateProperty<OptionRef>(optionProperty.Key).Key = optionRef.Key;
//                }
//
//                foreach (var textProperty in templateSense.Properties.Where(p=>p.Value.GetType()==typeof(MultiText)))
//                {
//                    var target = sense.GetOrCreateProperty<MultiText>(textProperty.Key);
//                    foreach (var form in ((MultiText)textProperty.Value).Forms)
//                    {
//                        target.SetAlternative(form.WritingSystemId,form.Form);
//                    }
//                }
//            }
			//sens
			//we use this for matching up, and well, it probably is a good gloss

		  //  AddSenseToLexicon(newVernacularWord, sense);
			var sense = CurrentTemplateLexicalEntry.Senses.FirstOrDefault();
			AddSenseToLexicon(newVernacularWord,sense);
		}

		/// <summary>
		/// Try to add the sense to a matching entry. If none found, make a new entry with the sense
		/// </summary>
		private void AddSenseToLexicon(MultiTextBase lexemeForm, LexSense sense)
		{
			//remove from the gloss and def any forms we don't want in our project for those fields
			foreach (var form in sense.Gloss.Forms)
			{
				//why are we checking definition writing system here? Well, the whole gloss/def thing continues to be murky. When gathering, we're just
				//trying to populate both.  And if you have your 1st def be, say, french, and don't have that in your glosses, well
				// in the WordList task, you won't see the words you gathered, because that's based on glosses!
				if (!_glossWritingSystemIds.Contains(form.WritingSystemId) && !_definitionWritingSystemIds.Contains(form.WritingSystemId))
					sense.Gloss.SetAlternative(form.WritingSystemId, null);
			}
			foreach (var form in sense.Definition.Forms)
			{
				if (!_definitionWritingSystemIds.Contains(form.WritingSystemId) && !_glossWritingSystemIds.Contains(form.WritingSystemId))
					sense.Definition.SetAlternative(form.WritingSystemId, null);
			}

			//I don't recall why we did this, but what it is doing is populating def from gloss and vice-versa, where there are blanks

			var definition = sense.Definition;
			if(definition.Empty)
			{
				foreach (var form in sense.Gloss.Forms)
				{
					//this check makes sure we don't introduce a form form a lang we allow for gloss, but not def
					if (_definitionWritingSystemIds.Contains(form.WritingSystemId))
						definition.SetAlternative(form.WritingSystemId, form.Form);
				}
			}

			var gloss = sense.Gloss;
			if (gloss.Empty)
			{
				foreach (var form in sense.Definition.Forms)
				{
					//this check makes sure we don't introduce a form form a lang we allow for def, but not gloss
					if (_glossWritingSystemIds.Contains(form.WritingSystemId))
					gloss.SetAlternative(form.WritingSystemId, form.Form);
				}
			}

			//review: the desired semantics of this find are unclear, if we have more than one ws
			ResultSet<LexEntry> entriesWithSameForm =
					LexEntryRepository.GetEntriesWithMatchingLexicalForm(
							lexemeForm[_lexicalUnitWritingSystem.Id], _lexicalUnitWritingSystem);
			LanguageForm firstGloss = new LanguageForm("en", "-none-",null);
			if(sense.Gloss.Forms.Length>0)
				firstGloss = sense.Gloss.Forms[0];

			if (entriesWithSameForm.Count == 0)
			{
				LexEntry entry = LexEntryRepository.CreateItem();
				entry.LexicalForm.MergeIn(lexemeForm);
				entry.Senses.Add(sense.Clone());
				LexEntryRepository.SaveItem(entry);
				Logger.WriteEvent("WordList-Adding new word '{0}'and givin the sense '{1}'", entry.GetSimpleFormForLogging(), firstGloss );
			}
			else
			{
				LexEntry entry = entriesWithSameForm[0].RealObject;

				foreach (LexSense s in entry.Senses)
				{
					if (sense.Gloss.Forms.Length > 0)
					{
						LanguageForm glossWeAreAdding = firstGloss;
						string glossInThisWritingSystem =
								s.Gloss.GetExactAlternative(glossWeAreAdding.WritingSystemId);
						if (glossInThisWritingSystem == glossWeAreAdding.Form)
						{
							Logger.WriteEvent("WordList '{0}' already exists in '{1}'", firstGloss, entry.GetSimpleFormForLogging());
							return; //don't add it again
						}
					}
				}
				if(sense.Gloss.Forms.Length==0 && sense.Definition.Forms.Length ==0 && sense.ExampleSentences.Count==0)
					return;//nothing worth adding (may happen in unit test)

				entry.Senses.Add(sense);

				//REVIEW: June 2011, Hatton added this, because of WS-34024: if a new *meaning* was added to an existing entry,
				//and then the user quit, this change was unsaved.
				LexEntryRepository.SaveItem(entry);
				Logger.WriteEvent("WordList-Added '{0}' to preexisting '{1}'", firstGloss, entry.GetSimpleFormForLogging());

			}
		}

		public override void Deactivate()
		{
			base.Deactivate();
			if (_gatherControl != null)
			{
				_gatherControl.Cleanup();
				_gatherControl.Dispose();
			}
			_gatherControl = null;
		}

		public void NavigateToIndex(int i)
		{
			CurrentIndexIntoWordlist = i;
		}

		public void NavigatePrevious()
		{
			--CurrentIndexIntoWordlist;
		}

		public bool NavigateNext()
		{
			bool atLeatOneWordHadTheNeededWritingSystem = false;

			CurrentIndexIntoWordlist++;
			while (CanNavigateNext)
			{
				//skip words lacking all the needed alternatives
				if (!GetWordHasElligibleWritingSystem(CurrentTemplateLexicalEntry))
				{
					++CurrentIndexIntoWordlist;
					continue;
				}
				atLeatOneWordHadTheNeededWritingSystem = true;

				//skip words we already have elicited
				if (GetRecordsWithMatchingGloss().Count > 0)
				{
					++CurrentIndexIntoWordlist;
					continue;
				}
				break;
			}
			return atLeatOneWordHadTheNeededWritingSystem;
		}

		public void NavigateFirstToShow()
		{
			_currentWordIndex = -1;
			if(!NavigateNext())
			{
				if (_words == null)//WS-33662, which we could not reproduce
				{
					LoadFailureMessage = "Sorry, there was a problem loading this word pack.";
					Palaso.Reporting.ErrorReport.NotifyUserOfProblem("Sorry, there was a problem loading this word pack. (to WeSay developers: may be reproduction of WS-33662)");
				}
				else
				{
					var firstLexeme = _words.First().LexicalForm;
					var s = "";
					foreach(var form in firstLexeme.Forms)
					{
						var name = form.WritingSystemId;
						if (_viewTemplate.WritingSystems.Contains(form.WritingSystemId))
							name = _viewTemplate.WritingSystems.Get(form.WritingSystemId).LanguageName;
						else
						{	//if these langs aren't in the project, we might not be able to look them up, so do them by hand
							if (form.WritingSystemId == "en")///SIL CAWL comes with French and English
								name = "English";
							else if (form.WritingSystemId == "fr")
								name = "French";
						}
						s += " "+ name + ",";
					}
					s = s.Trim(new char[] {' ', ','});
					LoadFailureMessage = string.Format(
						"To use this word pack, set your first definition field to one of ({0}).", s);
					Palaso.Reporting.ErrorReport.NotifyUserOfProblem(LoadFailureMessage);
				}
			}
		}

		public void NavigateAbsoluteFirst()
		{
			CurrentIndexIntoWordlist = 0;
		}

//        public ResultSet<LexEntry> NotifyOfAddedWord()
//        {
//            return
//                    LexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
//                            CurrentWordAsMultiText.Find(_preferredPromptingWritingSystemId),
//                            _lexicalUnitWritingSystem);
//        }

		public ResultSet<LexEntry> GetRecordsWithMatchingGloss()
		{
			//var form = CurrentWordAsMultiText.GetBestAlternative(new string[]{_preferredPromptingWritingSystemId});
			var form = CurrentPromptingLanguageForm;
			return
						LexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
								form,
								_lexicalUnitWritingSystem);
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return CountNotRelevant;
		}

		protected override int ComputeReferenceCount()
		{
			return CountNotRelevant; //Todo
		}

		public override void FocusDesiredControl()
		{
			// This is the place to implement how the task selects it's desired child control
			return;
		}

		/// <summary>
		/// Removes the sense (if otherwise empty) and deletes the entry if it has no reason left to live
		/// </summary>
		public void TryToRemoveAssociationWithListWordFromEntry(RecordToken<LexEntry> recordToken)
		{
			// have to iterate through these in reverse order
			// since they might get modified
			LexEntry entry = recordToken.RealObject;
			for (int i = entry.Senses.Count - 1;i >= 0;i--)
			{
				if(entry.Senses[i].Equals(CurrentTemplateSense))
				{
					entry.Senses.RemoveAt(i);
				}
			}
			entry.CleanUpAfterEditting();
			if (entry.IsEmptyExceptForLexemeFormForPurposesOfDeletion)
			{
				LexEntryRepository.DeleteItem(entry);
			}
			else
			{
				LexEntryRepository.SaveItem(entry);
			}
		}


	}
}
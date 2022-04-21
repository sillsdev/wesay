using Chorus.UI.Notes;
using Exortech.NetReflector;
using SIL.DictionaryServices.Model;
using SIL.i18n;
using SIL.Lift;
using SIL.Reporting;
using SIL.WritingSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project
{
	[ReflectorType("viewTemplate")]
	public class ViewTemplate : List<Field>
	{
		private string _id = "Default View Template";
		private bool _doWantGhosts = true;

		/// <summary>
		/// For serialization only
		/// </summary>
		[ReflectorCollection("fields", Required = true)]
		public List<Field> Fields
		{
			get { return this; }
			set
			{
				Clear();
				foreach (Field f in value)
				{
					if (f == null)
					{
						throw new ArgumentNullException("value", "one of the fields is null");
					}
					Add(f);
				}
			}
		}

		[ReflectorProperty("id", Required = false)]
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IList<WritingSystemDefinition> HeadwordWritingSystems
		{
			get
			{
				IList<string> ids =
						GetField(LexEntry.WellKnownProperties.LexicalUnit).WritingSystemIds;
				return BasilProject.Project.WritingSystemsFromIds(ids);
			}
		}

		///<summary>
		///Gets the field with the specified name.
		///</summary>
		///
		///<returns>
		///The field with the given field name.
		///</returns>
		///
		///<param name="fieldName">The field name of the field to get.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		public Field this[string fieldName]
		{
			get
			{
				Field field;
				if (!TryGetField(fieldName, out field))
				{
					throw new ArgumentOutOfRangeException("fieldName",
														  fieldName,
														  "View template does not contain a defintion for the given fieldname");
				}
				return field;
			}
		}

		public bool TryGetField(string fieldName, out Field field)
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException();
			}
			field = Find(delegate (Field f) { return f.FieldName == fieldName; });

			if (field == default(Field))
			{
				return false;
			}
			return true;
		}

		//todo: either switch to using the key (class.name) or ?
		public Field GetField(string fieldName)
		{
			Field field;
			if (TryGetField(fieldName, out field))
			{
				return field;
			}
			return null;
		}

		public Field GetMeaningField()
		{
			Field gloss = GetField(LexSense.WellKnownProperties.Gloss);
			return gloss.IsMeaningField ? gloss : GetField(LexSense.WellKnownProperties.Definition);
		}

		public List<Field> GetCustomFields(string className)
		{
			List<Field> customFields = new List<Field>();
			foreach (Field field in this)
			{
				if (field.ClassName == className && !field.IsBuiltInViaCode)
				{
					customFields.Add(field);
				}
			}
			return customFields;
		}

		public bool Contains(string fieldName)
		{
			Field field;
			if (!TryGetField(fieldName, out field))
			{
				return false;
			}
			return field.Enabled;
			// field.Visibility == CommonEnumerations.VisibilitySetting.Visible;
		}

		/// <summary>
		/// Update the users' field inventory with new stuff.
		/// </summary>
		/// <remarks>
		/// used in the admin to update the users template with new fields.
		///
		/// This was written before, and should not to be confused with,
		///  MigrateConfigurationXmlIfNeeded(), which could
		/// do most of this work. It's not obvious how that would get the guess
		/// of which writing systems to assign in (but that's not a big deal).
		///
		/// The algorithm here is to fill the list with all of the fields from the master inventory.
		/// If a field is also found in the users existing inventory, turn on the checkbox,
		/// and change any settings (e.g. the writing systems) to match the users' inventory spec.
		/// Then add any custom fields from the existing inventory.
		/// </remarks>
		/// <param name="factoryTemplate"></param>
		/// <param name="usersTemplate"></param>
		public static void UpdateUserViewTemplate(ViewTemplate factoryTemplate,
												  ViewTemplate usersTemplate)
		{
			if (factoryTemplate == null)
			{
				throw new ArgumentNullException();
			}
			if (usersTemplate == null)
			{
				throw new ArgumentNullException();
			}

			// handled by xslt           //in jan 2008 we changed this field name, moving it in line with the lift spec
			//            const string oldGlossFieldName = "SenseGloss";
			//            Field userGlossField = usersTemplate.GetField(oldGlossFieldName);
			//            if (userGlossField != null)
			//            {
			//                userGlossField.FieldName = LexSense.WellKnownProperties.Gloss;
			//            }

			foreach (Field masterField in factoryTemplate)
			{
				Field userField = usersTemplate.GetField(masterField.FieldName);
				if (userField != null)
				{
					//allow us to improve the descriptions
					userField.Description = masterField.Description;
				}
				else
				{
					masterField.Enabled = false; //let them turn it on if they want
					usersTemplate.Fields.Add(masterField);
				}
			}

			if (factoryTemplate.GetField(LexSense.WellKnownProperties.Definition) == null)
			{
				return; // this is some  test situation with a abnormal factory template
			}

			usersTemplate.MoreMigrations();
		}

		internal void MoreMigrations()
		{
			//pre-nov2007 (png alpha release) projects had Note.
			//then, when we fixed it to "note", they had two note fields!
			//this is to clean up the mess.
			RemoveByFieldName(this, "Note");
			RemoveDuplicateGloss();

			//In Jan 2008 (still in version 1 Preview 3, just a hand-full of users)
			//we switch from "meaning" being the gloss, to the definition, making Definition
			//a non-optional field, and gloss a normally hidden field

			Field def = GetField(LexSense.WellKnownProperties.Definition);
			Field gloss = GetField(LexSense.WellKnownProperties.Gloss);

			// In June 2017 the option was put in to have the meaning field be the gloss
			// so making sure gloss isn't the meaning field before doing it

			//this is an upgrade situation
			if (!gloss.IsMeaningField && (!def.Enabled || def.Visibility != CommonEnumerations.VisibilitySetting.Visible))
			{
				//copy writing systems from glosses
				foreach (string writingSystemId in gloss.WritingSystemIds)
				{
					if (!def.WritingSystemIds.Contains(writingSystemId))
					{
						def.WritingSystemIds.Add(writingSystemId);
					}
				}
			}

			//detect pre-gloss-to-definition switch
			if (!gloss.IsMeaningField && (!def.Enabled || def.Visibility != CommonEnumerations.VisibilitySetting.Visible))
			{
				gloss.Visibility = CommonEnumerations.VisibilitySetting.Invisible;
			}


			// In Feb 2008 we started giving user control over field order, but
			// certain key fields must be first.
			if (gloss.IsMeaningField)
			{
				gloss.Enabled = true;
				gloss.Visibility = CommonEnumerations.VisibilitySetting.Visible;
				MoveToFirstInClass(gloss);
			}
			else
			{
				def.Enabled = true;
				def.Visibility = CommonEnumerations.VisibilitySetting.Visible;
				MoveToFirstInClass(def);
			}

			MoveToFirstInClass(GetField(Field.FieldNames.EntryLexicalForm.ToString()));
			MoveToFirstInClass(GetField(Field.FieldNames.ExampleSentence.ToString()));

			//In Nov 2008 (v 0.5) we made the note field multi-paragraph
			Field note = GetField(PalasoDataObject.WellKnownProperties.Note);
			if (!note.IsMultiParagraph)
			{
				note.IsMultiParagraph = true;
			}

			//In March 2009 we moved Sense.LiteralMeaning --> Entry.literal-meaning
			//the default template has the new one, so we just have to remove the old
			//the parser (builder) does the actual data moving/renaming for existing data
			Field oldLitMeaning = GetField("LiteralMeaning");
			if (oldLitMeaning != null)
			{
				Field newLitMeaning = GetField(LexEntry.WellKnownProperties.LiteralMeaning);
				newLitMeaning.Enabled = oldLitMeaning.Enabled;
				RemoveByFieldName(this, "LiteralMeaning");
			}

		}

		/// <summary>
		/// there was a migration bug in Jan 2008 that gave Kim Blewett an extra gloss. Others may get that too, and the
		/// ui won't let them delete this well-known field
		/// </summary>
		public void RemoveDuplicateGloss()
		{
			bool foundAlready = false;
			Field doomed = null;
			foreach (Field field in Fields)
			{
				if (field.FieldName == LexSense.WellKnownProperties.Gloss)
				{
					if (foundAlready)
					{
						doomed = field;
					}

					foundAlready = true;
				}
			}
			if (doomed != null)
			{
				Fields.Remove(doomed);
			}
		}

		private void MoveToFirstInClass(Field field)
		{
			while (!IsFieldFirstInClass(field))
			{
				MoveUpInClass(field);
			}
		}

		public void MoveToLastInClass(Field field)
		{
			while (!IsFieldLastInClass(field))
			{
				MoveDownInClass(field);
			}
		}

		private static void RemoveByFieldName(ICollection<Field> usersTemplate, string fielName)
		{
			List<Field> condemned = new List<Field>();
			foreach (Field field in usersTemplate)
			{
				if (field.FieldName == fielName) //keep the case!
				{
					condemned.Add(field);
				}
			}
			foreach (Field field in condemned)
			{
				usersTemplate.Remove(field);
			}
		}

		public static ViewTemplate MakeMasterTemplate(IWritingSystemRepository writingSystems, string languageTag = WeSayWordsProject.VernacularWritingSystemIdForProjectCreation)
		{
			var defaultVernacularSet = new List<string> { languageTag };

			var defaultAnalysisSet = new List<string> { WeSayWordsProject.AnalysisWritingSystemIdForProjectCreation };

			ViewTemplate masterTemplate = new ViewTemplate();

			Field lexicalFormField = new Field(Field.FieldNames.EntryLexicalForm.ToString(),
											   "LexEntry",
											   defaultVernacularSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Word",
							  "The label for the field showing the Lexeme Form of the entry.");
			lexicalFormField.DisplayName = "Word";
			lexicalFormField.Description = "The Lexeme Form of the entry.";
			lexicalFormField.Enabled = true;
			lexicalFormField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			masterTemplate.Add(lexicalFormField);

			Field citationFormField = new Field(LexEntry.WellKnownProperties.Citation,
												"LexEntry",
												defaultVernacularSet);
			StringCatalog.Get("~Citation Form",
							  "The label for the field holding the citation form, which is how the word will be displayed in the dictionary.  This is used in languages where the lexeme form may be different from what the Headword should be.");
			citationFormField.DisplayName = "Citation Form";
			citationFormField.Description =
					"A form which overrides the Lexeme Form to be the Headword in the printed dictionary";
			citationFormField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			citationFormField.Enabled = false;
			masterTemplate.Add(citationFormField);

			Field definitionField = new Field(LexSense.WellKnownProperties.Definition,
											  "LexSense",
											  defaultAnalysisSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Definition",
							  "The label for the field showing the definition of the word.");
			definitionField.DisplayName = "Definition (Meaning)";
			definitionField.Description =
					"The definition of this sense of the word, in one or more languages.";
			definitionField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			definitionField.Enabled = true;
			definitionField.IsMeaningField = true;
			definitionField.IsSpellCheckingEnabled = true;
			masterTemplate.Add(definitionField);

			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Gloss",
							  "The label for the field showing a single word translation, as used in interlinear text glossing.");
			Field glossField = new Field(LexSense.WellKnownProperties.Gloss,
										 "LexSense",
										 defaultAnalysisSet);
			glossField.DisplayName = "Gloss";
			glossField.Description = "Normally a single word, used when interlinearizing texts.";
			glossField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			glossField.Enabled = false;
			glossField.IsMeaningField = false;
			glossField.IsSpellCheckingEnabled = true;
			masterTemplate.Add(glossField);

			Field silCawlField = new Field("SILCAWL",
										 "LexSense", new string[] { "en" });
			silCawlField.DisplayName = "SIL CAWL #";
			silCawlField.Description = "The SIL CAWL wordlist # for this entry, (see the SIL CAWL wordlist task).";
			silCawlField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			silCawlField.Enabled = false;
			silCawlField.IsSpellCheckingEnabled = false;
			masterTemplate.Add(silCawlField);

			Field literalMeaningField = new Field("literal-meaning", "LexEntry", defaultAnalysisSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Literal Meaning",
							  "The label for the field showing the literal meaning of idiom or proverb.");
			literalMeaningField.DisplayName = "Literal Meaning";
			literalMeaningField.Description = "Literal meaning of an idiom.";
			literalMeaningField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			literalMeaningField.Enabled = false;
			literalMeaningField.IsSpellCheckingEnabled = true;
			masterTemplate.Add(literalMeaningField);

			Field noteField = new Field(PalasoDataObject.WellKnownProperties.Note,
										"PalasoDataObject",
										defaultAnalysisSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Note", "The label for the field showing a note.");
			noteField.DisplayName = "Note";
			// noteField.ConfigurationName = "Note";
			noteField.Description = "A note.";
			noteField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			noteField.Enabled = true;
			noteField.IsSpellCheckingEnabled = true;
			noteField.IsMultiParagraph = true;

			masterTemplate.Add(noteField);

			//            Field entryNoteField = new Field(LexEntry.WellKnownProperties.Note, "LexEntry", defaultAnalysisSet);
			//            //this is here so the PoMaker scanner can pick up a comment about this label
			//            StringCatalog.Get("~Note", "The label for the field showing a note.");
			//            entryNoteField.DisplayName = "Note";
			//            entryNoteField.ConfigurationName = "Note on Entry";
			//            entryNoteField.Description = "A note on the entry.";
			//            entryNoteField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			//            entryNoteField.Enabled = false;
			//            masterTemplate.Add(entryNoteField);

			//            Field senseNoteField = new Field(LexSense.WellKnownProperties.Note, "LexSense", defaultAnalysisSet);
			//            //this is here so the PoMaker scanner can pick up a comment about this label
			//            senseNoteField.DisplayName = "Note";
			//            senseNoteField.ConfigurationName = "Note on Sense";
			//            senseNoteField.Description = "A note on the sense.";
			//            senseNoteField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			//            senseNoteField.Enabled = false;
			//            masterTemplate.Add(senseNoteField);

			Field pictureField = new Field("Picture", "LexSense", defaultAnalysisSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Picture", "The label for the field showing a picture.");
			pictureField.DisplayName = "Picture";
			pictureField.Description = "An image corresponding to the sense.  This field will automatically search SIL's image library, 'The Art of Reading', if it has been installed on your computer.";
			pictureField.DataTypeName = "Picture";
			pictureField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			pictureField.Enabled = true;
			masterTemplate.Add(pictureField);

			Field posField = new Field(LexSense.WellKnownProperties.PartOfSpeech,
									   "LexSense",
									   defaultAnalysisSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			//at some point, POS was renamed to PartOfSpeech
			StringCatalog.Get("~Parts of Speech", "The label for the field showing Part Of Speech");
			StringCatalog.Get("~PartOfSpeech", "The label for the field showing Part Of Speech");
			posField.DisplayName = "PartOfSpeech";
			posField.Description = "The grammatical category of the entry (Noun, Verb, etc.).";
			posField.DataTypeName = "Option";
			posField.OptionsListFile = "PartsOfSpeech.xml";
			posField.Enabled = true;
			masterTemplate.Add(posField);

			Field exampleField = new Field(LexExampleSentence.WellKnownProperties.ExampleSentence,
										   "LexExampleSentence",
										   defaultVernacularSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Example Sentence",
							  "The label for the field showing an example use of the word.");
			exampleField.DisplayName = "Example Sentence";
			exampleField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			exampleField.Enabled = true;
			exampleField.IsSpellCheckingEnabled = true;
			masterTemplate.Add(exampleField);

			Field translationField = new Field(LexExampleSentence.WellKnownProperties.Translation,
											   "LexExampleSentence",
											   defaultAnalysisSet);
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Example Translation",
							  "The label for the field showing the example sentence translated into other languages.");
			translationField.DisplayName = "Example Translation";
			translationField.Description =
					"The translation of the example sentence into another language.";
			translationField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			translationField.Enabled = false;
			translationField.IsSpellCheckingEnabled = true;
			masterTemplate.Add(translationField);

			Field ddp4Field = new Field(LexSense.WellKnownProperties.SemanticDomainDdp4, "LexSense", defaultAnalysisSet);

			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Sem Dom", "The label for the field showing Semantic Domains");

			ddp4Field.DisplayName = "Sem Dom";
			ddp4Field.Description =
					"The semantic domains of the sense, using Ron Moe's Dictionary Development Process version 4.\r\n. You can enter these directly by typing the number of the domain, its name, or a word used in the description. You can also use the Gather By Semantic Domains Task, which will try to use the input system chosen by this field.";
			ddp4Field.DataTypeName = "OptionCollection";
			ddp4Field.OptionsListFile = "SemDom.xml";
			ddp4Field.Enabled = true;
			ddp4Field.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			masterTemplate.Add(ddp4Field);

			Field baseFormField = new Field(LexEntry.WellKnownProperties.BaseForm,
											"LexEntry",
											defaultVernacularSet,
											Field.MultiplicityType.ZeroOr1,
											"RelationToOneEntry");
			//this is here so the PoMaker scanner can pick up a comment about this label
			StringCatalog.Get("~Base Form",
							  "The label for the field showing the entry which this entry is derived from, is a subentry of, etc.");
			baseFormField.DisplayName = "Base Form";
			baseFormField.Description =
					"Provides a field for identifying the form from which an entry is derived.  You may use this in place of the MDF subentry.  In a future version WeSay may support directly listing derived forms from the base form.";
			baseFormField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			baseFormField.Enabled = false;
			masterTemplate.Add(baseFormField);

			Field crossRefField = new Field(LexEntry.WellKnownProperties.CrossReference,
											"LexEntry",
											defaultVernacularSet,
											Field.MultiplicityType.ZeroOrMore,
											"RelationToOneEntry");
			StringCatalog.Get("~Cross Reference",
							  @"The label for the field showing a 'confer' relation (\cf in MDF).");
			crossRefField.DisplayName = "Cross Reference";
			crossRefField.Description =
					"Provides a single cross reference at this time (we need to do more work to allow it to hold multiple references... let us know if this is a priority for you.)";
			crossRefField.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			crossRefField.Enabled = false;
			masterTemplate.Add(crossRefField);

			return masterTemplate;
		}

		#region persistence

		public void Load(string path)
		{
			NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
			XmlReader reader = XmlReader.Create(path);
			try
			{
				r.Read(reader, this);
			}
			finally
			{
				reader.Close();
			}
		}

		public void LoadFromString(string xml)
		{
			NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
			XmlReader reader = XmlReader.Create(new StringReader(xml));
			try
			{
				r.Read(reader, this);
			}
			finally
			{
				reader.Close();
			}
		}

		/// <summary>
		/// Does not "start" the xml doc, no close the writer
		/// </summary>
		/// <param name="writer"></param>
		public void Write(XmlWriter writer)
		{
			NetReflector.Write(writer, this);
		}

		private static NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(ViewTemplate));
			t.Add(typeof(Field));
			//   t.Add(typeof(Field.WritingSystemId));
			return t;
		}

		#endregion

		public void OnWritingSystemIDChange(string from, string to)
		{
			foreach (Field field in Fields)
			{
				if (field.WritingSystemIds.Contains(to))
				{
					field.WritingSystemIds.Remove(from);
				}
				else
				{
					field.ChangeWritingSystemId(from, to);
				}
			}
		}

		internal string RemoveMeaning(string displayname)
		{
			if (displayname.EndsWith(" (Meaning)"))
			{
				int index = displayname.IndexOf(" (Meaning)");
				return displayname.Remove(index);
			}
			else return displayname;
		}

		internal string AddMeaning(string displayname)
		{
			if (!displayname.EndsWith(" (Meaning)"))
			{
				return displayname + " (Meaning)";
			}
			else return displayname;
		}

		public void OnMeaningFieldChange(string meaningField)
		{
			Field def = GetField(LexSense.WellKnownProperties.Definition);
			Field gloss = GetField(LexSense.WellKnownProperties.Gloss);

			// reset description to remove text about it being the meaning field
			def.Description =
		"The definition of this sense of the word, in one or more languages.";

			switch (meaningField)
			{
				case "definition":
					def.DisplayName = AddMeaning(def.DisplayName);
					def.Visibility = CommonEnumerations.VisibilitySetting.Visible;
					def.IsMeaningField = true;
					def.Enabled = true;
					gloss.DisplayName = RemoveMeaning(gloss.DisplayName);
					gloss.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
					gloss.IsMeaningField = false;
					gloss.Enabled = false;
					MoveToFirstInClass(def);
					break;
				case "gloss":
					def.DisplayName = RemoveMeaning(def.DisplayName);
					def.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
					def.IsMeaningField = false;
					def.Enabled = false;
					gloss.DisplayName = AddMeaning(gloss.DisplayName);
					gloss.Visibility = CommonEnumerations.VisibilitySetting.Visible;
					gloss.IsMeaningField = true;
					gloss.Enabled = true;
					MoveToFirstInClass(gloss);
					break;
			}

		}

		public bool IsWritingSystemUsedInField(string writingSystemId, string fieldName)
		{
			Field field = GetField(fieldName);
			if (field != null)
			{
				return field.WritingSystemIds.Contains(writingSystemId);
			}
			return false;
		}

		public bool IsWritingSystemInUse(string writingSystemId)
		{
			foreach (var field in Fields)
			{
				if (field.WritingSystemIds.Contains(writingSystemId))
				{
					return true;
				}
			}
			return false;
		}

		public WritingSystemDefinition GetDefaultWritingSystemForField(string fieldName)
		{
			Field field = GetField(fieldName);
			if (field == null)
			{
				throw new ConfigurationException(String.Format("The field {0} has not been enabled for your project. Please enable it in the WeSay config tool.", fieldName));
			}
			if (field.WritingSystemIds.Count == 0)
			{
				throw new ConfigurationException(String.Format("The field {0} has no input system associated with it. Please assign an input system to it in the WeSay config tool.", fieldName));
			}
			return BasilProject.Project.WritingSystems.Get(field.WritingSystemIds[0]);
		}

		public WritingSystemDefinition GetFirstNonVoiceWritingSystemForFieldOrThrow(string fieldName)
		{
			Field field = GetField(fieldName);
			if (field == null)
			{
				throw new ConfigurationException(String.Format("The field {0} has not been enabled for your project. Please enable it in the WeSay config tool.", fieldName));
			}
			if (field.WritingSystemIds.Count == 0)
			{
				throw new ConfigurationException(
					String.Format(
						"The field {0} has no input system associated with it. Please assign an input system to it in the WeSay config tool.",
						fieldName));
			}
			foreach (var writingSystemId in field.WritingSystemIds)
			{
				var writingSystem = BasilProject.Project.WritingSystems.Get(writingSystemId);
				if (!writingSystem.IsVoice)
					return writingSystem;
			}
			throw new ConfigurationException("A non-voice writing system is required for the field {0}", fieldName);
		}

		public bool IsFieldFirstInClass(Field field)
		{
			return GetFieldsOfClass(field.ClassName).IndexOf(field) == 0;
		}

		public bool IsFieldLastInClass(Field field)
		{
			List<Field> fields = GetFieldsOfClass(field.ClassName);
			return fields.IndexOf(field) == fields.Count - 1;
		}

		public void MoveUpInClass(Field field)
		{
			if (!IsFieldFirstInClass(field))
			{
				List<Field> classMates = GetFieldsOfClass(field.ClassName);
				int indexAmongClassmates = classMates.IndexOf(field);

				Field previousClassmate = classMates[indexAmongClassmates - 1];
				if (!previousClassmate.UserCanRelocate)
				{
					return;
				}
				int newIndexAmongAllFields = Fields.IndexOf(previousClassmate);

				Fields.Remove(field);
				Fields.Insert(newIndexAmongAllFields, field);
			}
		}

		private List<Field> GetFieldsOfClass(string className)
		{
			List<Field> fields = new List<Field>();
			foreach (Field field in Fields)
			{
				if (field.ClassName == className)
				{
					fields.Add(field);
				}
			}
			return fields;
		}

		public void MoveDownInClass(Field field)
		{
			if (!IsFieldLastInClass(field))
			{
				List<Field> classMates = GetFieldsOfClass(field.ClassName);
				int indexAmongClassmates = classMates.IndexOf(field);

				Field nextClassmate = classMates[indexAmongClassmates + 1];
				int newIndexAmongAllFields = Fields.IndexOf(nextClassmate);

				Fields.Remove(field);
				Fields.Insert(newIndexAmongAllFields, field);
			}
		}

		public GhostingRule GetGhostingRuleForField(string fieldName)
		{
			return new GhostingRule(DoWantGhosts);
		}

		public bool DoWantGhosts
		{
			get { return _doWantGhosts; }

			set { _doWantGhosts = value; }
		}

		public IEnumerable<string> GetHeadwordWritingSystemIds()
		{
			Field fieldControllingHeadwordOutput =
				GetField(LexEntry.WellKnownProperties.Citation);
			if (fieldControllingHeadwordOutput == null || !fieldControllingHeadwordOutput.Enabled)
			{
				fieldControllingHeadwordOutput =
					GetField(LexEntry.WellKnownProperties.LexicalUnit);
				if (fieldControllingHeadwordOutput == null)
				{
					throw new ArgumentException("Expected to find LexicalUnit in the view Template");
				}
			}
			return WritingSystems.FilterForTextLanguageTags(fieldControllingHeadwordOutput.WritingSystemIds);
		}

		public IWritingSystemRepository WritingSystems
		{
			get { return BasilProject.Project.WritingSystems; }
		}

		public ChorusNotesDisplaySettings CreateChorusDisplaySettings()
		{
			var list = new List<Chorus.IWritingSystem>();

			WritingSystemDefinition noteWritingSystem;
			try
			{
				noteWritingSystem = GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Note); ;
			}
			catch (ConfigurationException)
			{
				// if no writing system is defined for the Note field, so just use the default AnalysisWritingSystem
				noteWritingSystem = new WritingSystemDefinition(WeSayWordsProject.AnalysisWritingSystemIdForProjectCreation);
			}

			list.Insert(0, new ChorusWritingSystemAdaptor(noteWritingSystem));
			foreach (var system in WritingSystems.TextWritingSystems())
			{
				if (system != noteWritingSystem)
				{
					list.Add(new ChorusWritingSystemAdaptor(system));
				}
			}

			return new Chorus.UI.Notes.ChorusNotesDisplaySettings()
			{
				WritingSystems = list,
				WritingSystemForNoteContent = new ChorusWritingSystemAdaptor(noteWritingSystem),
				WritingSystemForNoteLabel = new ChorusWritingSystemAdaptor(GetDefaultWritingSystemForField(LexEntry.WellKnownProperties.LexicalUnit))
			};
		}

		public void DeleteWritingSystem(string id)
		{
			foreach (Field field in Fields)
			{
				field.WritingSystemIds.Remove(id);
			}
		}

	}

	/// <summary>
	/// this may get more complicated someday
	/// </summary>
	public class GhostingRule
	{
		public GhostingRule(bool show)
		{
			ShowGhost = show;
		}
		public bool ShowGhost { get; set; }
	}
}
<ClassProject>
  <Language>CSharp</Language>
  <Entities>
	<Entity type="Class">
	  <Name>LexSense</Name>
	  <Access>Public</Access>
	  <Member type="Property">public MultiText Gloss { get; }</Member>
	  <Member type="Property">public MultiText Definition { get; }</Member>
	  <Member type="Property">public IList&lt;LexExampleSentence&gt; ExampleSentences { get; }</Member>
	  <Member type="Property">public virtual Boolean IsEmpty { get; }</Member>
	  <Member type="Property">public Boolean IsEmptyForPurposesOfDeletion { get; }</Member>
	  <Member type="Property">public String Id { get; set; }</Member>
	  <Member type="Method">public String GetOrCreateId()</Member>
	  <Member type="Method">public override Void CleanUpAfterEditting()</Member>
	  <Member type="Method">public override Void CleanUpEmptyObjects()</Member>
	  <Member type="Constructor">public LexSense(WeSayDataObject parent)</Member>
	  <Member type="Constructor">public LexSense()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>WellKnownProperties</Name>
	  <Access>Public</Access>
	  <Member type="Method">public Boolean ContainsAnyCaseVersionOf(String fieldName)</Member>
	  <Member type="Constructor">public WellKnownProperties()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>SenseGlossMultiText</Name>
	  <Access>Public</Access>
	  <Member type="Property">public LexSense Parent { get; }</Member>
	  <Member type="Constructor">public SenseGlossMultiText(WeSayDataObject parent)</Member>
	  <Member type="Constructor">public SenseGlossMultiText()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Interface">
	  <Name>IFieldQuery&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">Predicate&lt;T&gt; FilteringPredicate { get; }</Member>
	  <Member type="Property">String Key { get; }</Member>
	  <Member type="Property">Field Field { get; }</Member>
	</Entity>
	<Entity type="Class">
	  <Name>MissingFieldQuery</Name>
	  <Access>Public</Access>
	  <Member type="Property">public sealed override String Key { get; }</Member>
	  <Member type="Property">public sealed override Predicate&lt;LexEntry&gt; FilteringPredicate { get; }</Member>
	  <Member type="Property">public String FieldName { get; }</Member>
	  <Member type="Property">public sealed override Field Field { get; }</Member>
	  <Member type="Constructor">public MissingFieldQuery(Field field)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LexRelationType</Name>
	  <Access>Public</Access>
	  <Member type="Property">public String ID { get; }</Member>
	  <Member type="Property">public Multiplicities Multiplicity { get; }</Member>
	  <Member type="Property">public TargetTypes TargetType { get; }</Member>
	  <Member type="Constructor">public LexRelationType(String id, Multiplicities multiplicity, TargetTypes targetType)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Enum">
	  <Name>Multiplicities</Name>
	  <Access>Public</Access>
	  <Value>One</Value>
	  <Value>Many</Value>
	</Entity>
	<Entity type="Enum">
	  <Name>TargetTypes</Name>
	  <Access>Public</Access>
	  <Value>Entry</Value>
	  <Value>Sense</Value>
	</Entity>
	<Entity type="Class">
	  <Name>LexRelation</Name>
	  <Access>Public</Access>
	  <Member type="Event">public event PropertyChangedEventHandler PropertyChanged</Member>
	  <Member type="Property">public sealed override String Key { get; set; }</Member>
	  <Member type="Property">public sealed override WeSayDataObject Parent { set; }</Member>
	  <Member type="Property">public String FieldId { get; set; }</Member>
	  <Member type="Property">public sealed override String TargetId { get; set; }</Member>
	  <Member type="Property">public sealed override Boolean ShouldHoldUpDeletionOfParentObject { get; }</Member>
	  <Member type="Property">public sealed override Boolean ShouldCountAsFilledForPurposesOfConditionalDisplay { get; }</Member>
	  <Member type="Property">public sealed override Boolean ShouldBeRemovedFromParentDueToEmptiness { get; }</Member>
	  <Member type="Property">public sealed override String Value { get; set; }</Member>
	  <Member type="Method">public LexEntry GetTarget(LexEntryRepository repository)</Member>
	  <Member type="Method">public Void SetTarget(LexEntry entry)</Member>
	  <Member type="Method">public sealed override Void RemoveEmptyStuff()</Member>
	  <Member type="Constructor">public LexRelation(String fieldId, String targetId, WeSayDataObject parent)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LexRelationCollection</Name>
	  <Access>Public</Access>
	  <Member type="Property">public WeSayDataObject Parent { get; set; }</Member>
	  <Member type="Property">public List&lt;LexRelation&gt; Relations { get; set; }</Member>
	  <Member type="Property">public sealed override Boolean ShouldHoldUpDeletionOfParentObject { get; }</Member>
	  <Member type="Property">public sealed override Boolean ShouldCountAsFilledForPurposesOfConditionalDisplay { get; }</Member>
	  <Member type="Property">public sealed override Boolean ShouldBeRemovedFromParentDueToEmptiness { get; }</Member>
	  <Member type="Method">public sealed override Void RemoveEmptyStuff()</Member>
	  <Member type="Constructor">public LexRelationCollection()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftPreparer</Name>
	  <Access>Internal</Access>
	  <Member type="Method">internal Void PopulateDefinitions(ProgressState state)</Member>
	  <Member type="Method">internal String PopulateDefinitions(String pathToLift)</Member>
	  <Member type="Method">public Boolean IsMigrationNeeded()</Member>
	  <Member type="Method">public Boolean MigrateIfNeeded()</Member>
	  <Member type="Method">public Void MigrateLiftFile(ProgressState state)</Member>
	  <Member type="Constructor">public LiftPreparer(String liftFilePath)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>WeSayDataObjectLabelAdaptor</Name>
	  <Access>Public</Access>
	  <Member type="Method">public sealed override String GetDisplayLabel(Object item)</Member>
	  <Member type="Method">public sealed override String GetToolTip(Object item)</Member>
	  <Member type="Method">public sealed override String GetToolTipTitle(Object item)</Member>
	  <Member type="Constructor">public WeSayDataObjectLabelAdaptor(IList&lt;String&gt; writingSystemIds)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftRepository</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public virtual Boolean CanPersist { get; }</Member>
	  <Member type="Property">private String LiftDirectory { get; }</Member>
	  <Member type="Property">public Boolean IsLiftFileLocked { get; }</Member>
	  <Member type="Method">public override Void Startup(ProgressState state)</Member>
	  <Member type="Method">public override Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public override Void DeleteItem(LexEntry item)</Member>
	  <Member type="Method">public override Void SaveItem(LexEntry item)</Member>
	  <Member type="Method">public override Void SaveItems(IEnumerable&lt;LexEntry&gt; items)</Member>
	  <Member type="Method">public override Void Dispose()</Member>
	  <Member type="Method">public Boolean MergeIncrementFiles()</Member>
	  <Member type="Constructor">public LiftRepository(String filePath)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Field</Name>
	  <Access>Public</Access>
	  <Member type="Property">public String FieldName { get; set; }</Member>
	  <Member type="Property">public String Key { get; }</Member>
	  <Member type="Property">public String DisplayName { get; set; }</Member>
	  <Member type="Property">public String ClassName { get; set; }</Member>
	  <Member type="Property">public Boolean UserCanDeleteOrModify { get; }</Member>
	  <Member type="Property">public Boolean UserCanRelocate { get; }</Member>
	  <Member type="Property">public String DataTypeName { get; set; }</Member>
	  <Member type="Property">public String OptionsListFile { get; set; }</Member>
	  <Member type="Property">public Boolean IsBuiltInViaCode { get; }</Member>
	  <Member type="Property">public IList&lt;String&gt; WritingSystemIds { get; set; }</Member>
	  <Member type="Property">public String Description { get; set; }</Member>
	  <Member type="Property">public VisibilitySetting Visibility { get; set; }</Member>
	  <Member type="Property">public Boolean Enabled { get; set; }</Member>
	  <Member type="Property">public Boolean ShowOptionListStuff { get; }</Member>
	  <Member type="Property">public MultiplicityType Multiplicity { get; set; }</Member>
	  <Member type="Property">public String NewFieldNamePrefix { get; }</Member>
	  <Member type="Property">public Boolean CanOmitFromMainViewTemplate { get; }</Member>
	  <Member type="Property">public Boolean IsSpellCheckingEnabled { get; set; }</Member>
	  <Member type="Method">public String MakeFieldNameSafe(String text)</Member>
	  <Member type="Method">public override String ToString()</Member>
	  <Member type="Method">public Void ChangeWritingSystemId(String oldId, String newId)</Member>
	  <Member type="Method">public Boolean GetDoShow(IReportEmptiness data, Boolean showNormallyHiddenFields)</Member>
	  <Member type="Method">public Boolean HasWritingSystem(String writingSystemId)</Member>
	  <Member type="Method">public Void ModifyMasterFromUser(Field master, Field user)</Member>
	  <Member type="Constructor">public Field()</Member>
	  <Member type="Constructor">public Field(String fieldName, String className, IEnumerable&lt;String&gt; writingSystemIds)</Member>
	  <Member type="Constructor">public Field(String fieldName, String className, IEnumerable&lt;String&gt; writingSystemIds, MultiplicityType multiplicity, String dataTypeName)</Member>
	  <Member type="Constructor">public Field(Field field)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Enum">
	  <Name>MultiplicityType</Name>
	  <Access>Public</Access>
	  <Value>ZeroOr1</Value>
	  <Value>ZeroOrMore</Value>
	</Entity>
	<Entity type="Enum">
	  <Name>BuiltInDataType</Name>
	  <Access>Public</Access>
	  <Value>MultiText</Value>
	  <Value>Option</Value>
	  <Value>OptionCollection</Value>
	  <Value>Picture</Value>
	  <Value>RelationToOneEntry</Value>
	</Entity>
	<Entity type="Enum">
	  <Name>FieldNames</Name>
	  <Access>Public</Access>
	  <Value>EntryLexicalForm</Value>
	  <Value>ExampleSentence</Value>
	  <Value>ExampleTranslation</Value>
	</Entity>
	<Entity type="Class">
	  <Name>WsIdCollectionSerializerFactory</Name>
	  <Access>Internal</Access>
	  <Member type="Method">public sealed override IXmlMemberSerialiser Create(ReflectorMember member, ReflectorPropertyAttribute attribute)</Member>
	  <Member type="Constructor">public WsIdCollectionSerializerFactory()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>WsIdCollectionSerializer</Name>
	  <Access>Internal</Access>
	  <Member type="Method">public override Void Write(XmlWriter writer, Object target)</Member>
	  <Member type="Method">public override Object Read(XmlNode node, NetReflectorTypeTable table)</Member>
	  <Member type="Constructor">public WsIdCollectionSerializer(ReflectorMember member, ReflectorPropertyAttribute attribute)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>WeSayStringConverter</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public abstract String[] ValidStrings { get; }</Member>
	  <Member type="Method">public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context)</Member>
	  <Member type="Method">public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType)</Member>
	  <Member type="Method">public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)</Member>
	  <Member type="Method">public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context)</Member>
	  <Member type="Method">public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)</Member>
	  <Modifier>Abstract</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>ParentClassConverter</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public virtual String[] ValidStrings { get; }</Member>
	  <Member type="Constructor">public ParentClassConverter()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>DataTypeClassConverter</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public virtual String[] ValidStrings { get; }</Member>
	  <Member type="Constructor">public DataTypeClassConverter()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Db4oLexEntryRepository</Name>
	  <Access>Internal</Access>
	  <Member type="Property">internal Int32 ActivationCount { get; }</Member>
	  <Member type="Constructor">public Db4oLexEntryRepository(String path)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftMerger</Name>
	  <Access>Internal</Access>
	  <Member type="Event">public event EventHandler&lt;EntryCreatedEventArgs&gt; EntryCreatedEvent</Member>
	  <Member type="Property">public IList&lt;String&gt; ExpectedOptionTraits { get; }</Member>
	  <Member type="Property">public IList&lt;String&gt; ExpectedOptionCollectionTraits { get; }</Member>
	  <Member type="Method">public sealed override LexEntry GetOrMakeEntry(Extensible eInfo, Int32 order)</Member>
	  <Member type="Method">public sealed override Void EntryWasDeleted(Extensible info, DateTime dateDeleted)</Member>
	  <Member type="Method">public sealed override LexSense GetOrMakeSense(LexEntry entry, Extensible eInfo, String rawXml)</Member>
	  <Member type="Method">public sealed override LexSense GetOrMakeSubsense(LexSense sense, Extensible info, String rawXml)</Member>
	  <Member type="Method">public sealed override LexExampleSentence GetOrMakeExample(LexSense sense, Extensible eInfo)</Member>
	  <Member type="Method">public sealed override Void MergeInLexemeForm(LexEntry entry, LiftMultiText forms)</Member>
	  <Member type="Method">public sealed override Void MergeInCitationForm(LexEntry entry, LiftMultiText contents)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInPronunciation(LexEntry entry, LiftMultiText contents, String rawXml)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInVariant(LexEntry entry, LiftMultiText contents, String rawXml)</Member>
	  <Member type="Method">public sealed override Void MergeInGloss(LexSense sense, LiftMultiText forms)</Member>
	  <Member type="Method">public sealed override Void MergeInExampleForm(LexExampleSentence example, LiftMultiText forms)</Member>
	  <Member type="Method">public sealed override Void MergeInTranslationForm(LexExampleSentence example, String type, LiftMultiText forms, String rawXml)</Member>
	  <Member type="Method">public sealed override Void MergeInSource(LexExampleSentence example, String source)</Member>
	  <Member type="Method">public sealed override Void MergeInDefinition(LexSense sense, LiftMultiText contents)</Member>
	  <Member type="Method">public sealed override Void MergeInPicture(LexSense sense, String href, LiftMultiText caption)</Member>
	  <Member type="Method">public sealed override Void MergeInNote(WeSayDataObject extensible, String type, LiftMultiText contents)</Member>
	  <Member type="Method">public sealed override WeSayDataObject GetOrMakeParentReversal(WeSayDataObject parent, LiftMultiText contents, String type)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInReversal(LexSense sense, WeSayDataObject parent, LiftMultiText contents, String type, String rawXml)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInEtymology(LexEntry entry, String source, LiftMultiText form, LiftMultiText gloss, String rawXml)</Member>
	  <Member type="Method">public sealed override Void ProcessRangeElement(String range, String id, String guid, String parent, LiftMultiText description, LiftMultiText label, LiftMultiText abbrev)</Member>
	  <Member type="Method">public sealed override Void ProcessFieldDefinition(String tag, LiftMultiText description)</Member>
	  <Member type="Method">public sealed override Void MergeInGrammaticalInfo(WeSayDataObject senseOrReversal, String val, List&lt;Trait&gt; traits)</Member>
	  <Member type="Method">public sealed override Void MergeInField(WeSayDataObject extensible, String typeAttribute, DateTime dateCreated, DateTime dateModified, LiftMultiText contents, List&lt;Trait&gt; traits)</Member>
	  <Member type="Method">public sealed override Void MergeInTrait(WeSayDataObject extensible, Trait trait)</Member>
	  <Member type="Method">public sealed override Void MergeInRelation(WeSayDataObject extensible, String relationFieldId, String targetId, String rawXml)</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Method">public sealed override Void FinishEntry(LexEntry entry)</Member>
	  <Member type="Constructor">public LiftMerger(LiftRepository repository)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>EntryCreatedEventArgs</Name>
	  <Access>Public</Access>
	  <Member type="Constructor">public EntryCreatedEventArgs(LexEntry entry)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LexEntryRepository</Name>
	  <Access>Public</Access>
	  <Member type="Property">public sealed override DateTime LastModified { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanQuery { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanPersist { get; }</Member>
	  <Member type="Method">public sealed override LexEntry CreateItem()</Member>
	  <Member type="Method">public LexEntry CreateItem(Extensible eInfo)</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public sealed override Int32 CountAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId GetId(LexEntry item)</Member>
	  <Member type="Method">public sealed override LexEntry GetItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void SaveItems(IEnumerable&lt;LexEntry&gt; items)</Member>
	  <Member type="Method">public sealed override ResultSet&lt;LexEntry&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public sealed override Void SaveItem(LexEntry item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(LexEntry item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(RepositoryId repositoryId)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public Int32 GetHomographNumber(LexEntry entry, WritingSystem headwordWritingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedByHeadword(WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedByLexicalForm(WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedByGloss(WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedBySemanticDomain(String fieldName)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithMatchingGlossSortedByLexicalForm(LanguageForm glossForm, WritingSystem lexicalUnitWritingSystem)</Member>
	  <Member type="Method">public LexEntry GetLexEntryWithMatchingId(String id)</Member>
	  <Member type="Method">public LexEntry GetLexEntryWithMatchingGuid(Guid guid)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithSimilarLexicalForm(String lexicalForm, WritingSystem writingSystem, ApproximateMatcherOptions matcherOptions)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithMatchingLexicalForm(String lexicalForm, WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithMissingFieldSortedByLexicalUnit(Field field, WritingSystem lexicalUnitWritingSystem)</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Method">public IList&lt;RepositoryId&gt; GetItemsModifiedSince(DateTime last)</Member>
	  <Member type="Method">public Void BackendDoLiftUpdateNow(Boolean p)</Member>
	  <Member type="Method">public Boolean BackendConsumePendingLiftUpdates()</Member>
	  <Member type="Method">public Boolean BackendBringCachesUpToDate()</Member>
	  <Member type="Method">public Void BackendLiftIsFreshNow()</Member>
	  <Member type="Method">public Void BackendRecoverUnsavedChangesOutOfCacheIfNeeded()</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Constructor">public LexEntryRepository(String path)</Member>
	  <Member type="Constructor">public LexEntryRepository(IRepository&lt;LexEntry&gt; decoratedRepository)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftUpdateService</Name>
	  <Access>Public</Access>
	  <Member type="Event">private event EventHandler Updating</Member>
	  <Member type="Property">private String LiftDirectory { get; }</Member>
	  <Member type="Constructor">public LiftUpdateService(LexEntryRepository lexEntryRepository)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LexExampleSentence</Name>
	  <Access>Public</Access>
	  <Member type="Property">public MultiText Sentence { get; }</Member>
	  <Member type="Property">public MultiText Translation { get; }</Member>
	  <Member type="Property">public virtual Boolean IsEmpty { get; }</Member>
	  <Member type="Property">public String TranslationType { get; set; }</Member>
	  <Member type="Constructor">public LexExampleSentence(WeSayDataObject parent)</Member>
	  <Member type="Constructor">public LexExampleSentence()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>WellKnownProperties</Name>
	  <Access>Public</Access>
	  <Member type="Method">public new Boolean Contains(String fieldName)</Member>
	  <Member type="Constructor">public WellKnownProperties()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LexEntry</Name>
	  <Access>Public</Access>
	  <Member type="Property">public MultiText LexicalForm { get; }</Member>
	  <Member type="Property">public DateTime CreationTime { get; set; }</Member>
	  <Member type="Property">public DateTime ModificationTime { get; set; }</Member>
	  <Member type="Property">public IList&lt;LexSense&gt; Senses { get; }</Member>
	  <Member type="Property">public Guid Guid { get; set; }</Member>
	  <Member type="Property">public virtual Boolean IsEmpty { get; }</Member>
	  <Member type="Property">public String Id { get; set; }</Member>
	  <Member type="Property">public Boolean IsEmptyExceptForLexemeFormForPurposesOfDeletion { get; }</Member>
	  <Member type="Property">public Boolean IsBeingDeleted { get; set; }</Member>
	  <Member type="Property">public Boolean ModifiedTimeIsLocked { get; set; }</Member>
	  <Member type="Property">public MultiText CitationForm { get; }</Member>
	  <Member type="Property">public Int32 OrderForRoundTripping { get; set; }</Member>
	  <Member type="Property">public MultiText VirtualHeadWord { get; }</Member>
	  <Member type="Property">public Boolean IsDirty { get; }</Member>
	  <Member type="Method">public override String ToString()</Member>
	  <Member type="Method">public override Void SomethingWasModified(String propertyModified)</Member>
	  <Member type="Method">public Void Clean()</Member>
	  <Member type="Method">public String GetOrCreateId(Boolean doCreateEvenIfNoLexemeForm)</Member>
	  <Member type="Method">public override Void CleanUpAfterEditting()</Member>
	  <Member type="Method">public override Void CleanUpEmptyObjects()</Member>
	  <Member type="Method">public LexSense GetOrCreateSenseWithMeaning(MultiText meaning)</Member>
	  <Member type="Method">public String GetToolTipText()</Member>
	  <Member type="Method">public LanguageForm GetHeadWord(String writingSystemId)</Member>
	  <Member type="Method">public String GetHeadWordForm(String writingSystemId)</Member>
	  <Member type="Method">public Void AddRelationTarget(String relationName, String targetId)</Member>
	  <Member type="Constructor">public LexEntry()</Member>
	  <Member type="Constructor">public LexEntry(String id, Guid guid)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>WellKnownProperties</Name>
	  <Access>Public</Access>
	  <Member type="Method">public new Boolean Contains(String fieldName)</Member>
	  <Member type="Constructor">public WellKnownProperties()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LexicalFormMultiText</Name>
	  <Access>Public</Access>
	  <Member type="Property">public LexEntry Parent { get; }</Member>
	  <Member type="Constructor">public LexicalFormMultiText(WeSayDataObject parent)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Interface">
	  <Name>IFindEntries</Name>
	  <Access>Public</Access>
	  <Member type="Method">LexEntry FindFirstEntryMatchingId(String id)</Member>
	</Entity>
	<Entity type="Class">
	  <Name>PairStringLexEntryIdDisplayProvider</Name>
	  <Access>Public</Access>
	  <Member type="Method">public sealed override String GetDisplayLabel(Object item)</Member>
	  <Member type="Method">public sealed override String GetToolTip(Object item)</Member>
	  <Member type="Method">public sealed override String GetToolTipTitle(Object item)</Member>
	  <Member type="Constructor">public PairStringLexEntryIdDisplayProvider()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftExporter</Name>
	  <Access>Public</Access>
	  <Member type="Property">public String ProducerString { get; }</Member>
	  <Member type="Property">protected XmlWriter Writer { get; }</Member>
	  <Member type="Method">public Void End()</Member>
	  <Member type="Method">public virtual Void Add(LexEntry entry)</Member>
	  <Member type="Method">public Void Add(LexEntry entry, Int32 order)</Member>
	  <Member type="Method">public String GetHumanReadableId(LexEntry entry, Dictionary&lt;String, Int32&gt; idsAndCounts)</Member>
	  <Member type="Method">public Void Add(LexSense sense)</Member>
	  <Member type="Method">public Void Add(LexExampleSentence example)</Member>
	  <Member type="Method">public Void Add(String propertyName, MultiText text)</Member>
	  <Member type="Method">public Void AddDeletedEntry(LexEntry entry)</Member>
	  <Member type="Constructor">public LiftExporter(String path)</Member>
	  <Member type="Constructor">public LiftExporter(StringBuilder builder, Boolean produceFragmentOnly)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>RepositoryId</Name>
	  <Access>Public</Access>
	  <Member type="Property">public RepositoryId Empty { get; }</Member>
	  <Member type="Method">public static Boolean operator !=(RepositoryId repositoryId1, RepositoryId repositoryId2)</Member>
	  <Member type="Method">public static Boolean operator ==(RepositoryId repositoryId1, RepositoryId repositoryId2)</Member>
	  <Member type="Method">public abstract Int32 CompareTo(RepositoryId other)</Member>
	  <Member type="Method">public new abstract Boolean Equals(RepositoryId other)</Member>
	  <Member type="Method">public override Boolean Equals(Object obj)</Member>
	  <Member type="Method">public override Int32 GetHashCode()</Member>
	  <Member type="Constructor">protected RepositoryId()</Member>
	  <Member type="Constructor">static RepositoryId()</Member>
	  <Modifier>Abstract</Modifier>
	</Entity>
	<Entity type="Interface">
	  <Name>IRepository&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">DateTime LastModified { get; }</Member>
	  <Member type="Property">Boolean CanQuery { get; }</Member>
	  <Member type="Property">Boolean CanPersist { get; }</Member>
	  <Member type="Method">Void Startup(ProgressState state)</Member>
	  <Member type="Method">T CreateItem()</Member>
	  <Member type="Method">Int32 CountAllItems()</Member>
	  <Member type="Method">RepositoryId GetId(T item)</Member>
	  <Member type="Method">T GetItem(RepositoryId id)</Member>
	  <Member type="Method">Void DeleteItem(T item)</Member>
	  <Member type="Method">Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">Void DeleteAllItems()</Member>
	  <Member type="Method">RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">Void SaveItem(T item)</Member>
	  <Member type="Method">Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	</Entity>
	<Entity type="Class">
	  <Name>SynchronicRepository&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">public sealed override DateTime LastModified { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanQuery { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanPersist { get; }</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Method">public sealed override T CreateItem()</Member>
	  <Member type="Method">public sealed override Int32 CountAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId GetId(T item)</Member>
	  <Member type="Method">public sealed override T GetItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(T item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public sealed override Void SaveItem(T item)</Member>
	  <Member type="Method">public sealed override Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">public sealed override ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Constructor">public SynchronicRepository&lt;T&gt;()</Member>
	  <Member type="Constructor">public SynchronicRepository&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>RecordToken&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">public RepositoryId Id { get; }</Member>
	  <Member type="Property">public T RealObject { get; }</Member>
	  <Member type="Property">public Object this[String fieldName] { get; set; }</Member>
	  <Member type="Method">public Boolean TryGetValue(String fieldName, out Object value)</Member>
	  <Member type="Method">public static Boolean operator !=(RecordToken&lt;T&gt; recordToken1, RecordToken&lt;T&gt; recordToken2)</Member>
	  <Member type="Method">public static Boolean operator ==(RecordToken&lt;T&gt; recordToken1, RecordToken&lt;T&gt; recordToken2)</Member>
	  <Member type="Method">public sealed override Boolean Equals(RecordToken&lt;T&gt; recordToken)</Member>
	  <Member type="Method">public override Boolean Equals(Object obj)</Member>
	  <Member type="Method">public override Int32 GetHashCode()</Member>
	  <Member type="Constructor">public RecordToken&lt;T&gt;()</Member>
	  <Member type="Constructor">public RecordToken&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Permuter</Name>
	  <Access>Internal</Access>
	  <Member type="Method">public Void Permute(List&lt;Dictionary&lt;String, T&gt;&gt; results, String key, T newItem)</Member>
	  <Member type="Method">public Void Permute(List&lt;Dictionary&lt;String, T&gt;&gt; results, String key, IEnumerable newItems)</Member>
	  <Member type="Method">public Void Permute(List&lt;Dictionary&lt;String, T&gt;&gt; results, IEnumerable&lt;Dictionary&lt;String, T&gt;&gt; newItems)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>PreciseDateTime</Name>
	  <Access>Public</Access>
	  <Member type="Property">public DateTime UtcNow { get; }</Member>
	  <Member type="Constructor">static PreciseDateTime()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>ResultSet&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">public RecordToken&lt;T&gt; this[Int32 index] { get; }</Member>
	  <Member type="Property">public Int32 Count { get; }</Member>
	  <Member type="Method">public Void RemoveAll(Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(Int32 startIndex, Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(Int32 startIndex, Int32 count, Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(Int32 startIndex, Int32 count, Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(Int32 startIndex, Predicate&lt;RecordToken&lt;T&gt;&gt; match)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(RepositoryId id)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(RepositoryId id, Int32 startIndex)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(RepositoryId id, Int32 startIndex, Int32 count)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(RepositoryId id, Int32 startIndex, Int32 count)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(RepositoryId id)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(RepositoryId id, Int32 startIndex)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(T item)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(T item, Int32 startIndex)</Member>
	  <Member type="Method">public RecordToken&lt;T&gt; FindFirst(T item, Int32 startIndex, Int32 count)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(T item)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(T item, Int32 startIndex)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(T item, Int32 startIndex, Int32 count)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(RecordToken&lt;T&gt; token, Int32 startIndex, Int32 count)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(RecordToken&lt;T&gt; token)</Member>
	  <Member type="Method">public Int32 FindFirstIndex(RecordToken&lt;T&gt; token, Int32 startIndex)</Member>
	  <Member type="Method">public static explicit operator BindingList&lt;RecordToken&lt;T&gt;&gt;(ResultSet&lt;T&gt; results)</Member>
	  <Member type="Method">public sealed override IEnumerator GetEnumerator()</Member>
	  <Member type="Method">public Void Sort(params SortDefinition[] sortDefinitions)</Member>
	  <Member type="Method">public Void SortByRepositoryId()</Member>
	  <Member type="Constructor">public ResultSet&lt;T&gt;()</Member>
	  <Member type="Constructor">public ResultSet&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>SortDefinition</Name>
	  <Access>Public</Access>
	  <Member type="Property">public String Field { get; }</Member>
	  <Member type="Property">public IComparer Comparer { get; }</Member>
	  <Member type="Constructor">public SortDefinition(String field, IComparer comparer)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>DictionaryEqualityComparer&lt;Key, Value&gt;</Name>
	  <Access>Internal</Access>
	  <Member type="Method">public override Boolean Equals(Dictionary&lt;Key, Value&gt; x, Dictionary&lt;Key, Value&gt; y)</Member>
	  <Member type="Method">public override Int32 GetHashCode(Dictionary&lt;Key, Value&gt; obj)</Member>
	  <Member type="Constructor">public DictionaryEqualityComparer&lt;Key, Value&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>DatabaseModified</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public DateTime LastModified { get; set; }</Member>
	  <Member type="Constructor">public DatabaseModified()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Db4oRepository&lt;T&gt;</Name>
	  <Access>Internal</Access>
	  <Member type="Property">protected IExtObjectContainer InternalDatabase { get; }</Member>
	  <Member type="Property">internal IExtObjectContainer Database { get; }</Member>
	  <Member type="Property">public sealed override DateTime LastModified { get; private set; }</Member>
	  <Member type="Property">Boolean WeSay.Data.IRepository&lt;T&gt;.CanQuery { get; }</Member>
	  <Member type="Property">Boolean WeSay.Data.IRepository&lt;T&gt;.CanPersist { get; }</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Method">public sealed override T CreateItem()</Member>
	  <Member type="Method">public sealed override Int32 CountAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId GetId(T item)</Member>
	  <Member type="Method">public sealed override T GetItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(T item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public RepositoryId[] GetItemsModifiedSince(DateTime last)</Member>
	  <Member type="Method">public sealed override Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">public sealed override ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public sealed override Void SaveItem(T item)</Member>
	  <Member type="Method">public Boolean CanQuery()</Member>
	  <Member type="Method">public Boolean CanPersist()</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Constructor">public Db4oRepository&lt;T&gt;()</Member>
	  <Member type="Constructor">public Db4oRepository&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Db4oException</Name>
	  <Access>Internal</Access>
	  <Member type="Constructor">public Db4oException()</Member>
	  <Member type="Constructor">public Db4oException(String message)</Member>
	  <Member type="Constructor">private Db4oException(SerializationInfo info, StreamingContext context)</Member>
	  <Member type="Constructor">public Db4oException(String message, Exception innerException)</Member>
	  <Member type="Constructor">public Db4oException(String message, Int32 errorCode)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Db4oDataSource</Name>
	  <Access>Public</Access>
	  <Member type="Property">public IObjectContainer Data { get; }</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Constructor">public Db4oDataSource(String filePath)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>RecordTokenComparer&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Method">public sealed override Int32 Compare(RecordToken&lt;T&gt; x, RecordToken&lt;T&gt; y)</Member>
	  <Member type="Constructor">public RecordTokenComparer&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Query</Name>
	  <Access>Public</Access>
	  <Member type="Property">protected virtual String TypeName { get; }</Member>
	  <Member type="Property">private List&lt;KeyValuePair&lt;String, MethodInfo&gt;&gt; ResultProperties { get; }</Member>
	  <Member type="Property">private List&lt;Query&gt; NestedQueries { get; }</Member>
	  <Member type="Method">public IEnumerable&lt;Dictionary&lt;String, Object&gt;&gt; GetResults(Object o)</Member>
	  <Member type="Method">public Query In(String fieldName)</Member>
	  <Member type="Method">public Query ForEach(String fieldName)</Member>
	  <Member type="Method">public Query Show(String fieldName)</Member>
	  <Member type="Method">public Query Show(String fieldName, String label)</Member>
	  <Member type="Method">public Query ShowEach(String fieldName)</Member>
	  <Member type="Method">public Query ShowEach(String fieldName, String label)</Member>
	  <Member type="Method">public override String ToString()</Member>
	  <Member type="Constructor">internal Query(Query root, Type t)</Member>
	  <Member type="Constructor">public Query(Type t)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>PredicateQuery&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Constructor">public PredicateQuery&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>MemoryRepository&lt;T&gt;</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public sealed override DateTime LastModified { get; internal set; }</Member>
	  <Member type="Property">public virtual Boolean CanQuery { get; }</Member>
	  <Member type="Property">public virtual Boolean CanPersist { get; }</Member>
	  <Member type="Method">public virtual Void Startup(ProgressState state)</Member>
	  <Member type="Method">public virtual T CreateItem()</Member>
	  <Member type="Method">public virtual Void DeleteItem(T item)</Member>
	  <Member type="Method">public virtual Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public virtual Void SaveItem(T item)</Member>
	  <Member type="Method">public virtual Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">public virtual ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public virtual Int32 CountAllItems()</Member>
	  <Member type="Method">public virtual RepositoryId GetId(T item)</Member>
	  <Member type="Method">public virtual T GetItem(RepositoryId id)</Member>
	  <Member type="Method">public virtual Void Dispose()</Member>
	  <Member type="Constructor">public MemoryRepository&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Db4oRepositoryId</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public Int64 Db4oId { get; }</Member>
	  <Member type="Method">public new Int32 CompareTo(Db4oRepositoryId other)</Member>
	  <Member type="Method">public override Int32 CompareTo(RepositoryId other)</Member>
	  <Member type="Method">public override Boolean Equals(RepositoryId other)</Member>
	  <Member type="Method">public static Boolean operator !=(Db4oRepositoryId db4oRepositoryId1, Db4oRepositoryId db4oRepositoryId2)</Member>
	  <Member type="Method">public static Boolean operator ==(Db4oRepositoryId db4oRepositoryId1, Db4oRepositoryId db4oRepositoryId2)</Member>
	  <Member type="Method">public new Boolean Equals(Db4oRepositoryId db4oRepositoryId)</Member>
	  <Member type="Method">public override Boolean Equals(Object obj)</Member>
	  <Member type="Method">public override Int32 GetHashCode()</Member>
	  <Member type="Method">public override String ToString()</Member>
	  <Member type="Constructor">public Db4oRepositoryId(Int64 id)</Member>
	  <Modifier>None</Modifier>
	</Entity>
  </Entities>
  <Relations>
	<Relation type="Nesting" first="0" second="1" />
	<Relation type="Nesting" first="5" second="6" />
	<Relation type="Nesting" first="5" second="7" />
	<Relation type="Nesting" first="13" second="14" />
	<Relation type="Nesting" first="13" second="15" />
	<Relation type="Nesting" first="13" second="16" />
	<Relation type="Nesting" first="13" second="17" />
	<Relation type="Nesting" first="13" second="18" />
	<Relation type="Nesting" first="23" second="24" />
	<Relation type="Nesting" first="27" second="28" />
	<Relation type="Nesting" first="29" second="30" />
	<Relation type="Generalization" first="20" second="19" />
	<Relation type="Generalization" first="21" second="19" />
  </Relations>
  <Positions>
	<Shape>
	  <Location left="5" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="172" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="339" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="506" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="673" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="840" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="1007" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="1174" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="1341" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="1508" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="1675" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="1842" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="2009" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="2176" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="2343" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="2510" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="2677" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="2844" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="3011" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="3178" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="3345" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="3512" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="3679" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="3846" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="4013" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="4180" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="4347" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="4514" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="4681" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="4848" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="5015" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="5182" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="5349" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="5516" top="5" />
	  <Size width="162" height="216" />
	</Shape>
	<Shape>
	  <Location left="5683" top="5" />
	  <Size width="162" height="289" />
	</Shape>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="0" />
	  <EndNode isHorizontal="False" location="0" />
	</Connection>
  </Positions>
</ClassProject>
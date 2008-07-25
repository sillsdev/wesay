<ClassProject>
  <Language>CSharp</Language>
  <Entities>
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
	  <Member type="Method">public IList&lt;RepositoryId&gt; GetItemsModifiedSince(DateTime last)</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Constructor">public LexEntryRepository(String path)</Member>
	  <Member type="Constructor">public LexEntryRepository(IRepository&lt;LexEntry&gt; decoratedRepository)</Member>
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
  </Entities>
  <Relations>
	<Relation type="Realization" first="0" second="2" />
	<Relation type="Association" first="1" second="4">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Association" first="4" second="3">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Association" first="0" second="2">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>True</IsComposition>
	</Relation>
  </Relations>
  <Positions>
	<Shape>
	  <Location left="614" top="417" />
	  <Size width="846" height="595" />
	</Shape>
	<Shape>
	  <Location left="13" top="122" />
	  <Size width="316" height="238" />
	</Shape>
	<Shape>
	  <Location left="1297" top="2" />
	  <Size width="303" height="320" />
	</Shape>
	<Shape>
	  <Location left="13" top="1082" />
	  <Size width="550" height="272" />
	</Shape>
	<Shape>
	  <Location left="13" top="417" />
	  <Size width="550" height="595" />
	</Shape>
	<Connection>
	  <StartNode isHorizontal="False" location="782" />
	  <EndNode isHorizontal="False" location="99" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="70" />
	  <EndNode isHorizontal="False" location="70" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="66" />
	  <EndNode isHorizontal="False" location="66" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="True" location="160" />
	  <EndNode isHorizontal="False" location="252" />
	</Connection>
  </Positions>
</ClassProject>
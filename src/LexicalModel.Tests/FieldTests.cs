using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class FieldTests
	{
		private bool _eventFired;

		[SetUp]
		public void Setup()
		{
			_eventFired = false;
		}

		[TearDown]
		public void TearDown() { }

		[Test]
		public void CreateNoWritingSystem()
		{
			Field field = new Field("fieldName", "LexEntry", new string[] { });
			Assert.IsNotNull(field);
		}

		[Test]
		public void CreateSingleWritingSystem()
		{
			Field field = new Field("fieldName", "LexEntry", new string[] { "writingSystemId" });
			Assert.IsNotNull(field);
		}

		[Test]
		public void CreateMultipleWritingSystems()
		{
			Field field = new Field("fieldName",
									"LexEntry",
									new string[]
										{"writingSystemId1", "writingSystemId2", "writingSystemId3"});
			Assert.IsNotNull(field);
		}

		[Test]
		public void Create_NullFieldName_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Field(null, "LexEntry", new string[] { "writingSystem" }));
		}

		[Test]
		public void Create_NullClassName_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Field("fieldName", null, new string[] { "writingSystem" }));
		}

		[Test]
		public void Create_NullWritingSystem_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Field("fieldName", "LexEntry", null));
		}

		[Test]
		public void Create_NullWritingSystems_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Field("fieldName", "LexEntry", new string[] { null, null }));
		}

		[Test]
		public void HasWritingSystem_Null_False()
		{
			Field field = new Field("fieldName",
									"LexEntry",
									new string[]
										{"writingSystemId1", "writingSystemId2", "writingSystemId3"});
			Assert.IsFalse(field.HasWritingSystem(null));
		}

		[Test]
		public void HasWritingSystem_Exists_True()
		{
			Field field = new Field("fieldName",
									"LexEntry",
									new string[]
										{"writingSystemId1", "writingSystemId2", "writingSystemId3"});
			Assert.IsTrue(field.HasWritingSystem("writingSystemId3"));
		}

		[Test]
		public void HasWritingSystem_NotExists_False()
		{
			Field field = new Field("fieldName",
									"LexEntry",
									new string[]
										{"writingSystemId1", "writingSystemId2", "writingSystemId3"});
			Assert.IsFalse(field.HasWritingSystem("3"));
			Assert.IsFalse(field.HasWritingSystem(String.Empty));
		}

		[Test]
		public void GetFieldName_InitializedFromConstructor()
		{
			Field field = new Field("fieldName", "LexEntry", new string[] { "writingSystemId" });
			Assert.AreEqual("fieldName", field.FieldName);
		}

		[Test]
		public void GetWritingSystems()
		{
			Field field = new Field("fieldName",
									"LexEntry",
									new string[]
										{"writingSystemId1", "writingSystemId2", "writingSystemId3"});
			Assert.AreEqual(3, field.WritingSystemIds.Count);
			Assert.IsTrue(field.WritingSystemIds.Contains("writingSystemId1"));
		}

		[Test]
		[Ignore("Semantics have changed, per John")]
		public void InvalidWritingSystemNotConveyedToMaster()
		{
			Field master = new Field("foo", "LexEntry", new string[] { "dropme", "keepme" });
			Field user = new Field("foo", "LexEntry", new string[] { "dropmetoo", "keepme" });
			Field.ModifyMasterFromUser(master, user);

			Assert.AreEqual(1, master.WritingSystemIds.Count);
			Assert.IsTrue(master.WritingSystemIds.Contains("keepme"));
		}

		[Test]
		public void MasterWritingSystemsTrimmedByUser()
		{
			Field master = new Field("foo", "LexEntry", new string[] { "dropme", "keepme" });
			Field user = new Field("foo", "LexEntry", new string[] { "keepme" });
			Field.ModifyMasterFromUser(master, user);

			Assert.AreEqual(1, master.WritingSystemIds.Count);
			Assert.IsTrue(master.WritingSystemIds.Contains("keepme"));
			Assert.IsFalse(master.WritingSystemIds.Contains("dropme"));
		}

		[Test]
		public void WritingSystems_Changed_FiresEvent()
		{
			Field field = new Field();
			field.WritingSystemsChanged += OnWritingSystemIdsChanged;
			field.WritingSystemIds.Add("en-US");
			Assert.That(_eventFired, Is.True);
		}

		private void OnWritingSystemIdsChanged(object sender, EventArgs e)
		{
			_eventFired = true;
		}

		[Test]
		public void WritingSystems_SwitchedOut_FiresEvent()
		{
			Field field = new Field();
			field.WritingSystemsChanged += OnWritingSystemIdsChanged;
			field.WritingSystemIds = new List<string>();
			Assert.That(_eventFired, Is.True);
		}

		[Test]
		public void WritingSystems_SwitchedOutThenChanged_FiresEvent()
		{
			Field field = new Field();
			field.WritingSystemIds = new List<string>();
			field.WritingSystemsChanged += OnWritingSystemIdsChanged;
			field.WritingSystemIds.Add("en-US");
			Assert.That(_eventFired, Is.True);
		}
	}
}
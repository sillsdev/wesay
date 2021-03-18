using NUnit.Framework;
using SIL.WritingSystems;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture]
	public class FieldDetailControlTests
	{
		private FieldDetailControl _control;
		private Field _field;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Sldr.Initialize(true);
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			Sldr.Cleanup();
		}

		[SetUp]
		public void Setup()
		{
			BasilProjectTestHelper.InitializeForTests();
			_control = new FieldDetailControl();

			_field = new Field("test", "LexEntry", new string[] {"en"});
			_control.CurrentField = _field;
		}

		[TearDown]
		public void TearDown()
		{
			_control.Dispose();
		}

		[Test]
		public void DescriptionChangedEvent_FieldDescriptionSetBeforeEvent()
		{
			string descriptionOfFieldInEvent = string.Empty;
			_field.Description = "original description";
			_control.DescriptionOfFieldChanged +=
					delegate { descriptionOfFieldInEvent = _field.Description; };
			_control._description.Text = "new description";
			Assert.AreEqual("new description", descriptionOfFieldInEvent);
		}

		[Test]
		public void IsSpellingEnabled_FieldPersistedOnChange()
		{
			bool newIsSpellCheckingEnabled = !_field.IsSpellCheckingEnabled;
			_control._enableSpelling.Checked = newIsSpellCheckingEnabled;
			Assert.AreEqual(newIsSpellCheckingEnabled, _field.IsSpellCheckingEnabled);
		}
	}
}
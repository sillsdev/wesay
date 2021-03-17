using System;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.TextBoxes;
using System.Windows.Forms;
using System.Drawing;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class WeSayTextBoxTests : NUnitFormTest
	{
		private Form _window;

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			_window = new Form();
			_window.Size = new Size(500, 500);

		}

		[TearDown]
		public override void TearDown()
		{
			_window.Dispose();
			base.TearDown();
		}

		[Test]
		public void Create()
		{
			IWeSayTextBox textBox = new WeSayTextBox();
			Assert.IsNotNull(textBox);
		}

		[Test]
		public void CreateWithWritingSystem()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition() {DefaultFont = new FontDefinition("Arial")};
			IWeSayTextBox textBox = new WeSayTextBox(ws, null);
			Assert.IsNotNull(textBox);
			Assert.AreSame(ws, textBox.WritingSystem);
		}

		[Test]
		public void SetWritingSystem_Null_Throws()
		{
			IWeSayTextBox textBox = new WeSayTextBox();
			Assert.Throws<ArgumentNullException>(() => textBox.WritingSystem = null);
		}

		[Test]
		public void WritingSystem_Unassigned_Get_Throws()
		{
			IWeSayTextBox textBox = new WeSayTextBox();
			WritingSystemDefinition ws;
			Assert.Throws<InvalidOperationException>(() => ws = textBox.WritingSystem);
		}

		[Test]
		public void WritingSystem_Unassigned_Focused_Throws()
		{
			IWeSayTextBox textBox = new WeSayTextBox();
			Assert.Throws<InvalidOperationException>(() => textBox.AssignKeyboardFromWritingSystem());
		}

		[Test]
		public void WritingSystem_Unassigned_Unfocused_Throws()
		{
			IWeSayTextBox textBox = new WeSayTextBox();
			Assert.Throws<InvalidOperationException>(() => textBox.ClearKeyboard());
		}

		[Test]
		[Platform(Exclude="Unix")]
		[Ignore("FLAKY test - sometimes fails in tc installer build release tests")]
		public void TextReflectsKeystrokes()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr") {DefaultFont = new FontDefinition("Arial")};
			IWeSayTextBox textBox = new WeSayTextBox(ws, "_textToSearchForBox");
			_window.Controls.Add((Control)textBox);
			_window.Show();
			TextBoxTester t = new TextBoxTester("_textToSearchForBox", _window);
			KeyboardController keyboardController = new KeyboardController(t);
			t.Properties.Focus();
			keyboardController.Press(Key.T);
			Assert.IsTrue(textBox.Text.Equals("t"));
			keyboardController.Dispose();

		}
	}
}
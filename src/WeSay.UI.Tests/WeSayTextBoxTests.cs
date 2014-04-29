using System;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using Palaso.WritingSystems;
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
		public void Setup()
		{
			base.Setup();
			_window = new Form();
			_window.Size = new Size(500, 500);

		}

		[TearDown]
		public void TearDown()
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
			IWritingSystemDefinition ws = new WritingSystemDefinition();
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
			IWritingSystemDefinition ws;
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
		public void TextReflectsKeystrokes()
		{
			IWritingSystemDefinition ws = WritingSystemDefinition.Parse("fr");
			IWeSayTextBox textBox = new WeSayTextBox(ws, "_textToSearchForBox");
			_window.Controls.Add((Control)textBox);
			_window.Show();
			TextBoxTester t = new TextBoxTester("_textToSearchForBox", _window);
			KeyboardController keyboardController = new KeyboardController(t);
			t.Properties.Focus();
			keyboardController.Press("Test");
			keyboardController.Press("e");
			keyboardController.Press("s");
			keyboardController.Press("t");
			Assert.IsTrue(textBox.Text.Equals("Testest"));
			keyboardController.Dispose();

		}
	}
}
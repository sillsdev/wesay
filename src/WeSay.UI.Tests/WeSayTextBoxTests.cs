using System;
using NUnit.Framework;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.TextBoxes;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class WeSayTextBoxTests
	{
		[SetUp]
		public void Setup() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Create()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			Assert.IsNotNull(textBox);
		}

		[Test]
		public void CreateWithWritingSystem()
		{
			WritingSystem ws = new WritingSystem();
			WeSayTextBox textBox = new WeSayTextBox(ws, null);
			Assert.IsNotNull(textBox);
			Assert.AreSame(ws, textBox.WritingSystem);
		}

		[Test]
		public void SetWritingSystem_Null_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			Assert.Throws<ArgumentNullException>(() => textBox.WritingSystem = null);
		}

		[Test]
		public void WritingSystem_Unassigned_Get_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			WritingSystem ws;
			Assert.Throws<InvalidOperationException>(() => ws= textBox.WritingSystem);
		}

		[Test]
		public void WritingSystem_Unassigned_Focused_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			Assert.Throws<InvalidOperationException>(() => textBox.AssignKeyboardFromWritingSystem());
		}

		[Test]
		public void WritingSystem_Unassigned_Unfocused_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			Assert.Throws<InvalidOperationException>(() => textBox.ClearKeyboard());
		}
	}
}
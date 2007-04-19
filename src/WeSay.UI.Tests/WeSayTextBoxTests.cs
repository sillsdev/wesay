using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Language;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class WeSayTextBoxTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[TearDown]
		public void TearDown()
		{

		}

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
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetWritingSystem_Null_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			textBox.WritingSystem = null;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WritingSystem_Unassigned_Get_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			WritingSystem ws = textBox.WritingSystem;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WritingSystem_Unassigned_Focused_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			textBox.AssignKeyboardFromWritingSystem();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WritingSystem_Unassigned_Unfocused_Throws()
		{
			WeSayTextBox textBox = new WeSayTextBox();
			textBox.ClearKeyboard();
		}
	}
}
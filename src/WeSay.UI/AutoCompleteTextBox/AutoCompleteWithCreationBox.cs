using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.UI
{
	public partial class AutoCompleteWithCreationBox : UserControl
	{

		public class CreateNewArgs : EventArgs
		{
			public string LabelOfNewItem;
			private object _NewlyCreatedItem;

			public CreateNewArgs(string labelOfNewItem)
			{
				LabelOfNewItem = labelOfNewItem;
			}

			/// <summary>
			/// Receiver fills this in after creating something
			/// </summary>
			public object NewlyCreatedItem
			{
				get
				{
					return _NewlyCreatedItem;
				}
				set
				{
					_NewlyCreatedItem = value;
				}
			}
		}

		public event EventHandler<CreateNewArgs> CreateNewClicked;

		public AutoCompleteWithCreationBox()
		{
			InitializeComponent();
			UpdateElementWidth();
			_textBox.SelectedItemChanged += new EventHandler(_textBox_SelectedItemChanged);
		}

		void _textBox_SelectedItemChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		public WeSay.UI.WeSayAutoCompleteTextBox Box
		{
			get
			{
				return this._textBox;
			}
		}

		void box_TextChanged(object sender, EventArgs e)
		{
			UpdateElementWidth();
		}

		private void UpdateElementWidth()
		{
			SuspendLayout();
			if (this._textBox.Text.Length == 0)
			{
				_textBox.Width = _textBox.MinimumSize.Width;
				return;
			}


			using (Graphics g = _textBox.CreateGraphics())
			{
				TextFormatFlags flags = TextFormatFlags.TextBoxControl |
										TextFormatFlags.Default |
										TextFormatFlags.NoClipping | TextFormatFlags.LeftAndRightPadding;

				Size sz = TextRenderer.MeasureText(g, _textBox.Text,
												   _textBox.Font,
												   new Size(int.MinValue, _textBox.Height),
												   flags);

				_textBox.Width = Math.Max(_textBox.MinimumSize.Width, sz.Width);
			}
			if(_addNewButton.Visible)
			{
				this.Width = _textBox.Width + _addNewButton.Width;
			}
			else
			{
				this.Width = _textBox.Width;
			}
			_addNewButton.Left = _textBox.Width;
			ResumeLayout(false);
		}

		private void UpdateDisplay()
		{
			_addNewButton.Visible = (_textBox.SelectedItem == null);
			UpdateElementWidth();
		}

		private void OnAddNewButton_Click(object sender, EventArgs e)
		{
			Debug.Assert(CreateNewClicked != null, "Doesn't make sense to use this class without CreateNewClicked handler.");
			CreateNewArgs creationArgs = new CreateNewArgs(_textBox.Text);
			CreateNewClicked.Invoke(this, creationArgs);
			_textBox.SelectedItem = creationArgs.NewlyCreatedItem;
		}
	}
}
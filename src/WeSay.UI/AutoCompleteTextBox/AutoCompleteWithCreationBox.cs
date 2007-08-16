using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.UI
{
	public partial class AutoCompleteWithCreationBox<T> : UserControl, IBindableControl<T>
		where T:class
	{
		public event EventHandler<CreateNewArgs> CreateNewClicked;

		public AutoCompleteWithCreationBox()
		{
			InitializeComponent();
			_textBox.SelectedItemChanged += new EventHandler(OnSelectedItemChanged);
			this.GotFocus += new EventHandler(OnFocusChanged);
			_textBox.GotFocus += new EventHandler(OnFocusChanged);
			this.LostFocus += new EventHandler(OnFocusChanged);
			_textBox.LostFocus += new EventHandler(OnFocusChanged);
			_addNewButton.LostFocus += new EventHandler(OnFocusChanged);
			UpdateDisplay();
		}


		void OnFocusChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
			Invalidate();//to reshow any warning icons we don't show in a different focus situation
		}

		void OnSelectedItemChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this, null);
			}
		}

		public WeSay.UI.WeSayAutoCompleteTextBox Box
		{
			get
			{
				return this._textBox;
			}
		}
		protected override void  OnPaint(PaintEventArgs e)
		{
			 base.OnPaint(e);
			 if (!this.ContainsFocus &&  HasProblems)
			 {
				 int y = e.ClipRectangle.Top;
				 e.Graphics.DrawString("!", new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
					 Brushes.Red, e.ClipRectangle.Left + _textBox.Width + 10, y);
			 }
		}

		private bool HasProblems
		{
			get
			{
				//note, this combines two different problems:
				//1) the user typed in a non-matching form and didn't hit the create button
				//2) the relation exists, but the target cannot be found.
				return Box.SelectedItem == null && !string.IsNullOrEmpty(Box.Text);
			}
		}


		#region IBindableControl<T> Members

		public event EventHandler ValueChanged;
		public event EventHandler GoingAway;

		 public T Value
		{
			get
			{
				return Box.SelectedItem as T;
			}
			set
			{
				Box.SelectedItem = value;
			}
		}

		#endregion

		void box_TextChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null);//shake any bindings to us loose
			}
			GoingAway = null;
			base.OnHandleDestroyed(e);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null); //shake any bindings to us loose
			}
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}


		private void UpdateElementWidth()
		{
			SuspendLayout();
			if (this._textBox.Text.Length == 0)
			{
				_textBox.Width = _textBox.MinimumSize.Width;
				return;
			}

			//NB:... doing CreateGraphics makes a bunch of events fire


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
			_addNewButton.Visible = _textBox.SelectedItem == null
				&& !string.IsNullOrEmpty(_textBox.Text)
				&& this.ContainsFocus;
			UpdateElementWidth();
		}

		private void OnAddNewButton_Click(object sender, EventArgs e)
		{
			Debug.Assert(CreateNewClicked != null, "Doesn't make sense to use this class without CreateNewClicked handler.");
			CreateNewArgs creationArgs = new CreateNewArgs(_textBox.Text);
			CreateNewClicked.Invoke(this, creationArgs);
			_textBox.SelectedItem = creationArgs.NewlyCreatedItem;
		}


		/// <summary>
		/// Use to make a new object from a simple form, and to notify the control of what
		/// object was created.
		/// </summary>
		public class CreateNewArgs : EventArgs
		{
			public string LabelOfNewItem;
			private object _newlyCreatedItem;

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
					return _newlyCreatedItem;
				}
				set
				{
					_newlyCreatedItem = value;
				}
			}
		}
	}
}
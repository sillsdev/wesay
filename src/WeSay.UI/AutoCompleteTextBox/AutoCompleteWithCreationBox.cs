using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Lift;
using Palaso.UiBindings;

namespace WeSay.UI.AutoCompleteTextBox
{
	public partial class AutoCompleteWithCreationBox<KV, ValueT>: UserControl,
																  IBindableControl<ValueT>
			where ValueT : class
	{
		private readonly CommonEnumerations.VisibilitySetting _visibility;
		public event EventHandler<CreateNewArgs> CreateNewClicked;

		public delegate ValueT GetValueFromKeyValueDelegate(KV t);

		public delegate KV GetKeyValueFromValueDelegate(ValueT t);

		private GetValueFromKeyValueDelegate _getValueFromKeyValueDelegate;
		private GetKeyValueFromValueDelegate _getKeyValueFromValueDelegate;

		public AutoCompleteWithCreationBox(CommonEnumerations.VisibilitySetting visibility)
		{
			_visibility = visibility;
			InitializeComponent();

			if (DesignMode)
			{
				return;
			}

			//todo: what other cases make sense
			if (visibility == CommonEnumerations.VisibilitySetting.ReadOnly)
			{
				_textBox.ReadOnly = true;
				//  _textBox.Enabled = false;
				_textBox.TabStop = false;

				TabStop = false;
			}

			_textBox.SelectedItemChanged += OnSelectedItemChanged;
			GotFocus += OnFocusChanged;
			_textBox.GotFocus += OnFocusChanged;
			LostFocus += OnFocusChanged;
			_textBox.LostFocus += OnFocusChanged;
			AddNewButton.LostFocus += OnFocusChanged;
			BackColorChanged += OnBackColorChanged;
			UpdateDisplay();
			GetValueFromKeyValue = CastKeyValueToValue;
			GetKeyValueFromValue = CastValueToKeyValue;

			_textBox.SizeChanged += _textBox_SizeChanged;
		}

		/// <summary>
		/// used when showing read only
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBackColorChanged(object sender, EventArgs e)
		{
			if (_textBox != null && _textBox.ReadOnly)
			{
				_textBox.BackColor = BackColor;
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			size.Height = GetPreferredHeight();
			return size;
		}

		private void _textBox_SizeChanged(object sender, EventArgs e)
		{
			Height = GetPreferredHeight();
		}

		private int GetPreferredHeight()
		{
			return Math.Max(_addNewButton.Height, _textBox.Height);
		}

		private static KV CastValueToKeyValue(ValueT v)
		{
			return (KV) ((object) v);
		}

		internal Button AddNewButton
		{
			get { return _addNewButton; }
		}

		private static ValueT CastKeyValueToValue(KV t)
		{
			return (ValueT) ((object) t);
		}

		private void OnFocusChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
			Invalidate(); //to reshow any warning icons we don't show in a different focus situation
		}

		private void OnSelectedItemChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this, null);
			}
		}

		public WeSayAutoCompleteTextBox Box
		{
			get { return _textBox; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (!ContainsFocus && HasProblems && !Box.ListBoxFocused)
			{
				int y = e.ClipRectangle.Top;
				e.Graphics.DrawString("!",
									  new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
									  Brushes.Red,
									  e.ClipRectangle.Left + _textBox.Width + 10,
									  y);
			}
		}

		internal bool HasProblems
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

		public override string Text
		{
			get { return Box.Text; }
			set { Box.Text = value; }
		}

		public ValueT Value
		{
			get
			{
				if (Box.SelectedItem == null)
				{
					return null;
				}
				else
				{
					return GetValueFromKeyValue.Invoke((KV) Box.SelectedItem);
				}
			}
			set { Box.SelectedItem = GetKeyValueFromValue.Invoke(value); }
		}

		public GetValueFromKeyValueDelegate GetValueFromKeyValue
		{
			get { return _getValueFromKeyValueDelegate; }
			set { _getValueFromKeyValueDelegate = value; }
		}

		public GetKeyValueFromValueDelegate GetKeyValueFromValue
		{
			get { return _getKeyValueFromValueDelegate; }
			set { _getKeyValueFromValueDelegate = value; }
		}

		#endregion

		private void box_TextChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null); //shake any bindings to us loose
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
			if (_textBox.Text.Length == 0)
			{
				_textBox.Width = _textBox.MinimumSize.Width;
				return;
			}

			//NB:... doing CreateGraphics makes a bunch of events fire
			SuspendLayout();
			using (Graphics g = _textBox.CreateGraphics())
			{
				TextFormatFlags flags = TextFormatFlags.TextBoxControl | TextFormatFlags.Default |
										TextFormatFlags.NoClipping |
										TextFormatFlags.LeftAndRightPadding;

				Size sz = TextRenderer.MeasureText(g,
												   _textBox.Text,
												   _textBox.Font,
												   new Size(int.MaxValue, _textBox.Height),
												   flags);

				_textBox.Width = Math.Max(_textBox.MinimumSize.Width, sz.Width);
			}
			if (AddNewButton.Visible)
			{
				Width = _textBox.Width + AddNewButton.Width;
			}
			else
			{
				Width = _textBox.Width;
			}
			AddNewButton.Left = _textBox.Width;
			ResumeLayout(false);
		}

		private void UpdateDisplay()
		{
			AddNewButton.Visible = CreateNewClicked != null && _textBox.SelectedItem == null &&
								   !string.IsNullOrEmpty(_textBox.Text) && ContainsFocus;
			UpdateElementWidth();

			//   if (ContainsFocus)
			{
				if (Box.SelectedItem != null)
				{
					Box.ForeColor = Color.Black;
				}
				else
				{
					Box.ForeColor = Color.DarkBlue;
				}
			}
			//this behavior may have to become a parameter
			if (!ContainsFocus && HasProblems && !Box.ListBoxFocused)
			{
				Box.BackColor = Color.Red;
			}
			else
			{
				Box.BackColor = SystemColors.Window;
			}
		}

		public void CreateNewObjectFromText()
		{
			Debug.Assert(CreateNewClicked != null,
						 "This shouldn't be called if CreateNewClicked handler is missing.");
			CreateNewArgs creationArgs = new CreateNewArgs(_textBox.Text);
			CreateNewClicked.Invoke(this, creationArgs);
			_textBox.SelectedItem = creationArgs.NewlyCreatedItem;
		}

		private void OnAddNewButton_Click(object sender, EventArgs e)
		{
			CreateNewObjectFromText();
		}
	}

	/// <summary>
	/// Use to make a new object from a simple form, and to notify the control of what
	/// object was created.
	/// </summary>
	public class CreateNewArgs: EventArgs
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
			get { return _newlyCreatedItem; }
			set { _newlyCreatedItem = value; }
		}
	}
}
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for Button.
	/// </summary>
	[DefaultEvent("Click")]
	public class Button : System.Windows.Forms.Control, System.Windows.Forms.IButtonControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Button()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserMouse, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.StandardClick, true);
			SetStyle(ControlStyles.StandardDoubleClick, true);

			TextAlign = DevAge.Drawing.ContentAlignment.MiddleCenter;
			StringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
			base.BackColor = Color.Transparent;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//
			// Button
			//
			this.Name = "Button";
			this.Size = new System.Drawing.Size(72, 24);

		}
		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			ButtonDrawStyle buttonStyle;
			bool focused = Focused;

			if (Enabled)
			{
				if (m_Pressed)
					buttonStyle = ButtonDrawStyle.Pressed;
				else if (m_MouseOver)
					buttonStyle = ButtonDrawStyle.Hot;
				else if (m_Default)
					buttonStyle = ButtonDrawStyle.DefaultButton;
				else
					buttonStyle = ButtonDrawStyle.Normal;
			}
			else
			{
				buttonStyle = ButtonDrawStyle.Disabled;
				focused = false;
			}

			ThemePainter.CurrentProvider.DrawButton(this, e.Graphics, ClientRectangle, buttonStyle, focused, Text, m_stringFormat, Image, ImageAlign, true);
		}

		private bool m_MouseOver = false;
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter (e);
			m_MouseOver = true;
			Invalidate();
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);
			m_MouseOver = false;
			Invalidate();
		}
		private bool m_Pressed = false;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
			m_Pressed = true;
			Invalidate();
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			m_Pressed = false;
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
			Invalidate();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);
			Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				m_Pressed = true;
				PerformClick();
				e.Handled = true;
			}
			base.OnKeyDown (e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			m_Pressed = false;
			Invalidate();
			base.OnKeyUp (e);
		}

		protected override void OnClick(EventArgs e)
		{
			Form frm = this.FindForm();
			if (frm != null)
				frm.DialogResult = this.DialogResult;

			base.OnClick (e);
			Invalidate();
		}

		protected override bool ProcessMnemonic(char charCode)
		{
			if (Control.IsMnemonic(charCode, Text))
			{
				PerformClick();
				return true;
			}
			return base.ProcessMnemonic(charCode);
		}


		private StringFormat m_stringFormat = new StringFormat();
		public StringFormat StringFormat
		{
			get{return m_stringFormat;}
			set
			{
				m_stringFormat = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Text Alignment. This property is only a wrapper around StringFormat
		/// </summary>
		public Drawing.ContentAlignment TextAlign
		{
			get
			{
				return Drawing.Utilities.StringFormatToContentAlignment(m_stringFormat);
			}

			set
			{
				Drawing.Utilities.ApplyContentAlignmentToStringFormat(value, m_stringFormat);
				Invalidate();
			}
		}

		private Drawing.ContentAlignment m_ImageAlignment = Drawing.ContentAlignment.MiddleLeft;
		public Drawing.ContentAlignment ImageAlign
		{
			get{return m_ImageAlignment;}
			set
			{
				m_ImageAlignment = value;
				Invalidate();
			}
		}

		private Image m_Image;

		public Image Image
		{
			get{return m_Image;}
			set
			{
				m_Image = value;
				Invalidate();
			}
		}

		[Browsable(true)]
		public override string Text
		{
			get{return base.Text;}
			set
			{
				base.Text = value;
				Invalidate();
			}
		}

		private bool m_Default = false;
		public bool IsDefault
		{
			get{return m_Default;}
		}

		[Browsable(false)]
		public new Color BackColor
		{
			get{return base.BackColor;}
			set{base.BackColor = value;}
		}

		#region IButtonControl Members
		private System.Windows.Forms.DialogResult m_DialogResult = DialogResult.None;
		public System.Windows.Forms.DialogResult DialogResult
		{
			get{return m_DialogResult;}
			set{m_DialogResult = value;}
		}

		public void PerformClick()
		{
			if (CanSelect)
				OnClick(EventArgs.Empty);
		}

		public void NotifyDefault(bool value)
		{
			m_Default = value;
		}
		#endregion
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for DropDownButton.
	/// </summary>
	[DefaultEvent("Click")]
	public class DropDownButton : System.Windows.Forms.Control
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DropDownButton()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserMouse, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.StandardClick, true);
			SetStyle(ControlStyles.StandardDoubleClick, true);

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
			this.Name = "DropDownButton";
			this.Size = new System.Drawing.Size(72, 24);

		}
		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			ControlDrawStyle buttonStyle;

			if (Enabled)
			{
				if (m_Pressed)
					buttonStyle = ControlDrawStyle.Pressed;
				else if (m_MouseOver)
					buttonStyle = ControlDrawStyle.Hot;
				else
					buttonStyle = ControlDrawStyle.Normal;
			}
			else
				buttonStyle = ControlDrawStyle.Disabled;

			ThemePainter.CurrentProvider.DrawDropDownButton(this, e.Graphics, ClientRectangle, buttonStyle);
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
			m_Pressed = true;
			Invalidate();
			base.OnMouseDown (e);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			m_Pressed = false;
			Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				m_Pressed = true;
				OnClick(EventArgs.Empty);
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
			base.OnClick (e);
			Invalidate();
		}

		[Browsable(false)]
		public new Color BackColor
		{
			get{return base.BackColor;}
			set{base.BackColor = value;}
		}
	}
}

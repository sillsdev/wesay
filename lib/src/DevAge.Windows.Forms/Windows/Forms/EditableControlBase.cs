using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for EditableControlBase.
	/// </summary>
	public class EditableControlBase : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditableControlBase()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserMouse, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			BackColor = DefaultColor;
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
			components = new System.ComponentModel.Container();
		}
		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			if (mBorderStyle == BorderStyle.System)
				ThemePainter.CurrentProvider.DrawEditableControlPanel(this, e.Graphics, ClientRectangle);
			else if (mBorderStyle == BorderStyle.None)
				e.Graphics.Clear(BackColor);

		}

		private static Color DefaultColor = System.Drawing.Color.FromKnownColor( System.Drawing.KnownColor.Window );
		[DefaultValue( typeof(Color), "Window")]
		public new Color BackColor
		{
			get{return base.BackColor;}
			set{base.BackColor = value;}
		}

		private BorderStyle mBorderStyle = BorderStyle.System;
		[DefaultValue(BorderStyle.System)]
		public BorderStyle BorderStyle
		{
			get{return mBorderStyle;}
			set{mBorderStyle = value;OnBorderStyleChanged(EventArgs.Empty);}
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			Invalidate();
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				if (BorderStyle == BorderStyle.None)
					return base.DisplayRectangle;
				else
					return Rectangle.Inflate(base.DisplayRectangle, -1, -1);
			}
		}

		protected void SetContentAndButtonLocation(System.Windows.Forms.Control content, System.Windows.Forms.Control rightButton)
		{
			Rectangle displayRectangle = DisplayRectangle;

			rightButton.Bounds = new Rectangle(displayRectangle.Right - 18,
				displayRectangle.Y,
				18,
				displayRectangle.Height);
			int right = rightButton.Location.X;
			content.Bounds = new Rectangle(displayRectangle.Location, new Size(right, displayRectangle.Height));
		}
	}

	public enum BorderStyle
	{
		None,
		System
	}
}

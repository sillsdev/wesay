#region History

//	name:			ArrowButton control
//	author:			(c) 2005 by Alexander Kloep
//					mail to: alexander.kloep@gmx.net
//	version:		1.0
//	last modified:	2005/08/30
//
//	date		| modification
//	------------+----------------------------------------------------
//	2005/07/02	| 1st beta version
//	2005/07/07	| Designer related stuff
//	2005/08/22	| Bugfix with HoverColor
//				| Bugfix with OnClickEvent
//	2005/08/30	| Formating


#endregion

#region Using

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;

#endregion

namespace ArrowButton
{
	#region Enums

	public enum eButtonAction : int
	{
		PRESSED	= 0x01,
		FOCUS,
		MOUSEOVER,
		ENABLED
	};

	#endregion

	[Description("Arrow Button Control")]
	[Designer(typeof(ArrowButtonDesigner))]
	public class ArrowButton : System.Windows.Forms.Control
	{

		#region Designer generated code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			//
			// ArrowButton
			//
			this.Name = "ArrowButton";
			this.Size = new System.Drawing.Size(48, 48);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ArrowButton_MouseUp);
			this.MouseEnter += new System.EventHandler(this.ArrowButton_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.ArrowButton_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ArrowButton_MouseDown);

		}
		#endregion

		#region Events / Delegates

		public delegate void ArrowButtonClickDelegate ( object sender, EventArgs e );
		public event ArrowButtonClickDelegate  OnClickEvent;

		#endregion

		#region Members

		private System.ComponentModel.Container components = null;
		private Point[] m_pnts = null;						// Array with the arrow points
		private Point m_CntPnt;								// Centerpoint
		GraphicsPath m_gp = new GraphicsPath ();			// Build the arrow
		private BitArray m_ButtonState = new BitArray ();	// actual button state ( pressed, focus etc. )
		private int m_nRotDeg = 0;							// Rotatin in degrees
		private Color m_ColorS = Color.WhiteSmoke;			// Used start-, endcolor for the OnPaint method
		private Color m_ColorE = Color.DarkGray;
		private Color m_NormalStartColor = Color.WhiteSmoke;// In normal state
		private Color m_NormalEndColor = Color.DarkGray;
		private Color m_HoverStartColor = Color.WhiteSmoke;	// If Mousecursor is over
		private Color m_HoverEndColor = Color.DarkRed;
		private bool m_isStubby;

		#endregion

		#region Constants

		private	const int MINSIZE = 24;						// Minimum squaresize

		#endregion

		#region Properties / DesignerProperties

		/// <summary>
		/// Startcolor for the GradientBrush
		/// </summary>
		[Description("The start color"), Category("ArrowButton")]
		public Color NormalStartColor
		{
			get { return m_NormalStartColor; }
			set { m_NormalStartColor = value; }
		}

		/// <summary>
		/// Endcolor for the GradientBrush
		/// </summary>
		[Description("The end color"), Category("ArrowButton")]
		public Color NormalEndColor
		{
			get { return m_NormalEndColor; }
			set { m_NormalEndColor = value; Refresh ();	}
		}

		[Description("The hover start color"), Category("ArrowButton")]
		public Color HoverStartColor
		{
			get { return m_HoverStartColor; }
			set { m_HoverStartColor = value; Refresh (); }
		}

		[Description("The hover end color"), Category("ArrowButton")]
		public Color HoverEndColor
		{
			get { return m_HoverEndColor; }
			set { m_HoverEndColor = value; Refresh (); }
		}

		[Description("Is arrow enabled"), Category("ArrowButton")]
		public bool ArrowEnabled
		{
			get { return m_ButtonState[ (int)eButtonAction.ENABLED ]; }
			set { m_ButtonState[ (int)eButtonAction.ENABLED ] = value; }
		}

		[Description("Pointing direction"), Category("ArrowButton")]
		public int Rotation
		{
			get { return m_nRotDeg; }
			set
			{
				m_nRotDeg = value;
				Clear ();
				Init ();
				Refresh ();
			}
		}

		//JH JDH WeSay added
		[Description("StubbyStyle"), Category("ArrowButton")]
		public bool StubbyStyle
		{
			get { return m_isStubby; }
			set
			{
				m_isStubby = value;
				Clear();
				Init();
				Refresh();
			}
		}

		#endregion

		#region Constructors

		public ArrowButton()
		{
			InitializeComponent();
			Init ();
			// Make the paintings faster and flickerfree
			SetStyle ( ControlStyles.UserPaint, true );
			SetStyle ( ControlStyles.ResizeRedraw, true );
			SetStyle ( ControlStyles.DoubleBuffer, true );
			SetStyle ( ControlStyles.UserPaint, true );
			SetStyle ( ControlStyles.AllPaintingInWmPaint, true );

			// Inital state
			m_ButtonState[ (int)eButtonAction.PRESSED ] = false;
			m_ButtonState[ (int)eButtonAction.ENABLED ] = true;

		}

		#endregion

		#region Overrides

		/// <summary>
		/// Repaint ( recalc ) the arrow
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			Clear ();
			Init ();
			Refresh ();
		}

		/// <summary>
		/// Paint the arrow.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if ( m_ButtonState [ (int)eButtonAction.MOUSEOVER ] == true )
			{
				m_ColorS = HoverStartColor;
				m_ColorE = HoverEndColor;
			}
			else
			{
				m_ColorS = NormalStartColor;
				m_ColorE = NormalEndColor;
			}

			Rectangle rect = this.ClientRectangle;
			LinearGradientBrush	b = new LinearGradientBrush  ( rect, m_ColorS, m_ColorE, 0, true  );
			// no clipping at design time
			if ( this.DesignMode != true )
			{
				e.Graphics.SetClip ( m_gp );
			}
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillPath ( b, m_gp );
			b.Dispose ();
			ColorArrowFrame ( e, m_ButtonState );
			DrawArrowText ( e.Graphics );
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sequence to create the button
		/// </summary>
		private void Init ()
		{
			// Check if clientrect is a square
			MakeSquare ();

			// Make the arrow smaller than the panelwidth, because
			// the diagonal from arrowhead to an edge from the arrowbottom,
			// is bigger than the panelwidth and so the edges were clipped
			// during rotation.
			int dx = this.Width - 3;

			// The arrow consist of eight points ( 0=7 )
			BuildInitialArrow ( dx );

			// Calculate the CenterPoint position
			m_CntPnt = new Point ( this.Width / 2, this.Width / 2 );

			// Turn arrow around the CenterPoint
			RotateArrow ( Rotation );

			// Prevent clipping
			MoveCenterPoint ( dx );

			// Build the graphical path out of the arrowpoints
			GraphicsPathFromPoints ( m_pnts, m_gp, m_CntPnt );

		}

		/// <summary>
		/// To prevent clipping of the arrow edges after an
		/// rotation, we shift the CenterPoint. For that we
		/// must only check the points 0 and 1
		/// </summary>
		private void MoveCenterPoint ( int dx )
		{
			// Sector I
			if ( ( m_nRotDeg >= 0 ) && ( m_nRotDeg <= 90 ) )
			{
				int cy = ( m_pnts[1].Y  ) - ( ( dx / 2 ) );
				if ( cy > 0 )
				{
					m_CntPnt.Y -= cy;
				}
				int cx = ( m_pnts[0].X ) + ( ( dx / 2 ) );
				if ( cx < 0 )
				{
					m_CntPnt.X -= cx;
				}
			}

			// Sector II
			if ( ( m_nRotDeg >= 91 ) && ( m_nRotDeg <= 180 ) )
			{
				int cy = ( m_pnts[0].Y  ) - ( ( dx / 2 ) );
				if ( cy > 0 )
				{
					m_CntPnt.Y += cy;
				}
				int cx = ( m_pnts[1].X ) + ( ( dx / 2 ) );
				if ( cx < 0 )
				{
					m_CntPnt.X -= cx;
				}
			}

			// Sector III
			if ( ( m_nRotDeg >= 181 ) && ( m_nRotDeg <= 270 ) )
			{
				int cy = ( m_pnts[1].Y  ) + ( ( dx / 2 ) );
				if ( cy < 0 )
				{
					m_CntPnt.Y -= cy;
				}
				int cx = ( m_pnts[1].X ) + ( ( dx / 2 ) );
				if ( cx < 0 )
				{
					m_CntPnt.X -= cx;
				}
			}

			// Sector IV
			if ( ( m_nRotDeg >= 271 ) && ( m_nRotDeg <= 360 ) )
			{
				int cy = ( m_pnts[0].Y  ) - ( ( dx / 2 ) );
				if ( cy > 0 )
				{
					m_CntPnt.Y += cy;
				}
				int cx = ( m_pnts[1].X ) - ( ( dx / 2 ) );
				if ( cx > 0 )
				{
					m_CntPnt.X -= cx;
				}
			}
		}


		/// <summary>
		/// Build the startarrow. it is a upward pointing arrow.
		/// </summary>
		/// <param name="dx">The maximum height and width of the panel</param>
		private void BuildInitialArrow ( int dx )
		{
			// The arrow consist of eight points
			m_pnts = new Point[8];


			// The initial points build an arrow in up-direction
			m_pnts[0] = new Point (  - dx / 4,	+ dx / 2);
			m_pnts[1] = new Point (  + dx / 4,	+ dx / 2);
			m_pnts[2] = new Point (  + dx / 4,	0 );
			if (m_isStubby)
			{
				m_pnts[3] = new Point(+dx / 4, 0);
				m_pnts[5] = new Point(-dx / 4, 0);
			}
			else
			{
				m_pnts[3] = new Point(+dx / 2, 0);
				m_pnts[5] = new Point(-dx / 2, 0);
			}
			m_pnts[4] = new Point (			0,	- dx / 2 );

			m_pnts[6] = new Point (  - dx / 4,	0 );
			m_pnts[7] = new Point (  - dx / 4,	+ dx / 2 );

//            m_pnts[0] = new Point(-dx / 4, +dx / 2);
//            m_pnts[1] = new Point(+dx / 4, +dx / 2);
//            m_pnts[2] = new Point(+dx / 4, 0);
//            m_pnts[3] = new Point(+dx / 2, 0);
//            m_pnts[4] = new Point(0, -dx / 2);
//            m_pnts[5] = new Point(-dx / 2, 0);
//            m_pnts[6] = new Point(-dx / 4, 0);
//            m_pnts[7] = new Point(-dx / 4, +dx / 2);
		}

		/// <summary>
		/// If the placeholder is not exact a square,
		/// make it a square
		/// </summary>
		private void MakeSquare ()
		{
			this.SuspendLayout ();
			if ( this.Width < MINSIZE )
			{
				this.Size = new Size ( MINSIZE, MINSIZE );
			}
			else
			{
				if ( this.Size.Width < this.Size.Height )
				{
					this.Size = new Size ( this.Size.Width, this.Size.Width );
				}
				else
				{
					this.Size = new Size ( this.Size.Height, this.Size.Height );
				}
			}
			this.ResumeLayout ();
		}


		/// <summary>
		/// Create the arrow as a GraphicsPath from the point array
		/// </summary>
		/// <param name="pnts">Array with points</param>
		/// <param name="gp">The GraphicsPath object</param>
		/// <param name="hs">Point with offset data</param>
		private void GraphicsPathFromPoints ( Point[] pnts, GraphicsPath gp, Point hs )
		{
			for ( int i = 0;	i < pnts.Length - 1;	i++ )
			{
				gp.AddLine (  hs.X + pnts[i].X,  hs.Y + pnts[i].Y,  hs.X + pnts[i+1].X,  hs.Y + pnts[i+1].Y );
			}
		}


		/// <summary>
		/// Display a the text on the arrow
		/// </summary>
		/// <param name="g"></param>
		private void DrawArrowText ( Graphics g )
		{
			if ( Text == String.Empty )
				return;
			StringFormat f = new StringFormat();
			f.Alignment = StringAlignment.Center;
			f.LineAlignment = StringAlignment.Center;
			float dx = 0;
			float dy = 0;
			if ( ( m_ButtonState [ (int)eButtonAction.PRESSED ] ) &&
				 ( m_ButtonState[ (int)eButtonAction.ENABLED ] == true ) )
			{
				dx = 1 / g.DpiX;
				dy = 1 / g.DpiY;
			}
			g.PageUnit = GraphicsUnit.Inch;
			g.TranslateTransform ( ( ClientRectangle.Width / g.DpiX ) / 2, ( ClientRectangle.Height / g.DpiY ) / 2 );
			// to prevent that the text is not readable, add 90 degrees
			// to turn in a readable direction ( 175 and 330 are arbitrary values )
			if ( ( Rotation >= 175 ) && ( Rotation <= 330 ) )
			{
				g.RotateTransform( Rotation + 90 );
			}
			else
			{
				g.RotateTransform( Rotation + 270 );
			}
			Color c = ForeColor;
			if ( m_ButtonState[ (int)eButtonAction.ENABLED ] == false )
			{
				c = SystemColors.GrayText;//.InactiveCaptionText;
			}
			g.DrawString ( Text, Font, new SolidBrush ( c ), 0 + dx, 0 + dy, f );
		}

		/// <summary>
		/// Simply clear the points and reset the graphicpath
		/// </summary>
		private void Clear ()
		{
			m_pnts = null;
			m_gp.Reset ();
		}

		/// <summary>
		/// With different colors around the arrow we engender the 3d effect.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="eAction">The "Button" action state of the control</param>
		private void ColorArrowFrame ( PaintEventArgs e, BitArray butstate )
		{
			if ( m_ButtonState[ (int)eButtonAction.ENABLED ] )
			{
				Pen p1 = null;
				if ( butstate [ (int)eButtonAction.PRESSED ] == false )
				{
					for ( int i = 0;	i < 7;	i++ )
					{
						if ( m_pnts[i].Y <= m_pnts[i+1].Y )
						{
							p1 = new Pen ( SystemColors.ControlLightLight, 2 );
						}
						else
						{
							p1 = new Pen ( SystemColors.ControlDark, 2 );
						}
						e.Graphics.DrawLine ( p1, m_CntPnt.X + m_pnts[i].X,  m_CntPnt.Y + m_pnts[i].Y,  m_CntPnt.X + m_pnts[i+1].X,  m_CntPnt.Y + m_pnts[i+1].Y );
					}
				}

				if ( butstate [  (int)eButtonAction.PRESSED ] == true )
				{
					for ( int i = 0;	i < 7;	i++ )
					{
						if ( m_pnts[i].Y <= m_pnts[i+1].Y )
						{
							p1 = new Pen ( SystemColors.ControlDark, 2 );
						}
						else
						{
							p1 = new Pen ( SystemColors.ControlLightLight, 2 );
						}
						e.Graphics.DrawLine ( p1, m_CntPnt.X + m_pnts[i].X,  m_CntPnt.Y + m_pnts[i].Y,  m_CntPnt.X + m_pnts[i+1].X,  m_CntPnt.Y + m_pnts[i+1].Y );
					}
				}
			}
		}

		/// <summary>
		/// Rotate the arrow around the CenterPoint to get different
		/// pointing directions.
		/// </summary>
		/// <param name="nDeg">Rotation in degree</param>
		private void RotateArrow ( int nDeg )
		{
			// only values between 0 and 360
			if ( nDeg > 360 )
			{
				nDeg -= 360;
			}

			m_nRotDeg = nDeg;
			double bog = ( Math.PI / 180 ) * nDeg;
			double cosA = Math.Cos ( bog );
			double sinA = Math.Sin ( bog );

			for ( int i = 0;	i < 8;	i++ )
			{
				int a = m_pnts[i].X;
				int b = m_pnts[i].Y;

				double x = ( a * cosA ) - ( b * sinA );
				double y = ( b * cosA ) + ( a * sinA );

				m_pnts[i].X = (int)x;
				m_pnts[i].Y = (int)y;
			}
		}


		#endregion

		#region Mouseactions

		private void ArrowButton_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.PRESSED ] = false;
			Refresh();
		}

		private void ArrowButton_MouseLeave(object sender, System.EventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.MOUSEOVER ] = false;
			Refresh();
		}

		private void ArrowButton_MouseEnter(object sender, System.EventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.MOUSEOVER ] = true;
			Refresh();
		}

		private void ArrowButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.PRESSED ] = true;
			Refresh();
			OnArrowClick ( e );
		}


		#endregion

		#region EventHandler function

		protected void OnArrowClick ( EventArgs e )
		{
			if ( OnClickEvent != null )
			{
				OnClickEvent ( this, e );
			}
		}

		#endregion

		#region Disposing

		/// <summary>
		/// Clear the used resources
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

	}

	#region Class Bitarray

	public class BitArray
	{
		#region Constants

		const int NUMBITS = 32;		// Bits = int = 4 byte = 32 bits

		#endregion

		#region Members

		private int m_Bits;

		#endregion

		#region Indexer

		/// <summary>
		/// The Indexer for a 32 bit array
		/// </summary>
		public bool this[int BitPos]
		{
			get
			{
				BitPosValid ( BitPos );
				return ( ( m_Bits & ( 1 << ( BitPos % 8 ) ) ) != 0 );
			}
			set
			{
				BitPosValid ( BitPos );
				// Set the bit to 1
				if ( value )
				{
					m_Bits |= ( 1 << ( BitPos % 8 ) );
				}
				// Set the bit to 0
				else
				{
					m_Bits &= ~( 1 << ( BitPos % 8 ) );
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Check if the wanted bit is in a valid Range
		/// </summary>
		/// <param name="BitPos"></param>
		private void BitPosValid ( int BitPos )
		{
			if ( ( BitPos < 0) || ( BitPos >= NUMBITS ) )
			{
				throw new ArgumentOutOfRangeException ();
			}
		}

		/// <summary>
		/// Clear all bits in the "bitarray"
		/// </summary>
		public void Clear ()
		{
			m_Bits = 0x00;
		}

		#endregion
	}

	#endregion

	#region Class ArrowButtonDesigner

	public class ArrowButtonDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		#region Constructors

		public ArrowButtonDesigner()
		{
		}

		#endregion

		#region Overrides

		protected override void PostFilterProperties(IDictionary Properties)
		{
			Properties.Remove("AllowDrop");
			Properties.Remove("BackColor");
			Properties.Remove("BackgroundImage");
			Properties.Remove("ContextMenu");
			Properties.Remove("FlatStyle");
			Properties.Remove("Image");
			Properties.Remove("ImageAlign");
			Properties.Remove("ImageIndex");
			Properties.Remove("ImageList");
			Properties.Remove("TextAlign");
			Properties.Remove("Enabled");
		}

		#endregion
	}

	#endregion
}

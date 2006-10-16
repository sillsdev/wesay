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

//  2006   | WeSay modified from "ArrowButton".  Currently a good portion of this code is not needed,
//                      as the arrow button did all kinds of rotation/shading stuff we don't need.

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

namespace WeSay.UI
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

	[Description("Flag Button Control")]
	[Designer(typeof(FlagButtonDesigner))]
	public class FlagButton : System.Windows.Forms.Control
	{

		#region Designer generated code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			//
			// FlagButton
			//
			this.Name = "FlagButton";
			this.Size = new System.Drawing.Size(48, 48);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FlagButton_MouseUp);
			this.MouseEnter += new System.EventHandler(this.FlagButton_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.FlagButton_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FlagButton_MouseDown);

		}
		#endregion

		#region Events / Delegates

		public delegate void FlagButtonClickDelegate ( object sender, EventArgs e );
		public event FlagButtonClickDelegate  OnClickEvent;

		#endregion

		#region Members

		private System.ComponentModel.Container components = null;
		private Point[] _points = null;						// Array with the Flag points
		private Point _centerPoint;								// Centerpoint
		GraphicsPath _graphicsPath = new GraphicsPath ();			// Build the Flag
		private BitArray m_ButtonState = new BitArray ();	// actual button state ( pressed, focus etc. )
		private int m_nRotDeg = 0;							// Rotatin in degrees
		private Color startFillHiliteColor = Color.WhiteSmoke;			// Used start-, endcolor for the OnPaint method
		private Color endFillHiliteColor = Color.DarkGray;
		private Color m_NormalStartColor = Color.WhiteSmoke;// In normal state
		private Color m_NormalEndColor = Color.DarkGray;
		private Color m_HoverStartColor = Color.WhiteSmoke;	// If Mousecursor is over
		private Color m_HoverEndColor = Color.DarkRed;
		private bool    _isSetOn=false;
		private Color ActiveColor = Color.FromArgb(255,153,51);
		private bool _wasSetBeforeMouseEntered;

		#endregion

		#region Constants

		private	const int MINSIZE = 24;						// Minimum squaresize

		#endregion

		#region Properties / DesignerProperties

		/// <summary>
		/// Startcolor for the GradientBrush
		/// </summary>
		[Description("The start color"), Category("FlagButton")]
		public Color NormalStartColor
		{
			get { return m_NormalStartColor; }
			set { m_NormalStartColor = value; }
		}

		/// <summary>
		/// Endcolor for the GradientBrush
		/// </summary>
		[Description("The end color"), Category("FlagButton")]
		public Color NormalEndColor
		{
			get { return m_NormalEndColor; }
			set { m_NormalEndColor = value; Refresh ();	}
		}

		[Description("The hover start color"), Category("FlagButton")]
		public Color HoverStartColor
		{
			get { return m_HoverStartColor; }
			set { m_HoverStartColor = value; Refresh (); }
		}

		[Description("The hover end color"), Category("FlagButton")]
		public Color HoverEndColor
		{
			get { return m_HoverEndColor; }
			set { m_HoverEndColor = value; Refresh (); }
		}

		[Description("Is Flag enabled"), Category("FlagButton")]
		public bool FlagEnabled
		{
			get { return m_ButtonState[ (int)eButtonAction.ENABLED ]; }
			set { m_ButtonState[ (int)eButtonAction.ENABLED ] = value; }
		}

		[Description("Pointing direction"), Category("FlagButton")]
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

		[Description("IsSetOn"), Category("FlagButton")]
		public bool IsSetOn
		{
			get { return _isSetOn; }
			set
			{
				_isSetOn = value;
				Clear();
				Init();
				Refresh();
			}
		}

		#endregion

		#region Constructors

		public FlagButton()
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
		/// Repaint ( recalc ) the Flag
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			Clear ();
			Init ();
			Refresh ();
		}

		/// <summary>
		/// Paint the Flag.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			// no clipping at design time
			if ( this.DesignMode != true )
			{
			//	e.Graphics.SetClip ( _graphicsPath );
			}

			Rectangle rect = this.ClientRectangle;

		  //  e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			//LinearGradientBrush	fillBrush = new LinearGradientBrush(rect, startFillHiliteColor, endFillHiliteColor, 0, true  );
			if (IsSetOn && !ButtonIsDown())//&& !MouseIsHovering())
			{
				SolidBrush f = new SolidBrush(ActiveColor);
				e.Graphics.FillPath(f, _graphicsPath);
				f.Dispose ();
			}


			DrawFrame ( e );
			//DrawFlagText ( e.Graphics );
		}

		/// <summary>
		/// With different colors around the Flag we engender the 3d effect.
		/// </summary>
		private void DrawFrame(PaintEventArgs e)
		{
			Pen pen = null;

			if (IsSetOn)
			{
				pen = new Pen(ActiveColor, 1);
			}
			else if (ButtonIsDown())
			{
				pen = new Pen(ActiveColor, 3);
			}
			else if (MouseIsHovering())
			{
				pen = new Pen(ActiveColor, 1);
			}
			else
			{
				int g = 230;
				pen = new Pen(Color.FromArgb(g, g, g), 1);
			}

			_centerPoint = new Point(Width/2, Width/2);
			for (int i = 0; i < _points.Length - 1; i++)
			{
				e.Graphics.DrawLine(pen, _centerPoint.X + _points[i].X, _centerPoint.Y + _points[i].Y, _centerPoint.X + _points[i + 1].X, _centerPoint.Y + _points[i + 1].Y);
			}
		}

		private bool ButtonIsDown()
		{
			return m_ButtonState[(int)eButtonAction.PRESSED];
		}

		private bool MouseIsHovering()
		{
			return m_ButtonState[(int)eButtonAction.MOUSEOVER] == true;
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

			// Make the Flag smaller than the panelwidth, because
			// the diagonal from Flaghead to an edge from the Flagbottom,
			// is bigger than the panelwidth and so the edges were clipped
			// during rotation.
			int dx = this.Width ;

			// The Flag consist of eight points ( 0=7 )
			MakeButtonShapePoints ( dx );

			// Calculate the CenterPoint position
			_centerPoint = new Point ( this.Width / 2, this.Width / 2 );


			// Prevent clipping
			MoveCenterPoint ( dx );

			// Build the graphical path out of the Flagpoints
			GraphicsPathFromPoints ( _points, _graphicsPath, _centerPoint );


			// Change control region, now control is an Flag
		 //   Region = new Region(_graphicsPath);
		}


		/// <summary>
		/// To prevent clipping of the Flag edges after an
		/// rotation, we shift the CenterPoint. For that we
		/// must only check the points 0 and 1
		/// </summary>
		private void MoveCenterPoint ( int dx )
		{
			int cy = ( _points[1].Y  ) - ( ( dx / 2 ) );
			if ( cy > 0 )
			{
				_centerPoint.Y -= cy;
			}
			int cx = ( _points[0].X ) + ( ( dx / 2 ) );
			if ( cx < 0 )
			{
				_centerPoint.X -= cx;
			}
		}


		/// <summary>
		/// Build the startFlag. it is a upward pointing Flag.
		/// </summary>
		/// <param name="dx">The maximum height and width of the panel</param>
		private void MakeButtonShapePoints ( int dx )
		{
			_points = new Point[11];
			_points[0] = new Point(0,-46);
			_points[1] = new Point(14,-14);
			_points[2] = new Point(46,-11);
			_points[3] = new Point(22,12);
			_points[4] = new Point(30,46);
			_points[5] = new Point(0,28);
			_points[6] = new Point(-30,46);
			_points[7] = new Point(-22,12);
			_points[8] = new Point(-46,-11);
			_points[9] = new Point(-14,-14);
			_points[10] = _points[0];

			for (int i = 0; i < _points.Length; i++)
			{
				_points[i].X = (int) ((_points[i].X/92.0)*(float) dx);
				_points[i].Y = (int) ((_points[i].Y/92.0)*(float) dx);
			}





//            _points = new Point[5];
//            _points[0] = new Point(0 - dx / 2, 0 - dx / 2);
//            _points[1] = new Point(dx / 2, 0 - dx / 2);
//            _points[2] = new Point(dx / 2, dx / 2);
//            _points[3] = new Point(-dx / 2, dx / 2);
//            _points[4] = _points[0];
		}

		/// <summary>
		/// If the placeholder is not exact a square,
		/// make it a square
		/// </summary>
		private void MakeSquare ()
		{
			this.SuspendLayout ();
//			if ( this.Width < MINSIZE )
//			{
//				this.Size = new Size ( MINSIZE, MINSIZE );
//			}
//			else
//			{
				if ( this.Size.Width < this.Size.Height )
				{
					this.Size = new Size ( this.Size.Width, this.Size.Width );
				}
				else
				{
					this.Size = new Size ( this.Size.Height, this.Size.Height );
				}
//			}
			this.ResumeLayout ();
		}


		/// <summary>
		/// Create the Flag as a GraphicsPath from the point array
		/// </summary>
		/// <param name="pnts">Array with points</param>
		/// <param name="gp">The GraphicsPath object</param>
		/// <param name="hs">Point with offset data</param>
		private void GraphicsPathFromPoints ( Point[] pnts, GraphicsPath gp, Point hs )
		{
			for (int i = 0; i < pnts.Length - 1; i++)
			{
				gp.AddLine (  hs.X + pnts[i].X,  hs.Y + pnts[i].Y,  hs.X + pnts[i+1].X,  hs.Y + pnts[i+1].Y );
			}
		}



		/// <summary>
		/// Simply clear the points and reset the graphicpath
		/// </summary>
		private void Clear ()
		{
			_points = null;
			_graphicsPath.Reset ();
		}




		#endregion

		#region Mouseactions

		private void FlagButton_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.PRESSED ] = false;
			Refresh();
		}

		private void FlagButton_MouseLeave(object sender, EventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.MOUSEOVER ] = false;
			Refresh();
		}

		private void FlagButton_MouseEnter(object sender, System.EventArgs e)
		{
			_wasSetBeforeMouseEntered = IsSetOn;
			m_ButtonState[ (int)eButtonAction.MOUSEOVER ] = true;
			Refresh();
		}

		private void FlagButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_ButtonState[ (int)eButtonAction.PRESSED ] = true;
			Refresh();
			OnFlagClick ( e );
		}


		#endregion

		#region EventHandler function

		protected void OnFlagClick ( EventArgs e )
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

	#region Class FlagButtonDesigner

	public class FlagButtonDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		#region Constructors

		public FlagButtonDesigner()
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

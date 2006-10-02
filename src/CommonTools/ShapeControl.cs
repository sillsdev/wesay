//originally from http://www.codeproject.com/cs/miscctrl/ShapeControldotNET.asp

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ShapeControl
{
	//All the defined shape type
	public enum ShapeType
	{
		Rectangle,
		RoundedRectangle,
		Diamond,
		Ellipse,
		TriangleUp,
		TriangleDown,
		TriangleLeft,
		TriangleRight,
		BallonNE,
		BallonNW,
		BallonSW,
		BallonSE,
		CustomPolygon,
		CustomPie

	}


	public class ShapeControl : System.Windows.Forms.Control
	{

		private GraphicsPath _custompath = new GraphicsPath();
		private System.ComponentModel.Container components = null;
		private bool _istextset = false;
		ShapeType _shape = ShapeType.Rectangle;
		DashStyle _borderstyle = DashStyle.Solid;
		Color _bordercolor = Color.FromArgb(255, 255, 0, 0);
		int _borderwidth = 3;
		GraphicsPath _outline = new GraphicsPath();
		bool _usegradient = false;
		Color _centercolor = Color.FromArgb(100, 255, 0, 0);
		Color _surroundcolor = Color.FromArgb(100, 0, 255, 255);

		[Category("Shape"), Description("Text to display")]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{

				base.Text = value;

				//When Visual Studio first create a new control, text=name
				//we do not want any default text, thus we override it with blank
				if ((!_istextset) && (base.Text.Equals(base.Name)))
					base.Text = "";
				_istextset = true;
			}
		}



		//Overide the BackColor Property to be associated with our custom editor
		[Category("Shape"), Description("Back Color")]
		[BrowsableAttribute(true)]
//        [EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value; this.Refresh();

			}
		}


		[Category("Shape"), Description("Using Gradient to fill Shape")]
		public bool UseGradient
		{
			get { return _usegradient; }
			set { _usegradient = value; this.Refresh(); }
		}

		//For Gradient Rendering, this is the color at the center of the shape
		[Category("Shape"), Description("Color at center")]
		[BrowsableAttribute(true)]
//        [EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public Color CenterColor
		{
			get { return _centercolor; }
			set { _centercolor = value; this.Refresh(); }
		}

		//For Gradient Rendering, this is the color at the edges of the shape
		[Category("Shape"), Description("Color at the edges of the Shape")]
		[BrowsableAttribute(true)]
//        [EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public Color SurroundColor
		{
			get { return _surroundcolor; }
			set { _surroundcolor = value; this.Refresh(); }
		}


		[Category("Shape"), Description("Border Width")]
		public int BorderWidth
		{
			get { return _borderwidth; }
			set
			{
				_borderwidth = value;
				if (_borderwidth < 0) _borderwidth = 0;

				this.Refresh();
			}
		}

		[Category("Shape"), Description("Border Color")]
		[BrowsableAttribute(true)]
//        [EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public Color BorderColor
		{
			get { return _bordercolor; }
			set { _bordercolor = value; this.Refresh(); }
		}

		[Category("Shape"), Description("Border Style")]
		public DashStyle BorderStyle
		{
			get { return _borderstyle; }
			set { _borderstyle = value; this.Refresh(); }
		}

		[Category("Shape"), Description("Select Shape")]
		[BrowsableAttribute(true)]
//        [EditorAttribute(typeof(ShapeTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public ShapeType Shape
		{
			get { return _shape; }
			set
			{
				_shape = value;
				OnResize(null);
			}
		}

		public ShapeControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			//Using of Double Buffer allow for smooth rendering
			//minizing flickering
			this.SetStyle(ControlStyles.SupportsTransparentBackColor |
						  ControlStyles.DoubleBuffer |
						  ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.UserPaint, true);

			//set the default backcolor and font
			this.BackColor = Color.FromArgb(0, 255, 255, 255);
			this.Font = new Font("Arial", 12, FontStyle.Bold);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		private void InitializeComponent()
		{

			this.TextChanged += new System.EventHandler(this.ShapeControl_TextChanged);

		}

		#endregion

		//This function creates the path for each shape
		//It is also being used by the ShapeTypeEditor to create the various shapes
		//for the Shape property editor UI
		internal static void updateOutline(ref GraphicsPath outline, ShapeType shape, int width, int height)
		{
			switch (shape)
			{
				case ShapeType.CustomPie:
					outline.AddPie(0, 0, width, height, 180, 270);
					break;
				case ShapeType.CustomPolygon:
					outline.AddPolygon(new Point[]{
								  new Point(0,0),
								  new Point(width/2,height/4),
								  new Point(width,0),
								  new Point((width*3)/4,height/2),
								  new Point(width,height),
								  new Point(width/2,(height*3)/4),
								  new Point(0,height),
								  new Point(width/4,height/2)
												  }
						);
					break;
				case ShapeType.Diamond:
					outline.AddPolygon(new Point[]{
								new Point(0,height/2),
								new Point(width/2,0),
								new Point(width,height/2),
								new Point(width/2,height)
												  });
					break;

				case ShapeType.Rectangle:
					outline.AddRectangle(new Rectangle(0, 0, width, height));
					break;

				case ShapeType.Ellipse:
					outline.AddEllipse(0, 0, width, height);
					break;

				case ShapeType.TriangleUp:
					outline.AddPolygon(new Point[] { new Point(0, height), new Point(width, height), new Point(width / 2, 0) });
					break;

				case ShapeType.TriangleDown:
					outline.AddPolygon(new Point[] { new Point(0, 0), new Point(width, 0), new Point(width / 2, height) });
					break;

				case ShapeType.TriangleLeft:
					outline.AddPolygon(new Point[] { new Point(width, 0), new Point(0, height / 2), new Point(width, height) });
					break;

				case ShapeType.TriangleRight:
					outline.AddPolygon(new Point[] { new Point(0, 0), new Point(width, height / 2), new Point(0, height) });
					break;

				case ShapeType.RoundedRectangle:
					int a = 20;
					outline.AddArc(0, 0, a, a, 180, 90);
					outline.AddLine(a / 2, 0, width - a / 2, 0);
					outline.AddArc(width - a, 0, a, a, 270, 90);
					outline.AddLine(width, a / 2, width, height - width / 8);
					outline.AddArc(width - a, height - a, a, a, 0, 90);
					outline.AddLine(width - a / 2, height, a / 2, height);
					outline.AddArc(0, height - a, a, a, 90, 90);
					outline.AddLine(0, height - a / 2, 0, a / 2);
					break;

				//                case ShapeType.RoundedRectangle:
				//                    outline.AddArc(0, 0, width / 4, width / 4, 180, 90);
				//                    outline.AddLine(width / 8, 0, width - width / 8, 0);
				//                    outline.AddArc(width - width / 4, 0, width / 4, width / 4, 270, 90);
				//                    outline.AddLine(width, width / 8, width, height - width / 8);
				//                    outline.AddArc(width - width / 4, height - width / 4, width / 4, width / 4, 0, 90);
				//                    outline.AddLine(width - width / 8, height, width / 8, height);
				//                    outline.AddArc(0, height - width / 4, width / 4, width / 4, 90, 90);
				//                    outline.AddLine(0, height - width / 8, 0, width / 8);
				//                    break;

				case ShapeType.BallonSW:
					outline.AddArc(0, 0, width / 4, width / 4, 180, 90);
					outline.AddLine(width / 8, 0, width - width / 8, 0);
					outline.AddArc(width - width / 4, 0, width / 4, width / 4, 270, 90);
					outline.AddLine(width, width / 8, width, (height * 0.75f) - width / 8);
					outline.AddArc(width - width / 4, (height * 0.75f) - width / 4, width / 4, width / 4, 0, 90);
					outline.AddLine(width - width / 8, (height * 0.75f), width / 8 + (width / 4), (height * 0.75f));
					outline.AddLine(width / 8 + (width / 4), height * 0.75f, width / 8 + (width / 8), height);
					outline.AddLine(width / 8 + (width / 8), height, width / 8 + (width / 8), (height * 0.75f));
					outline.AddLine(width / 8 + (width / 8), (height * 0.75f), width / 8, (height * 0.75f));
					outline.AddArc(0, (height * 0.75f) - width / 4, width / 4, width / 4, 90, 90);
					outline.AddLine(0, (height * 0.75f) - width / 8, 0, width / 8);
					break;

				case ShapeType.BallonSE:
					outline.AddArc(0, 0, width / 4, width / 4, 180, 90);
					outline.AddLine(width / 8, 0, width - width / 8, 0);
					outline.AddArc(width - width / 4, 0, width / 4, width / 4, 270, 90);
					outline.AddLine(width, width / 8, width, (height * 0.75f) - width / 8);
					outline.AddArc(width - width / 4, (height * 0.75f) - width / 4, width / 4, width / 4, 0, 90);
					outline.AddLine(width - width / 8, (height * 0.75f), width - (width / 4), (height * 0.75f));
					outline.AddLine(width - (width / 4), height * 0.75f, width - (width / 4), height);
					outline.AddLine(width - (width / 4), height, width - (3 * width / 8), (height * 0.75f));
					outline.AddLine(width - (3 * width / 8), (height * 0.75f), width / 8, (height * 0.75f));
					outline.AddArc(0, (height * 0.75f) - width / 4, width / 4, width / 4, 90, 90);
					outline.AddLine(0, (height * 0.75f) - width / 8, 0, width / 8);
					break;

				case ShapeType.BallonNW:
					outline.AddArc(width - width / 4, (height) - width / 4, width / 4, width / 4, 0, 90);
					outline.AddLine(width - width / 8, (height), width - (width / 4), (height));
					outline.AddArc(0, (height) - width / 4, width / 4, width / 4, 90, 90);
					outline.AddLine(0, (height) - width / 8, 0, height * 0.25f + width / 8);
					outline.AddArc(0, height * 0.25f, width / 4, width / 4, 180, 90);
					outline.AddLine(width / 8, height * 0.25f, width / 4, height * 0.25f);
					outline.AddLine(width / 4, height * 0.25f, width / 4, 0);
					outline.AddLine(width / 4, 0, 3 * width / 8, height * 0.25f);
					outline.AddLine(3 * width / 8, height * 0.25f, width - width / 8, height * 0.25f);
					outline.AddArc(width - width / 4, height * 0.25f, width / 4, width / 4, 270, 90);
					outline.AddLine(width, width / 8 + height * 0.25f, width, (height) - width / 8);
					break;

				case ShapeType.BallonNE:
					outline.AddArc(width - width / 4, (height) - width / 4, width / 4, width / 4, 0, 90);
					outline.AddLine(width - width / 8, (height), width - (width / 4), (height));
					outline.AddArc(0, (height) - width / 4, width / 4, width / 4, 90, 90);
					outline.AddLine(0, (height) - width / 8, 0, height * 0.25f + width / 8);
					outline.AddArc(0, height * 0.25f, width / 4, width / 4, 180, 90);
					outline.AddLine(width / 8, height * 0.25f, 5 * width / 8, height * 0.25f);
					outline.AddLine(5 * width / 8, height * 0.25f, 3 * width / 4, 0);
					outline.AddLine(3 * width / 4, 0, 3 * width / 4, height * 0.25f);
					outline.AddLine(3 * width / 4, height * 0.25f, width - width / 8, height * 0.25f);
					outline.AddArc(width - width / 4, height * 0.25f, width / 4, width / 4, 270, 90);
					outline.AddLine(width, width / 8 + height * 0.25f, width, (height) - width / 8);
					break;

				default: break;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			if ((this.Width < 0) || (this.Height <= 0)) return;
			_outline = new GraphicsPath();

			updateOutline(ref _outline, _shape, this.Width, this.Height);
			this.Region = new Region(_outline);
			this.Refresh();
			base.OnResize(e);
		}



		protected override void OnPaint(PaintEventArgs pe)
		{
			//Rendering with Gradient
			if (_usegradient)
			{
				PathGradientBrush br = new PathGradientBrush(this._outline);
				br.CenterColor = this._centercolor;
				br.SurroundColors = new Color[] { this._surroundcolor };
				pe.Graphics.FillPath(br, this._outline);
			}

			//Rendering with Border
			if (_borderwidth > 0)
			{
				Pen p = new Pen(_bordercolor, _borderwidth * 2);
				p.DashStyle = _borderstyle;
				pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				pe.Graphics.DrawPath(p, this._outline);
				p.Dispose();
			}

			//Rendering the text to be at the center of the shape
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			switch (_shape)
			{
				case ShapeType.BallonNE:
				case ShapeType.BallonNW:
					pe.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(0, this.Height * 0.25f, this.Width, this.Height * 0.75f), sf);
					break;

				case ShapeType.BallonSE:
				case ShapeType.BallonSW:
					pe.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(0, 0, this.Width, this.Height * 0.75f), sf);
					break;

				default:
					pe.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new Rectangle(0, 0, this.Width, this.Height), sf);
					break;
			}

			// Calling the base class OnPaint

			base.OnPaint(pe);
		}


		private void ShapeControl_TextChanged(object sender, System.EventArgs e)
		{
			this.Refresh();
		}
	}
}

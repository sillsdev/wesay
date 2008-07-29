using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeSay.UI
{
	public partial class CirclesProgressIndicator: UserControl
	{
		private int _maximum;
		private int _minimum;
		private int _value;
		private int _rows;
		private Size _bulletSize;
		private Padding _bulletPadding;
		private Color _bulletColor;
		private Color _bulletColorEnd;
		private bool _highlightCurrentOnly;

		public CirclesProgressIndicator()
		{
			_maximum = 10;
			_minimum = 1;
			_value = 1;
			_rows = 1;
			_bulletSize = new Size(5, 5);
			_bulletPadding = new Padding(1);
			_bulletColor = Color.Azure;
			_bulletColorEnd = Color.MediumBlue;
			_highlightCurrentOnly = false;
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the maximum value of the range of the control.
		/// </summary>
		/// <remarks>The default is 10. The Value property will be adjusted to be less than or equal to Maximum.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">The value specified is less than Minimum.</exception>
		[DefaultValue(10)]
		public int Maximum
		{
			get { return _maximum; }
			set
			{
				if (value < _minimum)
				{
					throw new ArgumentOutOfRangeException("value",
														  value,
														  "Maximum must not be less than Minimum");
				}
				_maximum = value;
				_value = Math.Min(_value, _maximum);
				if (AutoSize)
				{
					SetAutoSize();
				}
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the minimum value of the range of the control.
		/// </summary>
		/// <remarks>The default is 1. The Value property will be adjusted to be greater than or equal to Minimum.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">The value specified is Greater than Maximum or less than 1.</exception>
		[DefaultValue(1)]
		public int Minimum
		{
			get { return _minimum; }
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value",
														  value,
														  "Minumum must not be less than 1");
				}
				if (value > _maximum)
				{
					throw new ArgumentOutOfRangeException("value",
														  value,
														  "Minimum must be less than Maximum");
				}
				_minimum = value;
				_value = Math.Max(_value, _minimum);
				if (AutoSize)
				{
					SetAutoSize();
				}
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the current position of the control.
		/// </summary>
		/// <remarks>The default is 1.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The value specified is greater than the value of the Maximum property.
		/// -or- The value specified is less than the value of the Minimum property.
		/// </exception>
		[Bindable(true)]
		[DefaultValue(1)]
		public int Value
		{
			get { return _value; }
			set
			{
				if (Minimum > value || value > Maximum)
				{
					throw new ArgumentOutOfRangeException("value",
														  value,
														  "Value must be between Minimum and Maximum");
				}
				_value = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the number of items that show as a stack.
		/// </summary>
		/// <remarks>Default value is 1.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">The value specified is less than 1.</exception>
		[DefaultValue(1)]
		public int Rows
		{
			get { return _rows; }
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value",
														  value,
														  "Value must not be less than 1");
				}
				_rows = value;
				if (AutoSize)
				{
					SetAutoSize();
				}
				Invalidate();
			}
		}

		/// <summary>
		/// Advances the current position by the specified amount.
		/// </summary>
		/// <param name="value">The amount by which to increment the current position.</param>
		public void Increment(int value)
		{
			Value = Value + value;
		}

		protected override void OnAutoSizeChanged(EventArgs e)
		{
			base.OnAutoSizeChanged(e);
			if (AutoSize)
			{
				SetAutoSize();
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (AutoSize)
			{
				SetAutoSize();
			}
		}

		public Size CellSize
		{
			get { return Size.Add(_bulletSize, _bulletPadding.Size); }
		}

		[DefaultValue(typeof (Size), "5,5")]
		public Size BulletSize
		{
			get { return _bulletSize; }
			set
			{
				_bulletSize = value;
				if (AutoSize)
				{
					SetAutoSize();
				}
				Invalidate();
			}
		}

		[DefaultValue(typeof (Padding), "1")]
		public Padding BulletPadding
		{
			get { return _bulletPadding; }
			set
			{
				_bulletPadding = value;
				if (AutoSize)
				{
					SetAutoSize();
				}
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "System.Drawing.Color.Azure")]
		public Color BulletColor
		{
			get { return _bulletColor; }
			set
			{
				_bulletColor = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "System.Drawing.Color.MediumBlue")]
		public Color BulletColorEnd
		{
			get { return _bulletColorEnd; }
			set
			{
				_bulletColorEnd = value;
				Invalidate();
			}
		}

		[DefaultValue(false)]
		public bool HighlightCurrentOnly
		{
			get { return _highlightCurrentOnly; }
			set
			{
				_highlightCurrentOnly = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			int columnCount = GetColumnCount();

			int currentItem = Minimum - 1;
			for (int column = 0;column < columnCount;column++)
			{
				for (int row = 0;row < Rows;row++)
				{
					++currentItem;
					if (currentItem > Maximum)
					{
						return;
					}
					Point location = new Point(column * CellSize.Width, row * CellSize.Height);
					location.Offset(BulletPadding.Left, BulletPadding.Top);
					Rectangle rect = new Rectangle(location.X,
												   location.Y,
												   BulletSize.Width,
												   BulletSize.Height);
					Brush brush = new LinearGradientBrush(rect,
														  BulletColor,
														  BulletColorEnd,
														  LinearGradientMode.ForwardDiagonal);
					e.Graphics.DrawEllipse(new Pen(brush), rect);
					if ((HighlightCurrentOnly && currentItem == Value) ||
						(!HighlightCurrentOnly && currentItem <= Value))
					{
						e.Graphics.FillEllipse(brush, rect);
					}
				}
			}
		}

		private void SetAutoSize()
		{
			int columnCount = GetColumnCount();
			SetClientSizeCore(columnCount * CellSize.Width, Rows * CellSize.Height);
		}

		private int GetColumnCount()
		{
			int remainder;
			int columnCount = Math.DivRem(_maximum, Rows, out remainder);
			if (remainder != 0)
			{
				++columnCount;
			}
			return columnCount;
		}
	}
}
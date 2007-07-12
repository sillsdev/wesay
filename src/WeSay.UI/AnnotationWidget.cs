using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.UI.Properties;

namespace WeSay.UI
{
	public class AnnotationWidget
	{
		private CheckBox _flagButton;

		/// <summary>
		/// This will be referencing the actual annotation of the object
		/// </summary>
		//private Annotation _annotation;

		private MultiText _multitext;
		private string _writingSystemId;
		private string _nameForTesting;
	  private bool _hot;
		private static Image CheckedImage = Resources.FlagOn.GetThumbnailImage(20, 20, ReturnFalse, IntPtr.Zero);
		private static Image HotCheckedImage = Resources.HotFlagOn.GetThumbnailImage(20, 20, ReturnFalse, IntPtr.Zero);
		private static Image UncheckedImage = Resources.FlagOff.GetThumbnailImage(20, 20, ReturnFalse, IntPtr.Zero);
		private static Image HotUncheckedImage = Resources.HotFlagOff.GetThumbnailImage(20, 20, ReturnFalse, IntPtr.Zero);

		public AnnotationWidget(MultiText multitext,
								string writingSystemId,
								string nameForTesting)
		{
			_nameForTesting = nameForTesting;
			_multitext = multitext;
			_writingSystemId = writingSystemId;
		}

		public bool FlagIsOn
		{
			get
			{
				if (_flagButton != null)
				{
					return _multitext.GetAnnotationOfAlternativeIsStarred(_writingSystemId);
				}
				return false;
			}

			set
			{
				if (_flagButton != null)
				{
					_multitext.SetAnnotationOfAlternativeIsStarred(_writingSystemId, value);
				}
			}
		}

		public Control MakeControl(Size panelSize)
		{
			_flagButton = new CheckBox();
			_flagButton.Appearance = Appearance.Button;
		  _flagButton.AutoCheck = true;
		  _flagButton.Size = new Size(22, 22);
		  _flagButton.ThreeState = false;
		  _flagButton.Text = string.Empty;
		  _flagButton.FlatStyle = FlatStyle.Flat;
		  _flagButton.FlatAppearance.BorderSize = 0;
		  _flagButton.FlatAppearance.MouseOverBackColor = Color.White;
		  _flagButton.FlatAppearance.MouseDownBackColor = Color.White;

		  _flagButton.Location = new Point(
			  -1 + panelSize.Width - _flagButton.Width,
				1);
//          _flagButton.Location = new Point(
//              -1 + panelSize.Width - _flagButton.Width,
//              -1 + panelSize.Height - _flagButton.Height);

			_flagButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_flagButton.CheckedChanged += new EventHandler(OnFlagButtonCheckedChanged);
			_flagButton.TabStop = false;
			_flagButton.Checked = FlagIsOn;
			_flagButton.Name = _nameForTesting;
			_flagButton.MouseEnter += new EventHandler(_flagButton_MouseEnter);
			_flagButton.MouseLeave += new EventHandler(_flagButton_MouseLeave);

			if (Type.GetType("Mono.Runtime") == null) // Work around because Mono draws a border around image even though it shouldn't
			{
				_flagButton.Paint += new PaintEventHandler(_flagButton_Paint);
			}
			SetFlagImage();

			return _flagButton;
		}

	  void _flagButton_Paint(object sender, PaintEventArgs e)
	  {
		  // undo the border that Mono just drew around this image Mono bug 82081
		  e.Graphics.DrawRectangle(new Pen(_flagButton.BackColor, 1), _flagButton.ClientRectangle.X, _flagButton.ClientRectangle.Y, _flagButton.ClientRectangle.Width - 1, _flagButton.ClientRectangle.Height-1);
	  }

	  void _flagButton_MouseLeave(object sender, EventArgs e)
	  {
		_hot = false;
		SetFlagImage();
	}

	  void _flagButton_MouseEnter(object sender, EventArgs e)
	  {
		_hot = true;
		SetFlagImage();
	}

	  void OnFlagButtonCheckedChanged(object sender, EventArgs e)
	  {
		  Reporting.Logger.WriteMinorEvent("OnFlagButtonCheckedChanged ({0})", _flagButton.Checked);
		SetFlagImage();

		_flagButton.Refresh(); // give visual feedback right away since the next action can take some time
		FlagIsOn = _flagButton.Checked;
	  }

	  private void SetFlagImage()
	  {
		if (_flagButton.Checked)
		{
		  if (_hot)
		  {
			_flagButton.Image = HotCheckedImage;
		  }
		  else
		  {
			_flagButton.Image = CheckedImage;
		  }
		}
		else
		{
		  if (_hot)
		  {
			_flagButton.Image = HotUncheckedImage;
		  }
		  else
		  {
			_flagButton.Image = UncheckedImage;
		  }
		}
	  }

	  private static bool ReturnFalse()
	  {
		return false;
	  }

	}
}
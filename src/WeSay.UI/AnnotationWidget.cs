using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
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
		private Annotation _annotation;

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
		  _flagButton.Size = new Size(20, 20);
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
			_flagButton.Checked = this.FlagIsOn;
			_flagButton.Name = _nameForTesting;
			_flagButton.MouseEnter += new EventHandler(_flagButton_MouseEnter);
			_flagButton.MouseLeave += new EventHandler(_flagButton_MouseLeave);
			_flagButton.Paint += new PaintEventHandler(_flagButton_Paint);

			return _flagButton;
		}

	  void _flagButton_Paint(object sender, PaintEventArgs e)
	  {
		SetFlagImage();
	  }

	  void _flagButton_MouseLeave(object sender, EventArgs e)
	  {
		_hot = false;
	  }

	  void _flagButton_MouseEnter(object sender, EventArgs e)
	  {
		_hot = true;
	  }

	  void OnFlagButtonCheckedChanged(object sender, EventArgs e)
	  {
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
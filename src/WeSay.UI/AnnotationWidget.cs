using System;
using System.Drawing;
using System.Windows.Forms;
using SIL.Lift;
using SIL.Reporting;
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
		private readonly MultiText _multitext;

		private readonly string _writingSystemId;
		private readonly string _nameForTesting;
		private bool _hot;

		private static readonly Image CheckedImage = Resources.FlagOn.GetThumbnailImage(20,
																						20,
																						ReturnFalse,
																						IntPtr.Zero);

		private static readonly Image HotCheckedImage = Resources.HotFlagOn.GetThumbnailImage(20,
																							  20,
																							  ReturnFalse,
																							  IntPtr
																									  .
																									  Zero);

		private static readonly Image UncheckedImage = Resources.FlagOff.GetThumbnailImage(20,
																						   20,
																						   ReturnFalse,
																						   IntPtr.
																								   Zero);

		private static readonly Image HotUncheckedImage = Resources.HotFlagOff.GetThumbnailImage(
				20, 20, ReturnFalse, IntPtr.Zero);

		public AnnotationWidget(MultiText multitext, string writingSystemId, string nameForTesting)
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
			_flagButton.AutoSize = true;
			_flagButton.ThreeState = false;
			_flagButton.Text = string.Empty;
			_flagButton.FlatStyle = FlatStyle.Flat;
			_flagButton.FlatAppearance.BorderSize = 0;
			_flagButton.FlatAppearance.MouseOverBackColor = Color.White;
			_flagButton.FlatAppearance.MouseDownBackColor = Color.White;

			_flagButton.Location = new Point(-1 + panelSize.Width - _flagButton.Width, 1);
			//          _flagButton.Location = new Point(
			//              -1 + panelSize.Width - _flagButton.Width,
			//              -1 + panelSize.Height - _flagButton.Height);

			_flagButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_flagButton.TabStop = false;
			_flagButton.Checked = FlagIsOn;
			_flagButton.Name = _nameForTesting;
			_flagButton.MouseEnter += _flagButton_MouseEnter;
			_flagButton.MouseLeave += _flagButton_MouseLeave;
			_flagButton.CheckedChanged += OnFlagButtonCheckedChanged;

			SetFlagImage();

			return _flagButton;
		}

		private void _flagButton_MouseLeave(object sender, EventArgs e)
		{
			_hot = false;
			SetFlagImage();
		}

		private void _flagButton_MouseEnter(object sender, EventArgs e)
		{
			_hot = true;
			SetFlagImage();
		}

		private void OnFlagButtonCheckedChanged(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("OnFlagButtonCheckedChanged ({0})", _flagButton.Checked);
			SetFlagImage();

			_flagButton.Refresh();
			// give visual feedback right away since the next action can take some time
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
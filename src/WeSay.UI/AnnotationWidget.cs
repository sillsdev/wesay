using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public class AnnotationWidget
	{
		private FlagButton _flagButton;

		/// <summary>
		/// This will be referencing the actual annotation of the object
		/// </summary>
		private Annotation _annotation;

		private MultiText _multitext;
		private string _writingSystemId;
		private string _nameForTesting;

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
					_flagButton.IsSetOn = value;

					_multitext.SetAnnotationOfAlternativeIsStarred(_writingSystemId, value);
				}
			}
		}

		public Control MakeControl(Size panelSize)
		{
			_flagButton = new FlagButton();
			_flagButton.Size = new Size(20, 20);
			_flagButton.Location = new Point(
				-1 + panelSize.Width - _flagButton.Width,
				-1 + panelSize.Height - _flagButton.Height);
			_flagButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			_flagButton.Click += new EventHandler(OnClickFlagButton);
			_flagButton.TabStop = false;
			_flagButton.IsSetOn = this.FlagIsOn;
			_flagButton.Name = _nameForTesting;

			//            Panel panel = new Panel();
			//            panel.Size = flagButton.Size;
			//            panel.Location = flagButton.Location;
			//            panel.Anchor = flagButton.Anchor;
			//            panel.BackColor = System.Drawing.Color.Red;

			return _flagButton;
		}

		private void OnClickFlagButton(object sender, EventArgs e)
		{
			FlagButton b = (FlagButton)sender;
			this.FlagIsOn = !b.IsSetOn;
		}
	}
}
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Project;

namespace WeSay.CommonTools
{
	public partial class DictionaryStatusControl: UserControl
	{
		private Size _oldLabelSize;

		public DictionaryStatusControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
			ShowLogo = false;
		}

		public DictionaryStatusControl(int count)
		{
			InitializeComponent();
			_dictionarySizeLabel.Text = String.Format(StringCatalog.Get(this._dictionarySizeLabel.Text),
													  count, WeSayWordsProject.Project.Name);
	   }

		public bool ShowLogo
		{
			set
			{
				_logoImage.Visible = value;
				UpdateSize();
			}
		}

		private void UpdateSize()
		{
			int newHeight = _logoImage.Visible ? _logoImage.Location.Y + _logoImage.Height : 0;
			newHeight =
					Math.Max(_dictionarySizeLabel.Location.Y + _dictionarySizeLabel.Height,
							 newHeight);
			Height = newHeight;
		}

		private void DictionaryStatusControl_FontChanged(object sender, EventArgs e)
		{
			_dictionarySizeLabel.Font = Font;
		}

		private void _dictionarySizeLabel_SizeChanged(object sender, EventArgs e)
		{
			if (_dictionarySizeLabel.Height != _oldLabelSize.Height)
			{
				UpdateSize();
			}
			_oldLabelSize = _dictionarySizeLabel.Size;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			_dictionarySizeLabel.MaximumSize = new Size(Width - _dictionarySizeLabel.Location.X, int.MaxValue);
			_dictionarySizeLabel.PerformLayout();
		}
	}
}

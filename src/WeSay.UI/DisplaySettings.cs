using System.Drawing;

namespace WeSay.UI
{
	public class DisplaySettings
	{
		public static DisplaySettings Default = new DisplaySettings();

		public DisplaySettings()
		{
			SetColors();
		}


		private Color _backgroundColor;
		private Color _currentIndicatorColor;
		private Color _wsLabelColor;

		private string _skinName;

		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set { _backgroundColor = value; }
		}

		public bool UsingProjectorScheme
		{
			get { return Default._skinName == "projector"; }
		}

		public Color WritingSystemLabelColor
		{
			get { return _wsLabelColor; }
			set { _wsLabelColor = value; }
		}

		public Color CurrentIndicatorColor
		{
			get { return _currentIndicatorColor; }
			set { _currentIndicatorColor = value; }
		}


		public void ToggleColorScheme()
		{
			if(_skinName != "projector")
			{
				_skinName = "projector";
			}
			else
			{
				_skinName = string.Empty;
			}
			SetColors();
		}

		public string SkinName
		{
			get
			{
				return _skinName;
			}

			set
			{
				_skinName = value;
				SetColors();
			}
		}

		private void SetColors()
		{
			if (_skinName == "projector")
			{
				BackgroundColor = System.Drawing.Color.LightGreen;
				CurrentIndicatorColor = BackgroundColor;
				_wsLabelColor = Color.Blue;
			}
			else
			{
				BackgroundColor = Color.FromArgb(235, 255, 215);
				CurrentIndicatorColor = Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(253)))), ((int)(((byte)(219)))));
				_wsLabelColor = Color.Gray;

			}
		}
	}
}

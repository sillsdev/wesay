using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace WeSay.UI.Buttons
{
	[Description("Round Button Control")]
	public class RoundButton : RegionButton
	{
		private Size _keepThisSize;
		public RoundButton()
		{
			Size = new Size(50, 50); // Default size
			this.SizeChanged += new System.EventHandler(RoundButton_SizeChanged);

			//prevent large ui font making this button grow
			_keepThisSize = new Size(Size.Width, Size.Height);
		}

		void RoundButton_SizeChanged(object sender, System.EventArgs e)
		{
			this.Size = _keepThisSize;

		}

		/// <summary>
		/// Use this to actually change the size. Had to disable normal resizing
		/// because font-changes were making these buttons grow (they shouldn't)
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void ReallySetSize(int width, int height)
		{
			_keepThisSize = new Size(width,height);
			Size = _keepThisSize;
		}

		protected override void MakeRegion()
		{
			GraphicsPath path = new GraphicsPath();
			Rectangle rectangle = ClientRectangle;
			path.AddEllipse(rectangle);
			Path = path;
			Invalidate();
		}

	}
}

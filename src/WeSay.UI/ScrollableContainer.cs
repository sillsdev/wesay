using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	public partial class ScrollableContainer : UserControl
	{
		private Control _activeControl;

		public ScrollableContainer()
		{
			InitializeComponent();
		}

		public void ScrollAccordingToEventArgs(MouseEventArgs e)
		{
			OnMouseWheel(e);
		}

		protected override Point ScrollToControl(Control activeControl)
		{
			if (activeControl == _activeControl)
			{
				return DisplayRectangle.Location;
			}
			_activeControl = activeControl;
			return base.ScrollToControl(activeControl);
		}
	}
}

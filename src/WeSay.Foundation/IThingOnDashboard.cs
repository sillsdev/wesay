using System.Drawing;

namespace Palaso.LexicalModel
{
	public interface IThingOnDashboard
	{
		DashboardGroup Group { get; }

		string LocalizedLabel { get; }

		string LocalizedLongLabel { get; }

		string Description { get; }

		ButtonStyle DashboardButtonStyle { get; }

		Image DashboardButtonImage { get; }
	}

	public enum ButtonStyle
	{
		FixedAmount,
		VariableAmount,
		IconFixedWidth,
		IconVariableWidth
	}

	public enum DashboardGroup
	{
		DontShow,
		Gather,
		Describe,
		Refine,
		Review,
		Share,
	}
}
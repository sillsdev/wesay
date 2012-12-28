using System.Drawing;

namespace WeSay.Foundation
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

	/* For the code scanner...
		StringCatalog.Get("Gather");
		StringCatalog.Get("Describe");
		StringCatalog.Get("Refine");
		StringCatalog.Get("Review");
		StringCatalog.Get("Share");
	*/
}
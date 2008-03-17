using System.Drawing;

namespace WeSay.Project
{
	public interface IThingOnDashboard
	{
		string GroupName { get; }

		string LocalizedLabel { get; }

		ButtonStyle Style { get; }

		Image Image { get; }

	}

	public enum ButtonStyle
	{
		FixedAmount,
		VariableAmount,
		IconFixedWidth,
		IconVariableWidth
	}
}
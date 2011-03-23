using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.WritingSystems;
using Palaso.WritingSystems;

namespace WeSay.ConfigTool
{
	public class WritingSystemSetup: ConfigurationControlBase
	{
		private readonly WritingSystemSetupView _view;

		public WritingSystemSetup(ILogger logger, IWritingSystemRepository store)
			: base("set up fonts, keyboards, and sorting", logger)
		{
			_view = new WritingSystemSetupView(new WritingSystemSetupModel(store))
						{
							Dock = DockStyle.Fill
						};
		}

	}
}
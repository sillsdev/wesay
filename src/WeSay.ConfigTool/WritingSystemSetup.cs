using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Code;
using Palaso.i18n;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.WritingSystems;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public class WritingSystemSetup: ConfigurationControlBase
	{
		private readonly WritingSystemSetupView _view;

		public WritingSystemSetup(ILogger logger, IWritingSystemStore store)
			: base("set up fonts, keyboards, and sorting", logger)
		{
			_view = new WritingSystemSetupView(new WritingSystemSetupModel(store));
			_view.Dock = DockStyle.Fill;
		}

	}
}
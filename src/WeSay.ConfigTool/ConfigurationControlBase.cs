using System.Windows.Forms;
using Palaso.Reporting;

namespace WeSay.ConfigTool
{
	public class ConfigurationControlBase: UserControl
	{
		private readonly string _header;
		protected readonly ILogger _logger;

		public ConfigurationControlBase(string header, ILogger logger)
		{
			_header = header;
			_logger = logger;
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		public virtual void PreLoad() {}

		//design-time
		public ConfigurationControlBase()
		{
			_header = string.Empty;
		}

		public void SetOtherStuff()
		{
			Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			BorderStyle = BorderStyle.None;
			// Padding = new Padding(15);
		}

		public string Header
		{
			get { return _header; }
		}
	}
}
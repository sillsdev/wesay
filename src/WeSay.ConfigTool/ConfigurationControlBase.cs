using System.Windows.Forms;

namespace WeSay.ConfigTool
{
	public class ConfigurationControlBase: UserControl
	{
		private readonly string _header;

		public ConfigurationControlBase(string header)
		{
			_header = header;
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
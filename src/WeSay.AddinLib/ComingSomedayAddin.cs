using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.AddinLib
{
	//nb: not really an addin that is discoverable.
	public class ComingSomedayAddin: IWeSayAddin
	{
		private readonly string _name;
		private readonly string _longName;
		private readonly string _shortDescription;
		private readonly Image _buttonImage = null;

		public ComingSomedayAddin(string name, string longName, string shortDescription)
		{
			_name = name;
			_longName = longName;
			_shortDescription = shortDescription;
		}

		public ComingSomedayAddin(string name, string longName, string shortDescription, Image buttonImage)
		{
			_name = name;
			_longName = longName;
			_shortDescription = shortDescription;
			_buttonImage = buttonImage;
		}

		public Image ButtonImage
		{
			get { return _buttonImage; }
		}

		public bool Available
		{
			get { return false; }
		}

		public string LocalizedName
		{
			get { return _name; }
		}

		public string Description
		{
			get { return /*"Coming Someday: "+*/ _shortDescription; }
		}

		#region IWeSayAddin Members

		public object SettingsToPersist
		{
			get { return null; }
			set { throw new NotImplementedException(); }
		}

		public string ID
		{
			get { return _name; }
			set { throw new NotImplementedException(); }
		}

		#endregion

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public string LocalizedLongLabel
		{
			get { return _longName; }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public Image DashboardButtonImage
		{
			get { return null; }
		}

		#endregion

		public void Launch(Form parentForm, ProjectInfo projectInfo) {}
	}
}
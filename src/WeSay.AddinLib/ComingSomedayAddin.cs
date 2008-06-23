using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WeSay.AddinLib
{
	//nb: not really an addin that is discoverable.
	public class ComingSomedayAddin :IWeSayAddin
	{
		private string _name;
		private string _shortDescription;
		private readonly Image _buttonImage=null;

		public ComingSomedayAddin(string name, string shortDescription)
		{
			_name = name;
			_shortDescription = shortDescription;
		}

		public ComingSomedayAddin(string name, string shortDescription, Image buttonImage)
		{
			_name = name;
			_shortDescription = shortDescription;
			_buttonImage = buttonImage;
		}

		public Image ButtonImage
		{
			get
			{
				return _buttonImage;
			}
		}

		public bool Available
		{
			get
			{
				return false;
			}
		}



		public string LocalizedName
		{
			get
			{
				return _name;
			}
		}

		public string Description
		{
			get
			{
				return /*"Coming Someday: "+*/_shortDescription;
			}
		}

		#region IWeSayAddin Members

		public object SettingsToPersist
		{
			get
			{
				return null;
			}
			set
			{

			}
		}

		public string ID
		{
			get
			{
				return _name;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IThingOnDashboard Members

		public WeSay.Foundation.Dashboard.DashboardGroup Group
		{
			get { return WeSay.Foundation.Dashboard.DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get { return this.LocalizedName; }
		}

		public WeSay.Foundation.Dashboard.ButtonStyle DashboardButtonStyle
		{
			get { return WeSay.Foundation.Dashboard.ButtonStyle.IconVariableWidth; }
		}

		public Image DashboardButtonImage
		{
			get { return null; }
		}

		#endregion

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
		}
	}
}

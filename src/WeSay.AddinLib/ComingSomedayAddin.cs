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
		private Guid _id;

		public ComingSomedayAddin(string name, string shortDescription)
		{
			_name = name;
			_shortDescription = shortDescription;
		}

		public Image ButtonImage
		{
			get
			{
				return null;
			}
		}

		public bool Available
		{
			get
			{
				return false;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string ShortDescription
		{
			get
			{
				return "Coming Someday: "+_shortDescription;
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

		public Guid ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		#endregion

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
		}
	}
}

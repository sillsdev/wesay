using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class BackupPlanControl : ConfigurationControlBase
	{
		public BackupPlanControl()
			: base("set up backup plan")
		{
			InitializeComponent();
		}

	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mono.Addins;
using WeSay.AddinLib;
using WeSay.Project;

namespace Addin.Backup
{

	[Extension]
	public class BackupToDevice : IWeSayAddin//, IWeSayProjectAwareAddin
	{
		private WeSayWordsProject _project=null;

		public Image ButtonImage
		{
			get
			{
				return Resources.buttonImage;
			}
		}

		public bool Available
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get
			{
				return "Backup To Device";
			}
		}

		public string ShortDescription
		{
			get
			{
				return "Saves a backup on an external device, like a USB key.";
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
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

//        public WeSayWordsProject Project
//        {
//            set
//            {
//                _project = value;
//            }
//        }

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			BackupDialog d = new BackupDialog(projectInfo);
			d.ShowDialog(parentForm);
		}
	}
}

using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Mono.Addins;

namespace WeSay.AddinLib
{
//    public interface IHasSettings
//    {
//        bool EditSettings();
//    }

	[TypeExtensionPoint]
	public interface IWeSayAddin
	{
		Image ButtonImage { get;}

		bool Available { get;}

		string Name
		{
			get;
		}

		string ShortDescription
		{
			get;
		}

		void Launch(Form parentForm, ProjectInfo projectInfo);
	}

	public interface IWeSayAddinHasSettings
	{
		bool DoShowSettingsDialog(Form parentForm);

		object SettingsToPersist
		{
			get;
			set;
		}

		Guid ID
		{
			get;
			set;
		}
	}

//    public interface IWeSayProjectAwareAddin
//    {
//        WeSay.Project.WeSayWordsProject Project
//        {
//            set;
//        }
//    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.LexicalModel;

namespace WeSay.AddinLib
{
	public delegate string FileLocater(string fileName);

	[TypeExtensionPoint]
	public interface IWeSayAddin: IThingOnDashboard
	{
		Image ButtonImage { get; }

		bool Available { get; }

		string LocalizedName { get; }

		String ID { get; }

		void Launch(Form parentForm, ProjectInfo projectInfo);
	}

	public interface IWeSayAddinHasSettings
	{
		bool DoShowSettingsDialog(Form parentForm, ProjectInfo projectInfo);

		object Settings { get; set; }
	}

	public interface IWeSayAddinHasMoreInfo
	{
		void ShowMoreInfoDialog(Form parentForm);

	}
}
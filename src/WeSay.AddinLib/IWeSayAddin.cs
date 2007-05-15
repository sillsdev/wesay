using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;

namespace WeSay.AddinLib
{
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


//    public interface IWeSayProjectAwareAddin
//    {
//        WeSay.Project.WeSayWordsProject Project
//        {
//            set;
//        }
//    }
}

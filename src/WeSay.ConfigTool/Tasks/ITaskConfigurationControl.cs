using System.Windows.Forms;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{

	public class ConfigTaskControlFactory
	{
		static public Control Create(ITaskConfiguration config)
		{
		   // if (config.GetType() == typeof(MissingInfoConfiguration))
			if (config.TaskName == "AddMissingInfo")
			{
				return new MissingInfoTaskConfigControl(config);
			}
			if (config.TaskName == "GatherWordsBySemanticDomains")
			{
				return new GatherBySemDomTaskConfigControl(config);
			}
			return new DefaultTaskConfigurationControl(config);
		}
	}
//    public delegate Control ConfigTaskControlFactory(ITaskConfiguration config);

//    public delegate ITaskConfigurationControl Factory(ITaskConfiguration config);
//    public interface ITaskConfigurationControl
//    {
//    }
//
//    public class DefaultTaskConfiguration : ITaskConfigurationControl
//    {
//
//    }

}

using System.Windows.Forms;
using Autofac;
using WeSay.LexicalTools.AddMissingInfo;
using WeSay.Project;

namespace WeSay.ConfigTool.Tasks
{
	public class ConfigTaskControlFactory
	{
		static public Control Create(IContext diContainer, ITaskConfiguration config)
		{
			//nb: I tried but didn't figure out how to make autofac select the correct
			//control based on the type of the configuration:
			//            if(diContainer.TryResolve<ITaskConfigurationControl>(out x, new TypedParameter(config.GetType(), config)))


			if (config.GetType() == typeof(MissingInfoConfiguration))
			{
				return diContainer.Resolve<MissingInfoTaskConfigControl>(new TypedParameter(config.GetType(),config) );
			}
			if (config.TaskName == "GatherWordsBySemanticDomains")
			{
				return new GatherBySemDomTaskConfigControl(config);
			}
			return new DefaultTaskConfigurationControl(config, false);
		}
	}
}

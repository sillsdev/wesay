using System.Collections.Generic;
using WeSay.Project;

namespace WeSay.LexicalTools.Review.AdvancedHistory
{
	public interface IAdvancedHistoryTaskConfig : ITaskConfiguration
	{

	}

	public class AdvancedHistoryTaskConfig : TaskConfigurationBase, IAdvancedHistoryTaskConfig
	{
		public AdvancedHistoryTaskConfig(string xml)
			:base(xml)
		{
		}

		protected override IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get
			{
				yield break;
			}
		}

		public bool AreEquivalent(ITaskConfiguration taskConfiguration)
		{
			return taskConfiguration is AdvancedHistoryTaskConfig;
		}

		public override string ToString()
		{
			return LongLabel;
		}

		public bool Available
		{
			get { return string.IsNullOrEmpty(Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en")); }
		}

		public string Label
		{
			get { return "History"; }
		}

		public string LongLabel
		{
			get { return "Advanced History"; }
		}

		public string Description
		{
			get { return "This task is for advisors or very advanced users of projects using Chorus to collaborate as a team. It shows the history of the project: who did what, when.  When time and resources permit, we envision making a different task which brings this information to a wider range of users.  It only works when Chorus is enabled. "+
				Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en");
			}
		}

		public string RemainingCountText
		{
			get { return string.Empty; }
		}

		public string ReferenceCountText
		{
			get { return string.Empty; }
		}

		public bool IsPinned
		{
			get { return false; }
		}

	}
}
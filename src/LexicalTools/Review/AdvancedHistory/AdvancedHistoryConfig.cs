using System;
using System.Collections.Generic;
using WeSay.Project;
using Palaso.i18n;

namespace WeSay.LexicalTools.Review.AdvancedHistory
{
	public interface IAdvancedHistoryConfig : ITaskConfiguration
	{

	}

	public class AdvancedHistoryConfig : TaskConfigurationBase, IAdvancedHistoryConfig
	{
		public AdvancedHistoryConfig(string xml)
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
			return taskConfiguration is AdvancedHistoryConfig;
		}

		public override string ToString()
		{
			return LongLabel;
		}


		public string Label
		{
			get { return StringCatalog.Get("History"); }
		}

		public string LongLabel
		{
			get { return StringCatalog.Get("History"); }
		}

		public string Description
		{
			get {

				string d = Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en");
				if(!string.IsNullOrEmpty(d))
				{
					d= "NOT AVAILABLE ON THIS COMPUTER"+Environment.NewLine + Environment.NewLine+d+Environment.NewLine + Environment.NewLine;
				}

				d+= "This task is for advisors or very advanced users of projects using Chorus to collaborate as a team. It shows the history of the project: who did what, when.  When time and resources permit, we envision making a different task which brings this information to a wider range of users." ;
				return d;
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

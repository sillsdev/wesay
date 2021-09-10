using System;
using System.Collections.Generic;
using WeSay.Project;

namespace WeSay.LexicalTools.Review.NotesBrowser
{
	public interface INotesBrowserConfig : ITaskConfiguration
	{

	}

	public class NotesBrowserConfig : TaskConfigurationBase, INotesBrowserConfig
	{
		public NotesBrowserConfig(string xml)
			: base(xml)
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
			return taskConfiguration is NotesBrowserConfig;
		}

		public override string ToString()
		{
			return LongLabel;
		}


		public string Label
		{
			get { return "Notes"; }
		}

		public string LongLabel
		{
			get { return "Browse Notes In Project"; }
		}

		public string Description
		{
			get
			{
				string d = Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en");
				if (!string.IsNullOrEmpty(d))
				{
					d = "NOT AVAILABLE ON THIS COMPUTER" + Environment.NewLine + Environment.NewLine + d + Environment.NewLine + Environment.NewLine;
				}

				d += "This task lets you search for and view notes attached anywhere in the dictionary project.";
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
		public override bool IsAvailable
		{
			get { return string.IsNullOrEmpty(Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en")); }
		}

	}
}
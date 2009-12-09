using System.Collections.Generic;
using WeSay.LexicalTools.Review.AdvancedHistory;
using WeSay.Project;

namespace WeSay.LexicalTools.Review.NotesBrowser
{
	public interface INotesBrowserConfig : ITaskConfiguration
	{

	}

	public class NotesBrowserConfig : TaskConfigurationBase, INotesBrowserConfig
	{
		public NotesBrowserConfig(string xml)
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
			return taskConfiguration is NotesBrowserConfig;
		}

		public override string ToString()
		{
			return LongLabel;
		}

		public bool Available
		{
			get { return true; }
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
				return "This task lets you search for and view notes attached anywhere in the dictionary project.";
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
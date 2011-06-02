
using System.Xml;

namespace WeSay.Project
{

	public interface ITaskConfiguration
	{
		string TaskName { get; }
		string Label {get; }
		string LongLabel {get; }
		string Description { get; }
		string RemainingCountText {get; }
		string ReferenceCountText { get; }
		bool IsPinned { get; }
		bool IsVisible { get; set; }
		bool IsAvailable{get; }
		bool IsOptional { get; }
		void Write(XmlWriter writer);

		/// <summary>
		/// Tells whether a task found in another file (e.g. factory defaults) is already accounted for by this one
		/// </summary>
		bool AreEquivalent(ITaskConfiguration taskConfiguration);
	}

	public interface ICareThatWritingSystemIdChanged
	{
		void OnWritingSystemIdChanged(string from, string to);
	}
}

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
		bool IsOptional { get; }
	}

	public interface ICareThatWritingSystemIdChanged
	{
		void WritingSystemIdChanged(string from, string to);

	}
}
using SIL.WritingSystems;

namespace WeSay.UI
{
	public interface IControlThatKnowsWritingSystem
	{
		string Name { get; }
		WritingSystemDefinition WritingSystem { get; }
		string Text { get; set; }
	}
}
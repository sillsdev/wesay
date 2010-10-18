using WeSay.LexicalModel.Foundation;

namespace WeSay.UI
{
	public interface IControlThatKnowsWritingSystem
	{
		string Name { get; }
		WritingSystem WritingSystem { get; }
		string Text { get; set; }
	}
}
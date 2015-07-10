using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI
{
	public interface IControlThatKnowsWritingSystem
	{
		string Name { get; }
		WritingSystemDefinition WritingSystem { get; }
		string Text { get; set; }
	}
}
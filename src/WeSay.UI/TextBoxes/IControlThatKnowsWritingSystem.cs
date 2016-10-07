using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI
{
	public interface IControlThatKnowsWritingSystem
	{
		string Name { get; }
		IWritingSystemDefinition WritingSystem { get; }
		string Text { get; set; }
	}
}
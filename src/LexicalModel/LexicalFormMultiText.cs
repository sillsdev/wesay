using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexicalFormMultiText : MultiText
	{
		public override string ToString()
		{
			return GetFirstAlternative();
		}
	}
}

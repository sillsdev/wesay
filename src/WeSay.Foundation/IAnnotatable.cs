using System.Xml.Serialization;

namespace WeSay.Foundation
{
	public interface IAnnotatable
	{
		bool IsStarred { get; set; }
	}
}
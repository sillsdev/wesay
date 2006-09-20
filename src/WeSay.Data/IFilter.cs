using System;
namespace WeSay.LexicalModel
{
	public interface IFilter
	{
		Predicate<object> Inquire
		{
			get;
		}
	}
}

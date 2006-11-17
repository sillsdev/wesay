using System;

namespace WeSay.Foundation
{
	/// <summary>
	/// Use to mark fields that should not be persisted.
	/// </summary>
	/// <remarks>Having this here allows the use of the attribute without
	/// introducing a dll reference from the model to the persistence layer.</remarks>
	public class TransientAttribute : Attribute
	{
	}
}

namespace WeSay.Language
{
	/// <summary>
	/// An annotation is a like a "flag" on a field. You can say, e.g., "I'm not sure about this"
	/// </summary>
	public class Annotation
	{
		/// <summary>
		/// 0 means "off".  1 means "starred". In the future, other positive values could correspond to other icon.
		/// </summary>
		private int _status = 0;

		public Annotation()
		{
			IsOn = false;
		}

		public bool IsOn
		{
			get
			{
				return _status != 0;
			}
			set
			{
				_status = value?1:0;
			}
		}
	}
}
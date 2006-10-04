namespace WeSay.UI
{
	public interface ICurrentWorkTask
	{
		WeSay.UI.ITask CurrentWorkTask
		{
			get;
		}

		WeSay.UI.ITask ActiveTask
		{
			get;
			set;
		}
	}
}

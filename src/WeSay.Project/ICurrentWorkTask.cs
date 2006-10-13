namespace WeSay.Project
{
	public interface ICurrentWorkTask
	{
		ITask CurrentWorkTask
		{
			get;
		}

		ITask ActiveTask
		{
			get;
			set;
		}
	}
}

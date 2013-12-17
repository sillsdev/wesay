namespace WeSay.Project
{
	public interface ICurrentWorkTask
	{
		ITask CurrentWorkTask { get; }

		ITask ActiveTask { get; set; }
	}

	public class EmptyCurrentWorkTask: ICurrentWorkTask
	{
		#region ICurrentWorkTask Members

		public ITask CurrentWorkTask
		{
			get { return null; }
		}

		public ITask ActiveTask
		{
			get { return null; }
			set { }
		}

		#endregion
	}
}
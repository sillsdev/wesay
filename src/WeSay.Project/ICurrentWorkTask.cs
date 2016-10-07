namespace WeSay.Project
{
	public interface ICurrentWorkTask
	{
		ITask CurrentWorkTask { get; }

		ITask ActiveTask { get;}

		///this is not part of the property because it is very slow, and property setters are supposed to be trivial
		void SetActiveTask(ITask task);
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
		}

		public void SetActiveTask(ITask task)
		{

		}

		#endregion
	}
}
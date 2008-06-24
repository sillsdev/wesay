using System.Collections.Generic;

namespace WeSay.Project
{
	public interface ITaskBuilder
	{
		IList<ITask> Tasks { get; }
	}
}
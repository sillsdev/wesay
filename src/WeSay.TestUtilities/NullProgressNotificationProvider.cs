// // Copyright (c) -2021 SIL International
// // This software is licensed under the LGPL, version 2.1 or later
// // (http://www.gnu.org/licenses/lgpl-2.1.html)

using SIL.Progress;
using SIL.Windows.Forms.Progress;

namespace WeSay.TestUtilities
{
	public class NullProgressNotificationProvider : IProgressNotificationProvider
	{
		public T Go<T>(string taskDescription, ActionThatReportsProgress<T> thingToDo)
		{
			return thingToDo(new NullProgressState());
		}
	}
}
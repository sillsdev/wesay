using System;
using WeSay.Foundation.Progress;

namespace WeSay.Foundation
{
	public class ConsoleProgress : ProgressState
	{

		public ConsoleProgress() : base()
		{

		}

		public override string StatusLabel
		{
//            get
//            {
//                return _statusLabel;
//            }
			set
			{
				Console.WriteLine(value);
				base.StatusLabel = value;// _statusLabel = value;
			}
		}
		public override int NumberOfStepsCompleted
		{
//            get
//            {
//                return _numberOfStepsCompleted;
//            }
			set
			{
				Console.Write('.');
				base.NumberOfStepsCompleted = value;
			}
		}
	}
}
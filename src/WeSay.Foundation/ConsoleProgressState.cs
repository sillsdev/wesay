using System;
using WeSay.Foundation.Progress;

namespace WeSay.Foundation
{
	public class ConsoleProgress : ProgressState
	{
		int _numberOfSteps;
		int _numberOfStepsCompleted;
		string _statusLabel;

		public ConsoleProgress() : base()
		{

		}
		public override int NumberOfSteps
		{
			get
			{
				return _numberOfSteps;
			}
			set
			{
				_numberOfSteps = value;
			}
		}
		public override string StatusLabel
		{
			get
			{
				return _statusLabel;
			}
			set
			{
				Console.WriteLine(value);
				_statusLabel = value;
			}
		}
		public override int NumberOfStepsCompleted
		{
			get
			{
				return _numberOfStepsCompleted;
			}
			set
			{
				Console.Write('.');
				_numberOfStepsCompleted = value;
			}
		}
	}
}
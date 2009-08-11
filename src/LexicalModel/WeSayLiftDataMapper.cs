using Palaso.Lift;
using Palaso.Progress;

using WeSay.Foundation.Options; // review cp move to palaso?

namespace WeSay.LexicalModel
{
	public class WeSayLiftDataMapper : LiftDataMapper<LexEntry>
	{
		private class WeSayLiftReaderWriterProvider : ILiftReaderWriterProvider<LexEntry>
		{
			private readonly ProgressState _progressState;
			private readonly OptionsList _semanticDomainsList;

			public WeSayLiftReaderWriterProvider(ProgressState progressState, OptionsList semanticDomainsList)
			{
				_progressState = progressState;
				_semanticDomainsList = semanticDomainsList;
			}

			public ILiftWriter<LexEntry> CreateWriter(string liftFilePath)
			{
				return new WeSayLiftWriter(liftFilePath);
			}

			public ILiftReader<LexEntry> CreateReader()
			{
				return new WeSayLiftReader(_progressState, _semanticDomainsList);
			}
		}

		public WeSayLiftDataMapper(string filePath, OptionsList semanticDomainsList, ProgressState progressState)
			: base(filePath, progressState, new WeSayLiftReaderWriterProvider(progressState, semanticDomainsList))
		{
		}

		public WeSayLiftDataMapper(string filePath)
			: this(filePath, null, new ProgressState())
		{
		}

	}

}
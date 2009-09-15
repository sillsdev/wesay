using System.Collections.Generic;
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
			private readonly IEnumerable<string> _idsOfSingleOptionFields;

			public WeSayLiftReaderWriterProvider(ProgressState progressState, OptionsList semanticDomainsList, IEnumerable<string> idsOfSingleOptionFields)
			{
				_progressState = progressState;
				_semanticDomainsList = semanticDomainsList;
				_idsOfSingleOptionFields = idsOfSingleOptionFields;
			}

			public ILiftWriter<LexEntry> CreateWriter(string liftFilePath)
			{
				return new WeSayLiftWriter(liftFilePath);
			}

			public ILiftReader<LexEntry> CreateReader()
			{
				return new WeSayLiftReader(_progressState, _semanticDomainsList, _idsOfSingleOptionFields);
			}
		}

		public WeSayLiftDataMapper(string filePath, OptionsList semanticDomainsList, IEnumerable<string> idsOfSingleOptionFields, ProgressState progressState)
			: base(filePath, progressState, new WeSayLiftReaderWriterProvider(progressState, semanticDomainsList, idsOfSingleOptionFields))
		{
		}

		/// <summary>
		/// unit tests only
		/// </summary>
		/// <param name="filePath"></param>
		public WeSayLiftDataMapper(string filePath)
			: this(filePath, new OptionsList(), new string[]{}, new ProgressState())
		{
		}

	}

}
using Palaso.Lift;
using Palaso.Progress;

using WeSay.Foundation.Options; // review cp move to palaso?

namespace WeSay.LexicalModel
{
	public class WeSayLiftDataMapper : LiftDataMapper<LexEntry>
	{
		public WeSayLiftDataMapper(string filePath, OptionsList semanticDomainsList, ProgressState progressState, ILiftReaderWriterProvider<LexEntry> readerWriter)
			: base(filePath, progressState, readerWriter)
		{
		}

		/// <summary>
		/// unit tests only
		/// </summary>
		/// <param name="filePath"></param>
		internal WeSayLiftDataMapper(string filePath)
			: this(filePath, null, new ProgressState(), new WeSayLiftReaderWriterProvider(new ProgressState(), new OptionsList(), new string[]{} ))
		{
		}

	}
}
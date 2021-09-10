namespace WeSay.LexicalTools
{
	public delegate IConfirmDelete ConfirmDeleteFactory();

	public interface IConfirmDelete
	{
		bool DeleteConfirmed { get; }
		string Message { get; set; }
	}
}

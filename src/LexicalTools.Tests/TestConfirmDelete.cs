namespace WeSay.LexicalTools.Tests
{
	class TestConfirmDelete : IConfirmDelete
	{
		private bool _confirm = true;
		public bool DeleteConfirmed { get { return _confirm; } set { _confirm = value; } }
		public string Message { get; set; }
	}
}

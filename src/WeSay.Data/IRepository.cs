namespace WeSay.Data
{
	public interface IRepository<T>
	{
		T GetItem(RepositoryId id);
		RepositoryId GetId(T item);
	}
}
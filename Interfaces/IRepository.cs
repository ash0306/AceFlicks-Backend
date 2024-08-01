namespace MovieBookingBackend.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> GetById(K key);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T item);
        Task<T> Update(T item);
        Task<T> Delete(K key);
        Task<bool> DeleteRange(IList<K> key);
    }
}

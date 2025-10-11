namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IBaseRepository<T>
{
    Task<long> CreateAsync(T data);
    Task<bool> UpdateAsync(T data);
    Task<bool> DeleteAsync(T data);
    
    Task<bool> SaveAsync();
}
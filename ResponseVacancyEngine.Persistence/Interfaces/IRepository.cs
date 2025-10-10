namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IBaseRepository<T>
{
    Task<bool> Create(T data);
    Task<bool> Update(T data);
    Task<bool> Delete(T data);
    
    Task<bool> Save();
}
namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IBaseRepository<T>
{
    Task<bool> CreateAccount(T data);
    Task<bool> UpdateAccount(T data);
    Task<bool> DeleteAccount(T data);
    
    Task<bool> Save();
}
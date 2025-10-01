namespace ResponseVacancyEngine.Persistence.Data;

public interface IVacancyContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
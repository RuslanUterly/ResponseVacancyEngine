using Microsoft.EntityFrameworkCore;
using ResponseVacancyEngine.Infrastructure.Persistence.Configurations;
using ResponseVacancyEngine.Persistence.Data;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Persistence;

public class VacancyContext(DbContextOptions<VacancyContext> options) : DbContext(options), IVacancyContext
{
    public DbSet<Group>? Groups { get; set; }
    public DbSet<ExcludedWord>?  WordOfExceptions { get; set; }
    public DbSet<RespondedVacancy>? RespondedVacancies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new ExcludedWordConfiguration());
        modelBuilder.ApplyConfiguration(new RespondedVacancyConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
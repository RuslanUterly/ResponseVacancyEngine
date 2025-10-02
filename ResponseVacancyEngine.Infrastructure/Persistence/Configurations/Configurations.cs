using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.OwnsOne(x => x.Settings, settings =>
        {
            settings.ToJson();
        });
        
        builder.HasMany(x => x.ExcludedWords)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.RespondedVacancies)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(x => x.Account)
            .WithMany(x => x.Groups)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ExcludedWordConfiguration : IEntityTypeConfiguration<ExcludedWord>
{
    public void Configure(EntityTypeBuilder<ExcludedWord> builder)
    {
        builder.HasKey(x => x.GroupId);
        
        builder.Property(x => x.Words)
            .HasColumnType("text[]");
        
        builder.HasOne(e => e.Group)
            .WithMany(g => g.ExcludedWords)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RespondedVacancyConfiguration : IEntityTypeConfiguration<RespondedVacancy>
{
    public void Configure(EntityTypeBuilder<RespondedVacancy> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(v => v.Group)
            .WithMany(g => g.RespondedVacancies)
            .HasForeignKey(v => v.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(v => v.Account)
            .WithMany(a => a.RespondedVacancies)
            .HasForeignKey(v => v.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasMany(x => x.Groups)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.RespondedVacancies)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}




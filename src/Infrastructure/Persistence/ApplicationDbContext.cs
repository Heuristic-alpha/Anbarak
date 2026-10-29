using Anbarak.Application.Abstractions.Identity;
using Anbarak.Application.Abstractions.Persistence;
using Anbarak.Domain.Common;
using Anbarak.Domain.Entities;
using Anbarak.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Anbarak.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Constance.Schemas.IDENTITY);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        return SaveChanges(acceptAllChangesOnSuccess: true);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInformation();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(
            acceptAllChangesOnSuccess: true,
            cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();

        return base.SaveChangesAsync(
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var UtcNow = DateTime.UtcNow;
        var userId = _currentUserService.UserId;

        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = UtcNow;
                entry.Entity.CreatedBy = userId;

                entry.Entity.LastModified = UtcNow;
                entry.Entity.LastModifiedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Never overwrite the original creation information.
                entry.Property(entity => entity.Created).IsModified = false;
                entry.Property(entity => entity.CreatedBy).IsModified = false;

                entry.Entity.LastModified = UtcNow;
                entry.Entity.LastModifiedBy = userId;
            }
        }
    }
}

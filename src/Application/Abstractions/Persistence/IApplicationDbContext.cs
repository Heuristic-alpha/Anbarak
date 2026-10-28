using Anbarak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anbarak.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

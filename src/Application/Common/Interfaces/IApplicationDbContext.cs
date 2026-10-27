using Anbarak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanTest.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

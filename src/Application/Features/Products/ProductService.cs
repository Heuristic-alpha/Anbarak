using Anbarak.Application.Abstractions.Persistence;
using Anbarak.Contracts.DTOs.Products;
using Microsoft.EntityFrameworkCore;

namespace Anbarak.Application.Features.Products;

public sealed class ProductService(IApplicationDbContext dbContext)
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = dbContext.Products.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(product => product.IsActive);
        }

        return await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Sku)
            .Select(product => new ProductDto(
                product.Id,
                product.Sku,
                product.Name,
                product.Description,
                product.UnitOfMeasure.ToString(),
                product.IsActive,
                product.Created,
                product.LastModified))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductDto(
                product.Id,
                product.Sku,
                product.Name,
                product.Description,
                product.UnitOfMeasure.ToString(),
                product.IsActive,
                product.Created,
                product.LastModified))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductDto> CreateAsync(CreateProductInput input, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UnitOfMeasure>(input.UnitOfMeasure, ignoreCase: true, out var unitOfMeasure) || !Enum.IsDefined(unitOfMeasure))
        {
            throw new ArgumentException("The specified unit of measure is invalid.", nameof(input.UnitOfMeasure));
        }

        // The Domain entity validates the input and normalizes the SKU.
        var product = new Product(
            input.Sku,
            input.Name,
            unitOfMeasure,
            input.Description);

        // Check the normalized SKU before inserting.
        var skuExists = await dbContext.Products.AnyAsync(existing => existing.Sku == product.Sku, cancellationToken);

        if (skuExists)
        {
            throw new ProductSkuAlreadyExistsException(product.Sku);
        }

        await dbContext.Products.AddAsync(product, cancellationToken);

        // ApplicationDbContext assigns audit timestamps during saving.
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<ProductDto?> UpdateAsync(long id, UpdateProductInput input, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        // Domain rules remain inside the entity.
        product.UpdateDetails(input.Name, input.Description);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<bool> DeactivateAsync(long id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Deactivate();

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivateAsync(long id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Activate();

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.UnitOfMeasure.ToString(),
            product.IsActive,
            product.Created,
            product.LastModified);
    }
}

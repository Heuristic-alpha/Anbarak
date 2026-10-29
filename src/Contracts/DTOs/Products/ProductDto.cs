
namespace Anbarak.Contracts.DTOs.Products;

public sealed record ProductDto
    (
long Id,
string Sku,
string Name,
string? Description,
string UnitOfMeasure,
bool IsActive,
DateTime Created,
DateTime LastModified
    );
